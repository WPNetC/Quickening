using EnvDTE;
using Quickening.Services;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Quickening.Globals
{
    internal static class Strings
    {
        public const int XmlLayoutVersion = 3;
        public const string ROOT_TAG = "root";
        public const string FOLDER_TAG = "folder";
        public const string FILE_TAG = "file";
        public const string IDE_FILE_GUID = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";
        public const string IDE_FOLDER_GUID = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";
        public const string NEW_TEMPLATE_FILENAME = "new-template.txt";
        public const string LAYOUT_FILE_FILTER = "Xml files|*.xml";


        public static readonly string[] ReservedTagsXml = { ROOT_TAG, FOLDER_TAG, FILE_TAG };

        public static readonly Dictionary<XmlAttributeName, string> Attributes = new Dictionary<XmlAttributeName, string>
        {
            { XmlAttributeName.TemplateId, "template-id" },
            { XmlAttributeName.Include, "include" },
            { XmlAttributeName.Name, "name" },
            {XmlAttributeName.XmlVersion, "version" }
        };

        public static Project CurrentProject => IDEService.GetCurrentProject();
        public static string ProjectDirectory => Path.GetDirectoryName(CurrentProject.FullName);
        public static string ExtensionDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string TemplatesDirectory => Path.Combine(ExtensionDirectory, "Templates");
        public static string XmlDirectory => Path.Combine(ExtensionDirectory, "Xml");
        public static string DefaultXmlFile
        {
            get
            {
                return Path.Combine(XmlDirectory, Properties.Settings.Default.DefaultXmlFile);
            }
            set
            {
                // Disallow empty value.
                if (string.IsNullOrEmpty(value))
                    return;

                // Ensure we have xml extension.
                var file = value.ToLower();
                if (Path.GetExtension(file) == ".xml")
                    file = Path.GetFileName(value);
                else
                {
                    if (string.IsNullOrEmpty(Path.GetExtension(file)))
                        file = Path.GetFileName(value) + ".xml";
                    else file = Path.ChangeExtension(file, ".xml");
                }

                Properties.Settings.Default.DefaultXmlFile = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string BaseXmlText => $"<{ROOT_TAG} {Attributes[XmlAttributeName.XmlVersion]}=\"{XmlLayoutVersion}\"></{ROOT_TAG}>";
    }
}