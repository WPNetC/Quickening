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
                "Controller Name (uses filename if blank)"))
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
                
                fileName += ".js"; // Add file extension now we have set all values.

                // Parse XML layout, but filter to return just the controller attributes.
                var ps = new ParserService();
                var parsedResults = ps.PaseXML(xmlPath, false)
                    .Where(p => p.Value.NodeType == ProjectItemType.File);

                // This should only hold 1 result at the end.
                Dictionary<string, AttributeSet> attrs = new Dictionary<string, AttributeSet>();

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
                    catch(Exception ex)
                    {
                        continue;
                    }
                }

                // This should only return 1 path as we should have only passed in 1 result.
                var absPaths = ps.WriteStructure(project, projectDirectory, attrs);

                foreach (var filePath in absPaths)
                {
                    if (!File.Exists(filePath))
                        continue;

                    var text = File.ReadAllText(filePath)
                        .Replace(Strings.FILENAME_KEY, fileName)
                        .Replace(Strings.NG_MODULE_KEY, modName)
                        .Replace(Strings.NG_CONTROLLER_KEY, ctrlName);

                    File.WriteAllText(filePath, text);
                }

                return ctrlName;
            }
        }
    }
}
