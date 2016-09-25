using Microsoft.VisualStudio.Shell.Interop;
using Quickening.Globals;
using Quickening.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quickening.Services
{
    public class AngularService
    {
        public string CreateController(string xmlPath)
        {
            // Take reference to project and directory to ensure it doesn't change mid method.
            var project = Strings.CurrentProject;
            string projectDirectory = Strings.ProjectDirectory;

            // Launch textbox pop-up.
            using (var tbp = new TextInputPopUp(
                "Filename*",
                "Module (uses 'ngMain' if blank)",
                "Controller Name (uses filename if blank)",
                "Inject (seperate with pipes)"))
            {
                var dr = tbp.ShowDialog();

                if (dr != true)
                    return null;

                // Remove .js extension if present as we may use this for other values.
                var fileName = tbp.Values[0][1]?.Replace(".js", "");
                if (string.IsNullOrEmpty(fileName?.Trim()))
                    throw new ArgumentNullException("FileName", "FileName cannot be left empty");

                var modName = tbp.Values[1][1]?.Trim();
                if (string.IsNullOrEmpty(modName))
                    modName = "ngMain";

                var ctrlName = tbp.Values[2][1]?.Trim();
                if (string.IsNullOrEmpty(ctrlName))
                    ctrlName = fileName;

                // Create injection string.
                var injects = tbp.Values[3][1]?.Trim().Split('|').Where(p => !string.IsNullOrEmpty(p.Trim()));
                var injectString = "";
                if (injects?.Count() > 0)
                {
                    foreach (var inject in injects)
                    {
                        injectString += $"'{inject}', ";
                    }
                    injectString = injectString.Substring(0, injectString.Length - 2);
                }

                fileName += ".js"; // Add file extension now we have set all values.

                // Create base list of replacements.
                var replacements = new Dictionary<string, string>()
                {
                    {Strings.FILENAME_KEY, fileName },
                    {Strings.NG_MODULE_KEY, modName },
                    {Strings.NG_CONTROLLER_KEY, ctrlName },
                    {Strings.NG_INJECTION_ARRAY_KEY, injectString },
                    {Strings.NG_INJECTION_PROPERTIES_KEY, injectString.Replace("'", "") }
                };

                var attrs = CreateAttributesFromXml(xmlPath, fileName);

                // This should only return 1 path as we should have only passed in 1 result.
                var absPaths = ParserService.WriteStructure(project, projectDirectory, attrs);

                foreach (var filePath in absPaths)
                {
                    if (!File.Exists(filePath))
                        continue;

                    StringBuilder sb = new StringBuilder();
                    var lines = File.ReadAllLines(filePath);

                    // Used to detect skipped injection statement in order to remove extra space from file.
                    bool isLineAfterSkippedInject = false;

                    // Replace on a line-by-line basis so we can exclude lines if wanted.
                    foreach (var line in lines)
                    {
                        // If we have no injects we can skip adding the line.
                        if (isLineAfterSkippedInject)
                        {
                            isLineAfterSkippedInject = false;
                            continue;
                        }
                        else if (line.Contains(Strings.NG_INJECTION_ARRAY_KEY) &&
                            string.IsNullOrEmpty(injectString.Trim()))
                        {
                            isLineAfterSkippedInject = true;
                            continue;
                        }

                        sb.AppendLine(RepalceKeys(line, replacements));
                    }

                    File.WriteAllText(filePath, sb.ToString());
                }

                return ctrlName;
            }

        }

        private Dictionary<string, AttributeSet> CreateAttributesFromXml(string xmlPath, string fileName)
        {
            // Parse XML layout, but filter to return just the controller attributes.
            var parsedResults = ParserService.PaseXML(xmlPath, false)
                .Where(p => p.Value.NodeType == ProjectItemType.File);

            // This should only hold 1 result at the end.
            var attrs = new Dictionary<string, AttributeSet>();

            // Go through results and add to new dictionary.
            foreach (var parsedResult in parsedResults)
            {
                try
                {
                    var k = parsedResult.Key?.Replace(Strings.FILENAME_KEY, fileName);
                    var v = parsedResult.Value;
                    if (v == null)
                        continue;

                    v.ProjectItemName = fileName;
                    attrs.Add(k, v);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }

            return attrs;
        }

        private string RepalceKeys(string input, Dictionary<string, string> replacements)
        {
            foreach (var item in replacements)
            {
                input = input.Replace(item.Key, item.Value);
            }
            return input;
        }
    }
}
