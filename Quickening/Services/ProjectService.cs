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
        public const string RootTag = "__root__";
        public static readonly string[] ReservedTagsXml = { "__root__", "__file__", "__files__" };
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
