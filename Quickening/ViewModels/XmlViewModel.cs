using Quickening.Globals;
using Quickening.ICommands;
using Quickening.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;

namespace Quickening.ViewModels
{
    public class XmlViewModel : ViewModelBase
    {
        private XmlDataProvider _xmlData;
        private ObservableCollection<string> _templates;
        private XmlNode _selectedNode;
        private AttributeSet _nodeAttributes;
        private bool _canUseTemplate;
        private bool _canSetType;
        private bool _canSetName;
        private bool _canSave;
        private bool _canSetInclude;
        private bool _canEditTemplate;
        private ICommand _cmdSaveNode;
        private ICommand _cmdCreateNewTemplate;
        private ICommand _cmdEditTemplate;
        private string _currentDataFile;

        public XmlViewModel()
        {

        }

        public string CurrentDataFile
        {
            get
            {
                if (string.IsNullOrEmpty(_currentDataFile))
                    _currentDataFile = Path.Combine(ProjectService.XmlDirectory, "web-basic-V3.xml");
                return _currentDataFile;
            }
            set
            {
                if (value != _currentDataFile)
                {
                    // Ensure we have the full path to the Xml file.
                    if (value != null && !value.StartsWith(ProjectService.XmlDirectory))
                    {
                        _currentDataFile = Path.Combine(ProjectService.XmlDirectory, Path.GetFileName(value));
                    }
                    else
                        _currentDataFile = value;

                    OnChanged();
                }
            }
        }
        public XmlDataProvider XmlData
        {
            get
            {
#if DEBUG
                if (_xmlData == null)
                {
                    XmlDataProvider dp = new XmlDataProvider();
                    dp.Source = new Uri(CurrentDataFile);
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
                    // Clear any previously selected node.
                    SelectedNode = null;

                    _xmlData = value;
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

        public XmlNode SelectedNode
        {
            get
            {
                return _selectedNode;
            }
            set
            {
                if (value != _selectedNode)
                {
                    _selectedNode = value;

                    if (_selectedNode != null)
                        NodeAttributes = AttributeSet.FromXmlNode(_selectedNode);

                    SetEnabledControls();

                    // Can't save a newly loaded node as no changes made.
                    CanSave = false;

                    OnChanged();
                }
            }
        }
        public AttributeSet NodeAttributes
        {
            get
            {
                return _nodeAttributes;
            }
            private set
            {
                if (value != _nodeAttributes)
                {
                    if (_nodeAttributes != null)
                        _nodeAttributes.PropertyChanged -= NodeAttributes_PropertyChanged;

                    _nodeAttributes = value;

                    CheckCanSave();
                    OnChanged();

                    if (_nodeAttributes != null)
                        _nodeAttributes.PropertyChanged += NodeAttributes_PropertyChanged;
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
        public bool CanEditTemplate
        {
            get
            {
                return _canEditTemplate;
            }
            private set
            {
                if (value != _canEditTemplate)
                {
                    _canEditTemplate = value;
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
        public ICommand cmdSaveNode
        {
            get
            {
                if (_cmdSaveNode == null)
                    _cmdSaveNode = new SaveNode(this);
                return _cmdSaveNode;
            }
        }
        public ICommand cmdCreateNewTemplate
        {
            get
            {
                if (_cmdCreateNewTemplate == null)
                    _cmdCreateNewTemplate = new CreateNewTemplate(this);
                return _cmdCreateNewTemplate;
            }
        }
        public ICommand cmdEditTemplate
        {
            get
            {
                if (_cmdEditTemplate == null)
                    _cmdEditTemplate = new EditTemplate(this);
                return _cmdEditTemplate;
            }
        }

        internal void SaveNode()
        {
            PropertiesToNode();
            CanSave = false;
        }
        internal void CreateNewTemplate()
        {

        }
        internal void EditTemplate()
        {

        }

        /// <summary>
        /// Load a new XML file into the tree.
        /// </summary>
        /// <param name="xmlFileName"></param>
        internal void LoadXml()
        {
            if (string.IsNullOrEmpty(CurrentDataFile))
                return;

            try
            {
                XmlDataProvider dp = new XmlDataProvider();
                dp.Source = new Uri(CurrentDataFile);
                dp.XPath = "/root";
                XmlData = dp;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Error");
            }
        }

        /// <summary>
        /// Update the list of available templates.
        /// </summary>
        private void UpdateTemplateList()
        {
            var dInf = new DirectoryInfo(ProjectService.TemplatesDirectory);
            Templates = new ObservableCollection<string>(dInf.GetFiles().Select(p => p.Name.Replace(p.Extension, "")));
        }

        /// <summary>
        /// Sets the properties that are linked to the view based on the selected node.
        /// </summary>
        private void PropertiesToNode()
        {
            if (SelectedNode == null ||
                NodeAttributes == null ||
                string.IsNullOrEmpty(NodeAttributes.ProjectItemName))
                return;

            // Take references to avoid chance of change.
            var attrs = NodeAttributes;
            var node = SelectedNode;
            var doc = node.OwnerDocument;

            // Check if node types match, and if not create a new node.
            if (attrs.NodeType.ToString().ToLower() != SelectedNode.Name?.ToLower())
            {
                // Create new node.
                var newNode = doc.CreateNode(XmlNodeType.Element, attrs.NodeType.ToString().ToLower(), SelectedNode.NamespaceURI);

                // Move any child nodes over.
                var futureOrphans = node.ChildNodes.GetEnumerator();
                while (futureOrphans.MoveNext())
                {
                    var orphan = futureOrphans.Current as XmlNode;
                    if (orphan == null)
                        continue;

                    node.RemoveChild(orphan);
                    newNode.AppendChild(orphan);
                }

                // Replace old node in parent.
                node.ParentNode.ReplaceChild(newNode, node);
                node = newNode;
            }

            // Set node name.
            ((XmlElement)node).SetAttribute(ProjectService.Attributes[XmlAttributeName.Name], attrs.ProjectItemName);

            // Set include property.
            ((XmlElement)node).SetAttribute(ProjectService.Attributes[XmlAttributeName.Include], attrs.Include.ToString().ToLower());

            // Set or remove template id.
            if (!string.IsNullOrEmpty(attrs.TemplateId))
                ((XmlElement)node).SetAttribute(ProjectService.Attributes[XmlAttributeName.TemplateId], attrs.TemplateId);
            else if (node.Attributes[ProjectService.Attributes[XmlAttributeName.TemplateId]]?.Value != null)
            {
                ((XmlElement)node).RemoveAttribute(ProjectService.Attributes[XmlAttributeName.TemplateId]);
            }

            doc.Save(CurrentDataFile);
            SelectedNode = node;
        }
        /// <summary>
        /// Sets the controls that are enabled on the view based on the currently selected node.
        /// </summary>
        private void SetEnabledControls()
        {
            // Set all to false initially, then we just enable the ones we want.
            CanSetType =
                CanSetName =
                CanSetInclude =
                CanUseTemplate =
                CanEditTemplate = false;

            if (SelectedNode == null || SelectedNode.Name.ToLower() == "root") // This will only work with V3 XML format.
                return;

            // If we are not at root we can always set these values.
            CanSetName = CanSetInclude = true;

            switch (NodeAttributes.NodeType)
            {
                case ProjectItemType.File:
                    CanSetType = true;
                    CanUseTemplate = true;
                    CanEditTemplate = !string.IsNullOrEmpty(NodeAttributes.TemplateId); // Can edit template only if there is a template to edit.
                    break;
                case ProjectItemType.Folder:
                    CanSetType = SelectedNode.ChildNodes.Count == 0; // Only allow to change type if no child objects as files cannot have children. (newtered)
                    NodeAttributes.TemplateId = null; // Clear any template that might be set.
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Check if any properties have changed and if so allow saving.
        /// </summary>
        private void CheckCanSave()
        {
            // If we have no node, or no attribute set, or are at root we cannot save.
            if (SelectedNode == null || SelectedNode?.Name == "root" || NodeAttributes == null)
            {
                CanSave = false;
                return;
            }

            // Check each property and if any have changed we can instantly set CanSave to true and return.
            ProjectItemType itemType;
            if (Enum.TryParse(SelectedNode?.Name, true, out itemType))
            {
                if (NodeAttributes.NodeType != itemType)
                {
                    CanSave = true;
                    return;
                }
            }

            if (NodeAttributes.ProjectItemName != SelectedNode?.Attributes[ProjectService.Attributes[XmlAttributeName.Name]]?.Value)
            {
                CanSave = true;
                return;
            }

            if (NodeAttributes.TemplateId != SelectedNode?.Attributes[ProjectService.Attributes[XmlAttributeName.TemplateId]]?.Value)
            {
                CanSave = true;
                return;
            }

            // If we can't parse the value it should be because the attribute doesn't exist.
            // In this case we default to true.
            bool include;
            if (!Boolean.TryParse(SelectedNode?.Attributes[ProjectService.Attributes[XmlAttributeName.Include]]?.Value, out include))
                include = true;

            // As this is the final check we can just set the value to the result.
            CanSave = include != NodeAttributes.Include;
        }
        /// <summary>
        /// Handles when a property of the node attributes changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NodeAttributes_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CheckCanSave();
            SetEnabledControls();
        }

    }
}
