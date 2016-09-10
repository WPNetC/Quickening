using EnvDTE;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Quickening.Services
{
    public enum XmlAttributeName
    {
        Include,
        TemplateId
    }

    internal static class ProjectService
    {
        public const string ROOT_TAG = "__root__";
        public const string IDE_FILE_GUID = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";
        public const string IDE_FOLDER_GUID = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";

        public static readonly string[] ReservedTagsXml = { "__root__", "__file__", "__files__", "root", "folder", "file" };
        public static readonly Dictionary<XmlAttributeName, string> Attributes = new Dictionary<XmlAttributeName, string>
        {
            { XmlAttributeName.TemplateId, "template-id" },
            { XmlAttributeName.Include, "include" }
        };

        public static Project CurrentProject => IDEService.GetCurrentProject();
        public static string ProjectDirectory => Path.GetDirectoryName(CurrentProject.FullName);
        public static string ExtensionDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string TemplatesDirectory => Path.Combine(ExtensionDirectory, "Templates");
        public static string XmlDirectory => Path.Combine(ExtensionDirectory, "Xml");
    }
}
