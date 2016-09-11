using Quickening.Globals;
using Quickening.ICommands;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;

namespace Quickening.ViewModels
{
    public class XmlViewModel : ViewModelBase
    {
        #region Private Fields

        private XmlDataProvider _xmlData;
        private ObservableCollection<string> _templates;
        private XmlNode _selectedNode;
        private AttributeSet _nodeAttributes;
        private bool _canUseTemplate, _canSetType, _canSetName, _canSave, _canSetInclude, _canEditTemplate;
        private string _currentDataFile;
        private ICommand _cmdSaveNode, _cmdCreateNewTemplate, _cmdEditTemplate, _cmdAddItem, _cmdRemoveItem;

        #endregion Private Fields

        public XmlViewModel()
        {
        }

        public string CurrentDataFile
        {
            get
            {
                return _currentDataFile;
            }
            set
            {
                if (value != _currentDataFile)
                {
                    // Ensure we have the full and correct path to the Xml file.
                    if (value != null)
                    {
                        if (!value.StartsWith(Strings.XmlDirectory))
                            _currentDataFile = Path.Combine(Strings.XmlDirectory, Path.GetFileName(value));

                        if (Path.GetExtension(_currentDataFile).ToLower() != ".xml")
                            _currentDataFile = Path.ChangeExtension(_currentDataFile, ".xml");
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
                if (_xmlData == null)
                    LoadXml(CurrentDataFile, false);

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
                    var dInf = new DirectoryInfo(Strings.TemplatesDirectory);
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

        public bool CanAddItem { get; internal set; }

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

        public ICommand cmdAddItem
        {
            get
            {
                if (_cmdAddItem == null)
                    _cmdAddItem = new AddItem(this);
                return _cmdAddItem;
            }
        }

        public ICommand cmdRemoveItem
        {
            get
            {
                if (_cmdRemoveItem == null)
                    _cmdRemoveItem = new RemoveItem(this);
                return _cmdRemoveItem;
            }
        }

        internal void SaveNode()
        {
            PropertiesToNode();
            CanSave = false;
        }

        internal void CreateNewTemplate()
        {
            var path = Path.Combine(Strings.TemplatesDirectory, "new-template.txt");
            if (!File.Exists(path))
                File.WriteAllText(path, "Enter contents...");

            // Launch new file in notepad (doesn't seem to work in VS)
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo("notepad.exe", $"{path}");
            process.StartInfo = startInfo;
            process.Start();
        }

        internal void EditTemplate()
        {
            if (string.IsNullOrEmpty(NodeAttributes.TemplateId))
                return;

            var path = Path.Combine(Strings.TemplatesDirectory, $"{NodeAttributes.TemplateId}.txt");
            if (!File.Exists(path))
                return;

            // Launch new file in notepad (doesn't seem to work in VS)
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo("notepad.exe", $"{path}");
            process.StartInfo = startInfo;
            process.Start();
        }

        /// <summary>
        /// Adds an item to the current XML tree, underneath the currently selected node.
        /// <para>NOTE: This method makes many presumptions upon the Views' structure and format.
        /// As such any changes to the View will likely break this method.</para>
        /// </summary>
        /// <param name="parameter"></param>
        internal void AddItem(object parameter)
        {
            // Parametere should be the button that launched the command.
            var btn = parameter as Button;

            // Check we have both a button and a node to use.
            if (btn == null || SelectedNode == null)
                return;

            // As such it's parent should be a stackpanel.
            var parent = LogicalTreeHelper.GetParent(btn) as StackPanel;
            if (parent != null)
            {
                // There should only be 1 textbox control.
                var textBox = parent.Children.OfType<TextBox>().First();

                // If no text, exit here. We cannot create an element with no name.
                if (string.IsNullOrEmpty(textBox?.Text))
                    return;

                // Name of control should contain type of item to add.
                ProjectItemType itemType;
                if (textBox.Name.ToLower().Contains("folder"))
                    itemType = ProjectItemType.Folder;
                else if (textBox.Name.ToLower().Contains("file"))
                    itemType = ProjectItemType.File;
                else return; // We should show an error here?

                // Create new node.
                var newNode = SelectedNode.OwnerDocument.CreateNode(XmlNodeType.Element, itemType.ToString().ToLower(), SelectedNode.NamespaceURI);
                // Set node name.
                ((XmlElement)newNode).SetAttribute(Strings.Attributes[XmlAttributeName.Name], textBox.Text);
                // Append new node to selected node.
                SelectedNode.AppendChild(newNode);
                // Save XML file.
                SelectedNode.OwnerDocument.Save(CurrentDataFile);
                // Reload XML file. (Seems to help prevent errors)
                LoadXml(CurrentDataFile);
                // Select the new node for editing.
                SelectedNode = newNode;
                // Clear text box for next input.
                textBox.Text = "";
            }

            // Close context menu
            var upLevel = LogicalTreeHelper.GetParent(parent);
            ContextMenu menu = upLevel as ContextMenu;
            while (menu == null && upLevel != null)
            {
                upLevel = LogicalTreeHelper.GetParent(upLevel);
                menu = upLevel as ContextMenu;
            }
            if (menu != null)
            {
                menu.IsOpen = false;
            }
        }

        internal void RemoveItem()
        {
            if (SelectedNode == null)
                return;

            // Prevent removing root element and if attempted show an error.
            if (SelectedNode.Name == Strings.ROOT_TAG)
            {
                MessageBox.Show("Cannot delete root element.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create message based on if there are subitems or not.
            StringBuilder sb = new StringBuilder();
            string title = "";
            System.Windows.Forms.MessageBoxIcon icon;
            if (SelectedNode.ChildNodes.Count > 0)
            {
                sb.AppendLine("This folder is not empty.");
                sb.AppendLine();
                sb.AppendLine("Deleting this will also delete all sub-items.");
                sb.AppendLine("Are you sure you wish to delete this folder?");

                title = "Folder not empty.";
                icon = System.Windows.Forms.MessageBoxIcon.Warning;
            }
            else
            {
                sb.AppendLine("Are you sure?");

                title = "Delete object.";
                icon = System.Windows.Forms.MessageBoxIcon.Question;
            }

            // Check before deleting using previous message settings.
            var dr = System.Windows.Forms.MessageBox.Show(
                sb.ToString(),
                title,
                System.Windows.Forms.MessageBoxButtons.YesNo,
                icon);

            if (dr != System.Windows.Forms.DialogResult.Yes)
                return;

            try
            {
                // Remove from either parent or document.
                if (SelectedNode.ParentNode != null)
                    SelectedNode.ParentNode.RemoveChild(SelectedNode);
                else
                    SelectedNode.OwnerDocument.RemoveChild(SelectedNode);

                // Save updated XML to file.
                if (!String.IsNullOrEmpty(CurrentDataFile))
                    SelectedNode.OwnerDocument.Save(CurrentDataFile);
                else
                {
                    MessageBox.Show("Could not find current XML file. Please deselect then reselect to try to reload.");
                    return;
                }
                // Reload XML file. (Seems to help prevent errors)
                LoadXml(CurrentDataFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}{Environment.NewLine}Could not remove node.{Environment.NewLine}Try to reload the file and try again.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Load a new XML file into the tree.
        /// </summary>
        /// <param name="xmlFileName"></param>
        public void LoadXml(string fileName, bool updateUI = true)
        {
            CurrentDataFile = fileName;

            if (string.IsNullOrEmpty(CurrentDataFile))
                return;

            try
            {
                XmlDataProvider dp = new XmlDataProvider();
                dp.Source = new Uri(CurrentDataFile);
                dp.XPath = $"/{Strings.ROOT_TAG}";

                if (updateUI)
                {
                    // Clear selected node.
                    SelectedNode = null;

                    XmlData = dp;
                }
                else
                    _xmlData = dp;
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
            var dInf = new DirectoryInfo(Strings.TemplatesDirectory);
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
            ((XmlElement)node).SetAttribute(Strings.Attributes[XmlAttributeName.Name], attrs.ProjectItemName);

            // Set include property.
            ((XmlElement)node).SetAttribute(Strings.Attributes[XmlAttributeName.Include], attrs.Include.ToString().ToLower());

            // Set or remove template id.
            if (!string.IsNullOrEmpty(attrs.TemplateId))
                ((XmlElement)node).SetAttribute(Strings.Attributes[XmlAttributeName.TemplateId], attrs.TemplateId);
            else if (node.Attributes[Strings.Attributes[XmlAttributeName.TemplateId]]?.Value != null)
            {
                ((XmlElement)node).RemoveAttribute(Strings.Attributes[XmlAttributeName.TemplateId]);
            }

            // Save new XML file and update selected node.
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

            if (NodeAttributes.ProjectItemName != SelectedNode?.Attributes[Strings.Attributes[XmlAttributeName.Name]]?.Value)
            {
                CanSave = true;
                return;
            }

            if (NodeAttributes.TemplateId != SelectedNode?.Attributes[Strings.Attributes[XmlAttributeName.TemplateId]]?.Value)
            {
                CanSave = true;
                return;
            }

            // If we can't parse the value it should be because the attribute doesn't exist.
            // In this case we default to true.
            bool include;
            if (!Boolean.TryParse(SelectedNode?.Attributes[Strings.Attributes[XmlAttributeName.Include]]?.Value, out include))
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