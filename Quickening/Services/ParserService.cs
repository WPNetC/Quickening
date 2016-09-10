using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Quickening.Services
{
    internal class ParserService
    {
        #region XML Methods

        /// <summary>
        /// Parse Xml file into dictionary of relative paths and template ids.
        /// <para>Optionally writes structure to project folder (defaults to true).</para>
        /// </summary>
        /// <param name="xmlPath">Path to the Xml template file.</param>
        /// <param name="write">If the structure should be written to disk.</param>
        /// <returns></returns>
        public Dictionary<string, AttributeSet> PaseXML(string xmlPath, bool write = true)
        {
            // Take reference to project and directory to ensure it doesn't change mid method.
            var project = ProjectService.CurrentProject;
            string projectDirectory = ProjectService.ProjectDirectory;

            // Read Xml file.
            var doc = new XmlDocument();
            try
            {
                doc.Load(xmlPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            // Skip to root node.
            var root = doc.ChildNodes[0];

            // Create list to hold unique paths and template ids.
            var paths = new Dictionary<string, AttributeSet>();

            // Itterate through XML and populate list.
            foreach (XmlNode node in root.ChildNodes)
            {
                var nodeDir = node.Attributes[ProjectService.Attributes[XmlAttributeName.Name]].Value;
                paths.Add(nodeDir, AttributeSet.FromXmlNode(node));

                var children = node.ChildNodes;
                foreach (XmlNode child in children)
                {
                    AddPathsFromXmlNode(child, ref paths);
                }
            }

            if (write)
                WriteStructure(project, projectDirectory, paths);

            return paths;
        }

        /// <summary>
        /// Write the parsed structure to disk.
        /// </summary>
        /// <param name="projectDirectory"></param>
        /// <param name="projectItems"></param>
        public void WriteStructure(Project project, string projectDirectory, Dictionary<string, AttributeSet> projectItems)
        {
            // Itterate through unique paths and create files and folders.
            foreach (var projectItem in projectItems)
            {
                // Don't create a 'root' element as this is just a placeholder.
                if (projectItem.Value.NodeType == Globals.ProjectItemType.Root)
                    continue;

                string absPath, relPath;
                bool pathIsAbsolute = projectItem.Key.StartsWith(projectDirectory);

                // Create absolute and relative paths.
                if (pathIsAbsolute)
                {
                    absPath = projectItem.Key;
                    relPath = projectItem.Key.Replace(projectDirectory, "");
                }
                else
                {
                    absPath = Path.Combine(projectDirectory, projectItem.Key);
                    relPath = projectItem.Key;
                }

                // If item is a folder.
                if (projectItem.Value.NodeType == Globals.ProjectItemType.Folder &&
                    !Directory.Exists(absPath))
                {
                    // Both these methods create the directory, so we only want to use 1.
                    if (projectItem.Value.Include)
                        IDEService.AddItemToProject(project, relPath, absPath, false);
                    else
                        Directory.CreateDirectory(absPath);

                    continue;
                }

                // If item is a file.
                else if (!File.Exists(absPath))
                {
                    // Check parent directory exists before creating file.
                    var parentDir = Path.GetDirectoryName(absPath);
                    if (!Directory.Exists(parentDir))
                    {
                        if (projectItem.Value.Include)
                        {
                            // Create relative path for parent directory.
                            var parPath = projectItem.Key.StartsWith(parentDir) ? projectItem.Key.Replace(parentDir, "") : projectItem.Key;

                            // Add to solution. This also creates the item.
                            IDEService.AddItemToProject(project, parPath, absPath, true);
                        }
                        else
                            Directory.CreateDirectory(parentDir);
                    }

                    // If there is a template id.
                    if (!string.IsNullOrEmpty(projectItem.Value.TemplateId))
                    {
                        // Create path to template.
                        var templatePath = Path.Combine(ProjectService.TemplatesDirectory, projectItem.Value.TemplateId + ".txt");

                        // Check template exists.
                        if (File.Exists(templatePath))
                        {
                            // Create file from template.
                            string text = File.ReadAllText(templatePath);

                            // Add to project if requested.
                            if (projectItem.Value.Include)
                                IDEService.AddItemToProject(project, relPath, absPath, true);

                            // Write template content to file.
                            File.WriteAllText(absPath, text);

                            continue;
                        }
                        // If not we have an error, but don't need to throw an exception.
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("Cannot find template file: " + templatePath + " for file '" + relPath + "'."
                                + "\r\n Empty file will be created instead.", "Error");
                        }
                    }

                    // Create empty file.
                    File.Create(absPath).Dispose();

                    // Add to project if requested.
                    if (projectItem.Value.Include)
                        IDEService.AddItemToProject(project, relPath, absPath, true);
                }
            }
        }

        private void AddPathsFromXmlNode(XmlNode node, ref Dictionary<string, AttributeSet> paths)
        {
            // Don't use our own tags on paths
            var rel = ProjectService.ReservedTagsXml.Contains(node.Attributes[ProjectService.Attributes[XmlAttributeName.Name]].Value) ?
                "" :
                node.Attributes[ProjectService.Attributes[XmlAttributeName.Name]].Value;

            // Go back up tree until we hit root node.
            var parent = node.ParentNode;
            while (parent != null &&
                (parent.Name?.ToLower() != ProjectService.ROOT_TAG) &&
                parent.Attributes[ProjectService.Attributes[XmlAttributeName.Name]]?.Value?.ToLower() != ProjectService.ROOT_TAG)
            {
                // Don't use our own tags in paths
                if (!ProjectService.ReservedTagsXml.Contains(parent.Attributes[ProjectService.Attributes[XmlAttributeName.Name]].Value))
                    rel = Path.Combine(parent.Attributes[ProjectService.Attributes[XmlAttributeName.Name]].Value, rel);
                parent = parent.ParentNode;
            }

            if (node.HasChildNodes)
            {
                // Itterate through all sub directories and files.
                foreach (XmlNode child in node.ChildNodes)
                {
                    AddPathsFromXmlNode(child, ref paths);
                }
            }
            else if (!paths.Keys.Contains(rel))
                    paths.Add(rel, AttributeSet.FromXmlNode(node));
        }

        #endregion

        #region JSON Method
        // JSON method not used. Left in case is decided to reimplement.
        // NOTE: Will require Newtonsoft if re-enabled.
        /*
        private static readonly string[] reservedTagsJson = new[] { "/files", "/dirs" };
        static string GetJson()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.json");
            string json = File.ReadAllText(path);
            return json;
        }
        private static void ParseJson(string json)
        {
            var jObj = JObject.Parse(json);
            var jSer = JsonConvert.DeserializeObject(json);

            foreach (var kvp in jObj)
            {
                var key = kvp.Key;
                var path = Path.Combine(basePath, key);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var token = kvp.Value;
                foreach (var item in token)
                {
                    CreateItemFromToken(item);
                }
            }

            Console.WriteLine("Finished");
        }
        private static void CreateItemFromToken(JToken jObj)
        {
            if (jObj.HasValues)
            {
                var rel = jObj.Path.Replace(".", "\\");

                if (rel.EndsWith("\\/files"))
                {
                    CreateFile(jObj);
                }
                else if (rel.EndsWith("\\/dirs"))
                {
                    CreateDirectory(jObj);
                }
                else
                {
                    foreach (var value in jObj.Values())
                    {
                        CreateItemFromToken(value);
                    }
                }
            }
            else
            {
                CreateUndeclared(jObj);
            }
        }
        private static void CreateDirectory(JToken jObj)
        {
            var dirsPath = jObj.Path.Replace(".", "\\").Replace("\\/dirs", "");
            var dirNames = jObj.Values();
            foreach (var dir in dirNames)
            {
                var dirName = dir.Value<string>();
                var dirPath = Path.Combine(basePath, dirsPath, dirName);

                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
            }
        }
        private static void CreateFile(JToken jObj)
        {
            var filesPath = jObj.Path.Replace(".", "\\").Replace("\\/files", "");
            var fileNames = jObj.Values();
            foreach (var file in fileNames)
            {
                var fileName = file.Value<string>();
                var filePath = Path.Combine(basePath, filesPath, fileName);
                var dir = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                if (!File.Exists(filePath))
                    File.Create(filePath).Dispose();
            }
        }
        private static void CreateUndeclared(JToken jObj)
        {
            var reld = jObj.Path.Replace(".", "\\");
            var path = Path.Combine(basePath, reld);
            if (string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            else if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
        }
        */
        #endregion
    }
}
