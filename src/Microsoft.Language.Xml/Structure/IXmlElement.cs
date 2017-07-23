using System.Collections.Generic;

namespace Microsoft.Language.Xml
{
    public interface IXmlElement
    {
        int Start { get; }
        int FullWidth { get; }
        string Name { get; set; }
        string Value { get; set; }
        IXmlElement Parent { get; set; }
        IEnumerable<XmlNodeSyntax> Children { get; set; }
        IEnumerable<XmlAttributeSyntax> Attributes { get; set; }
        string this[string attributeName] { get; set; }
        IXmlElementSyntax AsSyntaxElement { get; }

        void AppendChild(XmlNodeSyntax node);
        void InsertChildAt(XmlNodeSyntax node, int position);
        void RemoveChildAt(int position);
        void RemoveChild(XmlNodeSyntax node);
        void AddAttribute(XmlAttributeSyntax attribute);
        void RemoveAttribute(XmlAttributeSyntax attribute);
        void RemoveAttribute(string name);
    }
}
