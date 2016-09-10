using EnvDTE;
using Quickening.Services;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Quickening.Globals
{
    internal static class Strings
    {
        public const string ROOT_TAG = "root";
        public const string FOLDER_TAG = "folder";
        public const string FILE_TAG = "file";
        public const string IDE_FILE_GUID = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";
        public const string IDE_FOLDER_GUID = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";

        public static readonly string[] ReservedTagsXml = { ROOT_TAG, FOLDER_TAG, FILE_TAG };
        public static readonly Dictionary<XmlAttributeName, string> Attributes = new Dictionary<XmlAttributeName, string>
        {
            { XmlAttributeName.TemplateId, "template-id" },
            { XmlAttributeName.Include, "include" },
            { XmlAttributeName.Name, "name" }
        };

        public static Project CurrentProject => IDEService.GetCurrentProject();
        public static string ProjectDirectory => Path.GetDirectoryName(CurrentProject.FullName);
        public static string ExtensionDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string TemplatesDirectory => Path.Combine(ExtensionDirectory, "Templates");
        public static string XmlDirectory => Path.Combine(ExtensionDirectory, "Xml");
    }
}
