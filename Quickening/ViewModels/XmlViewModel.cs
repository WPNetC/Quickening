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
    }
}
