using Quickening.Globals;
using Quickening.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Xml;

namespace Quickening.ViewModels
{
    public class XmlViewModel : ViewModelBase
    {
        private XmlNode _selectedNode;
        private XmlDataProvider _xmlData;
        private bool _canUseTemplate;
        private ProjectItemType _itemType;
        private string _itemName;
        private bool _includeInProject;
        private string _templateId;
        private ObservableCollection<string> _templates;
        private bool _canSetType;
        private bool _canSetName;
        private bool _canSave;
        private bool _canSetInclude;

        public XmlViewModel()
        {

        }

        public XmlDataProvider XmlData
        {
            get
            {
#if DEBUG
                if (_xmlData == null)
                {
                    XmlDataProvider dp = new XmlDataProvider();
                    var path = Path.Combine(ProjectService.XmlDirectory, "web-basic-V3.xml");
                    dp.Source = new Uri(path);
                    dp.XPath = "/root";
                    _xmlData = dp;
                }
#endif
                return _xmlData;
            }
            private set
            {
                if (value != _xmlData)
                {
                    _xmlData = value;
                    OnChanged();
                }
            }
        }
        public XmlNode SelectedNode
        {
            get
            {
#if DEBUG
                if (_selectedNode == null)
                {
                    _selectedNode = XmlData?.Document?.FirstChild;
                }
#endif
                return _selectedNode;
            }
            set
            {
                if (value != _selectedNode)
                {
                    _selectedNode = value;

                    NodeToProperties();

                    OnChanged();
                }
            }
        }
        public ObservableCollection<string> Templates
        {
            get
            {
                if (_templates == null)
                {
                    var dInf = new DirectoryInfo(ProjectService.TemplatesDirectory);
                    _templates = new ObservableCollection<string>(dInf.GetFiles().Select(p => p.Name.Replace(p.Extension, "")));
                }
                return _templates;
            }
            private set
            {
                if (value != _templates)
                {
                    _templates = value;
                    OnChanged();
                }
            }
        }

        public bool CanSetType
        {
            get
            {
                return _canSetType;
            }
            private set
            {
                if (value != _canSetType)
                {
                    _canSetType = value;
                    OnChanged();
                }
            }
        }
        public bool CanSetName
        {
            get
            {
                return _canSetName;
            }
            private set
            {
                if (value != _canSetName)
                {
                    _canSetName = value;
                    OnChanged();
                }
            }
        }
        public bool CanSetInclude
        {
            get
            {
                return _canSetInclude;
            }
            private set
            {
                if (value != _canSetInclude)
                {
                    _canSetInclude = value;
                    OnChanged();
                }
            }
        }
        public bool CanUseTemplate
        {
            get
            {
                return _canUseTemplate;
            }
            private set
            {
                if (value != _canUseTemplate)
                {
                    _canUseTemplate = value;
                    OnChanged();
                }
            }
        }
        public bool CanSave
        {
            get
            {
                return _canSave;
            }
            private set
            {
                if (value != _canSave)
                {
                    _canSave = value;
                    OnChanged();
                }
            }
        }

        public ProjectItemType ItemType
        {
            get
            {
                return _itemType;
            }
            set
            {
                if (value != _itemType)
                {
                    _itemType = value;
                    OnChanged();
                }
            }
        }
        public string ItemName
        {
            get
            {
                return _itemName;
            }
            set
            {
                if (value != _itemName)
                {
                    _itemName = value;
                    OnChanged();
                }
            }
        }
        public string Template
        {
            get
            {
                return _templateId;
            }
            set
            {
                if (value != _templateId)
                {
                    _templateId = value;
                    OnChanged();
                }
            }
        }
        public bool IncludeInProject
        {
            get
            {
                return _includeInProject;
            }
            set
            {
                if (value != _includeInProject)
                {
                    _includeInProject = value;
                    OnChanged();
                }
            }
        }

        internal void LoadXml(string xmlFileName)
        {
            // Make sure we just have the file name to standardise the process.
            // As this could potentially be null or empty we will use string.Replace instead of Path.GetFileName.
            if (xmlFileName.StartsWith(ProjectService.XmlDirectory))
                xmlFileName = xmlFileName.Replace(ProjectService.XmlDirectory, "");
            
            XmlDataProvider dp = new XmlDataProvider();
            var path = Path.Combine(ProjectService.XmlDirectory, xmlFileName ?? "web-basic-V3.xml");
            dp.Source = new Uri(path);
            dp.XPath = "/root";
            XmlData = dp;
        }
        internal void UpdateTemplateList()
        {
            var dInf = new DirectoryInfo(ProjectService.TemplatesDirectory);
            Templates = new ObservableCollection<string>(dInf.GetFiles().Select(p => p.Name.Replace(p.Extension, "")));
        }
        internal void NodeToProperties()
        {
            ProjectItemType itemType;
            Enum.TryParse(SelectedNode?.Name, true, out itemType);

            bool include;
            if (!Boolean.TryParse(SelectedNode?.Attributes["include"]?.Value, out include))
                include = true;

            ItemType = itemType;
            ItemName = SelectedNode?.Attributes["name"]?.Value;
            IncludeInProject = include;
            Template = SelectedNode?.Attributes["template-id"]?.Value;
        }
    }
}
