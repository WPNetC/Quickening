using EnvDTE;
using Quickening.Services;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Quickening.Globals
{
    /// <summary>
    /// This class contains a combination of immutable and mutable strings.
    /// These strings denote common properties and values.
    /// <para>
    /// DEVNOTE:
    /// These should also provide standardisation, convinience and comparison bases for may processes and as such should be a cannon source of essential string vaules.
    /// </para>
    /// </summary>
    internal static class Strings
    {
        #region Immutable
        /// <summary>
        /// Current version of the XML scheme.
        /// </summary>
        public const int XmlLayoutVersion = 3;

        /// <summary>
        /// Current tag for the root element.
        /// </summary>
        public const string ROOT_TAG = "root";
        /// <summary>
        /// Current tag for folder elements.
        /// </summary>
        public const string FOLDER_TAG = "folder";
        /// <summary>
        /// Current tag for file elements.
        /// </summary>
        public const string FILE_TAG = "file";

        /// <summary>
        /// Placeholder key used in place of a filename in templates.
        /// </summary>
        public const string FILENAME_KEY = "{_[filename]_}";
        /// <summary>
        /// Placeholder key used in place of angular module name.
        /// </summary>
        public const string NG_MODULE_KEY = "{_[ngModule]_}";
        public const string NG_CONTROLLER_KEY = "{_[ngController]_}";
        public const string NG_SERVICE_KEY = "{_[ngService]_}";
        public const string JS_FUNCTION_KEY = "{_[jsFunction]_}";

        /// <summary>
        /// Placeholder key used in place of javascript function name.
        /// </summary>
        public const string JS_FUNCTION_NAME_KEY = "{ _[function]_}";

        /// <summary>
        /// VSs' guid for files when in a ProjectItems collection.
        /// </summary>
        public const string IDE_FILE_GUID = "{6BB5F8EE-4483-11D3-8BCF-00C04F8EC28C}";
        /// <summary>
        /// VSs' guid for folders when in a ProjectItems collection.
        /// </summary>
        public const string IDE_FOLDER_GUID = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";


        /// <summary>
        /// Current filename of the default template file.
        /// <para>This should be excluded from any template lists\searches.</para>
        /// </summary>
        public const string NEW_TEMPLATE_FILENAME = "new-template.txt";
        /// <summary>
        /// Filter of available extensions for use when (im\ex)porting layout files.
        /// </summary>
        public const string LAYOUT_FILE_FILTER = "Xml files|*.xml";


        /// <summary>
        /// Array of resrved tags for the currently supported XML schemas.
        /// </summary>
        public static readonly string[] ReservedTagsXml = { ROOT_TAG, FOLDER_TAG, FILE_TAG };
        /// <summary>
        /// Enum to string list of all XML attributes.
        /// <para>This should be used in place of hard-coded values to imporove maintainability.</para>
        /// </summary>
        public static readonly Dictionary<XmlAttributeName, string> Attributes = new Dictionary<XmlAttributeName, string>
        {
            { XmlAttributeName.TemplateId, "template-id" },
            { XmlAttributeName.Include, "include" },
            { XmlAttributeName.Name, "name" },
            { XmlAttributeName.XmlVersion, "version" }
        };


        /// <summary>
        /// Returns the currently selected project.
        /// </summary>
        public static Project CurrentProject => IDEService.GetCurrentProject();


        /// <summary>
        /// The path to the root folder of the currently selected project.
        /// </summary>
        public static string ProjectDirectory => Path.GetDirectoryName(CurrentProject.FullName);
        /// <summary>
        /// The folder extension is installed to.
        /// </summary>
        public static string ExtensionDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        /// <summary>
        /// The folder the file templates are read from.
        /// </summary>
        public static string QuickBlocksDirectory => Path.Combine(ExtensionDirectory, "QuickBlocks");
        /// <summary>
        /// The folder the layout structures are read from.
        /// </summary>
        public static string LayoutsDirectory => Path.Combine(QuickBlocksDirectory, "Layouts");
        /// <summary>
        /// The folder the file templates are read from.
        /// </summary>
        public static string TemplatesDirectory => Path.Combine(QuickBlocksDirectory, "Templates");
        /// <summary>
        /// The folder the file templates are read from.
        /// </summary>
        public static string AngularDirectory => Path.Combine(QuickBlocksDirectory, "Angular");


        /// <summary>
        /// Returns the current base XML structure.
        /// </summary>
        public static string BaseXmlText => $"<{ROOT_TAG} {Attributes[XmlAttributeName.XmlVersion]}=\"{XmlLayoutVersion}\"></{ROOT_TAG}>";
        #endregion

        #region Mutable
        /// <summary>
        /// Gets or sets the current default XML layout file.
        /// <para>This is the file used by the 'Add Web Defaults' command.</para>
        /// </summary>
        public static string DefaultXmlFile
        {
            get
            {
                return Path.Combine(LayoutsDirectory, Properties.Settings.Default.DefaultXmlFile);
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
        #endregion
    }
}