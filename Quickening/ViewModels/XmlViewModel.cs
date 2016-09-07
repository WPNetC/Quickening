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
                    dp.Source = new Uri(@"C:\Projects\Quickening\Quickening\Xml\web-basic-V3.xml");
                    dp.XPath = "/root";
                    _xmlData = dp;
                }
#endif
                return _xmlData;
            }
            private set
            {
                if(value != _xmlData)
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
                if(_selectedNode == null)
                {
                    _selectedNode = XmlData?.Document?.FirstChild;
                }
#endif
                return _selectedNode;
            }
            set
            {
                if(value != _selectedNode)
                {
                    _selectedNode = value;

                    NodeToProperties();

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
                if(value != _canUseTemplate)
                {
                    _canUseTemplate = value;
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
                if(value != _itemType)
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
                if(value != _itemName)
                {
                    _itemName = value;
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
                if(value != _includeInProject)
                {
                    _includeInProject = value;
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
                if(value != _templateId)
                {
                    _templateId = value;
                    OnChanged();
                }
            }
        }

        internal void LoadXml()
        {
            XmlDataProvider dp = new XmlDataProvider();
            dp.Source = new Uri(@"C:\Projects\Quickening\Quickening\Xml\web-basic-V3.xml");
            dp.XPath = "/root";
            XmlData = dp;
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
