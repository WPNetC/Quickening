using Quickening.Services;
using System;
using System.Xml;

namespace Quickening
{
    /// <summary>
    /// Holds all the attributes that may have been in the original XML.
    /// </summary>
    public struct AttributeSet
    {
        /// <summary>
        /// The name of the XML node.
        /// </summary>
        public string NodeName { get; private set; }

        /// <summary>
        /// Id of the template to use when creating a file.
        /// <pare>Default to empty string.</pare>
        /// </summary>
        public string TemplateId { get; private set; }

        /// <summary>
        /// If the file or folder should be included in the solution.
        /// <para>Defaults to true.</para>
        /// </summary>
        public bool Include { get; private set; }
        
        private AttributeSet(string name, string id, bool include)
        {
            NodeName = name;
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
            // Look for 'template-id' attribute, default to empty.
            string id = node.Attributes[ProjectService.Attributes[XmlAttributeName.TemplateId]]?.Value ?? "";

            // Look for 'include' attribute, default to true.
            bool include;
            if (!Boolean.TryParse(node.Attributes[ProjectService.Attributes[XmlAttributeName.Include]]?.Value, out include))
                include = true;

            // As we want to be able to exclude entire folders, run up the node tree to check if a parent is excluded.
            var parent = node.ParentNode;
            while (include && (parent != null && parent.Name != ProjectService.ROOT_TAG))
            {
                bool includeParent = true; // Seperate bool to get parent nodes setting.
                if (bool.TryParse(parent.Attributes?[ProjectService.Attributes[XmlAttributeName.Include]]?.Value, out includeParent))
                    include = includeParent; // Assigning parent value to 'include' will end loop if value is false.

                parent = parent.ParentNode; // Step up XML tree.
            }

            return new AttributeSet(node.Name, id, include);
        }
    }
}
