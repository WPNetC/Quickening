﻿using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Quickening.Services
{
    internal class ParserService
    {
        #region XML Method

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
                var nodeDir = node.Name;
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
                // Create absolute and relative paths.
                var absPath = !projectItem.Key.StartsWith(projectDirectory) ? Path.Combine(projectDirectory, projectItem.Key) : projectItem.Key;
                var relPath = projectItem.Key.StartsWith(projectDirectory) ? projectItem.Key.Replace(projectDirectory, "") : projectItem.Key;

                // If no extension it must be a directory.
                if (string.IsNullOrEmpty(Path.GetExtension(absPath)))
                {
                    if (!Directory.Exists(absPath))
                    {
                        // Both these methods create the directory, so we only want to use 1.
                        if (projectItem.Value.Include)
                            IDEService.TraverseProjectItem(project, relPath, absPath);
                        else
                            Directory.CreateDirectory(absPath);
                    }
                }
                else
                {
                    if (!File.Exists(absPath))
                    {
                        // Check parent directory exists before creating file.
                        var dir = Path.GetDirectoryName(absPath);
                        if (!Directory.Exists(dir))
                        {
                            if (projectItem.Value.Include)
                            {
                                // Create relative path for parent directory.
                                var parPath = projectItem.Key.StartsWith(dir) ? projectItem.Key.Replace(dir, "") : projectItem.Key;
                                // Add to solution. This also creates the item.
                                IDEService.TraverseProjectItem(project, parPath, absPath);
                            }
                            else
                                Directory.CreateDirectory(dir);
                        }
                        
                        // If there is a template id.
                        if (!string.IsNullOrEmpty(projectItem.Value.TemplateId))
                        {
                            var templatePath = Path.Combine(ProjectService.TemplatesDirectory, projectItem.Value + ".txt");
                            if (File.Exists(templatePath))
                            {
                                string text = File.ReadAllText(templatePath);

                                if (projectItem.Value.Include)
                                    IDEService.TraverseProjectItem(project, relPath, absPath);
                                else
                                    File.WriteAllText(absPath, text);
                            }
                            else
                            {
                                Console.WriteLine("Cannot find template file: " + templatePath);
                            }
                        }
                        else
                        {
                            if (projectItem.Value.Include)
                                IDEService.TraverseProjectItem(project, relPath, absPath);
                            else
                                File.Create(absPath).Dispose();
                        }
                    }
                }
            }
        }

        private void AddPathsFromXmlNode(XmlNode node, ref Dictionary<string, AttributeSet> paths)
        {
            // Don't use our own tags on paths
            var rel = ProjectService.ReservedTagsXml.Contains(node.Name) ? "" : node.Name;

            // Go back up tree until we hit root node.
            var parent = node.ParentNode;
            while (parent != null
                && parent.Name.ToLower() != "__root__")
            {
                // Don't use our own tags in paths
                if (!ProjectService.ReservedTagsXml.Contains(parent.Name))
                    rel = Path.Combine(parent.Name, rel);
                parent = parent.ParentNode;
            }

            // This seems to pass on all nodes. Seems text content might count as a child node.
            if (node.HasChildNodes)
            {
                // If we are in the 'files' node.
                if (node.Name.ToLower() == "__files__")
                {
                    // Itterate through each file node.
                    foreach (XmlNode fileNode in node.ChildNodes)
                    {
                        // Generate relative path from inner text of file node.
                        var filePath = Path.Combine(rel, fileNode.InnerText);
                        var fileAttrs = AttributeSet.FromXmlNode(fileNode);
                        if (!paths.Keys.Contains(filePath))
                            paths.Add(filePath, fileAttrs);
                    }
                }
                // Otherwise it is a directory node.
                else
                {
                    // Itterate through all sub directories and files.
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        AddPathsFromXmlNode(child, ref paths);
                        var childAttrs = AttributeSet.FromXmlNode(child);
                        if (!paths.Keys.Contains(rel))
                            paths.Add(rel, childAttrs);
                    }
                }

            }
            // In case did not pass last check we are assuming it is an empty directory.
            else
            {
                if (!paths.Keys.Contains(rel))
                {
                    var attrs = AttributeSet.FromXmlNode(node);
                    paths.Add(rel, attrs);
                }
            }
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
