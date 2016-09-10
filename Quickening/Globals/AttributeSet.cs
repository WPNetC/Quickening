using Quickening.Globals;
using Quickening.Services;
using System;
using System.ComponentModel;
using System.Xml;

namespace Quickening
{
    /// <summary>
    /// Holds all the attributes that may have been in the original XML.
    /// <para>If the XML format is changed or updated this will need to be updated to match.</para>
    /// </summary>
    public class AttributeSet : INotifyPropertyChanged
    {
        private ProjectItemType _nodeType;
        private string _projectItemName;
        private string _templateId;
        private bool _include;

        private AttributeSet(string name, string itemName, string id, bool include)
        {
            ProjectItemType itemType;
            Enum.TryParse(name, true, out itemType);
            NodeType = itemType;

            ProjectItemName = itemName;
            TemplateId = id;
            Include = include;
        }

        /// <summary>
        /// Create a new AttributerSet directly from an XmlNode.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static AttributeSet FromXmlNode(XmlNode node)
        {
            // Look for 'name' attribute, default to root as this should be only node with no name.
            string name = node.Attributes[Strings.Attributes[XmlAttributeName.Name]]?.Value ?? "root";

            // Look for 'template-id' attribute, default to empty.
            string id = node.Attributes[Strings.Attributes[XmlAttributeName.TemplateId]]?.Value ?? "";

            // Look for 'include' attribute, default to true.
            bool include;
            if (!Boolean.TryParse(node.Attributes[Strings.Attributes[XmlAttributeName.Include]]?.Value, out include))
                include = true;

            // As we want to be able to exclude entire folders, run up the node tree to check if a parent is excluded.
            var parent = node.ParentNode;
            while (include && (parent != null && parent.Name != Strings.ROOT_TAG))
            {
                bool includeParent = true; // Seperate bool to get parent nodes setting.
                if (bool.TryParse(parent.Attributes?[Strings.Attributes[XmlAttributeName.Include]]?.Value, out includeParent))
                    include = includeParent; // Assigning parent value to 'include' will end loop if value is false.

                parent = parent.ParentNode; // Step up XML tree.
            }

            return new AttributeSet(node.Name, name, id, include);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the qualified name of the XML node.
        /// </summary>
        public ProjectItemType NodeType
        {
            get
            {
                return _nodeType;
            }
            set
            {
                if(value != _nodeType)
                {
                    _nodeType = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NodeName"));
                }
            }
        }

        /// <summary>
        /// The of the file or folder.
        /// </summary>
        public string ProjectItemName
        {
            get
            {
                return _projectItemName;
            }
            set
            {
                if (value != _projectItemName)
                {
                    _projectItemName = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ProjectItemName"));
                }
            }
        }

        /// <summary>
        /// Id of the template to use when creating a file.
        /// <pare>Default to empty string.</pare>
        /// </summary>
        public string TemplateId
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TemplateId"));
                }
            }
        }

        /// <summary>
        /// If the file or folder should be included in the solution.
        /// <para>Defaults to true.</para>
        /// </summary>
        public bool Include
        {
            get
            {
                // Can't exclude root folder, as this is the project itself.
                if (NodeType == ProjectItemType.Root)
                    return true;

                return _include;
            }
            set
            {
                if (value != _include)
                {
                    _include = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Include"));
                }
            }
        }
        
    }
}
