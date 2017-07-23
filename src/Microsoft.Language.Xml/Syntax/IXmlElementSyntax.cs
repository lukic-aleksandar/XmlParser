using System.Collections.Generic;

namespace Microsoft.Language.Xml
{
    public interface IXmlElementSyntax
    {
        XmlNameSyntax Name { get; set; }
        SyntaxNode Content { get; set; }
        IXmlElementSyntax Parent { get; set; }
        IEnumerable<XmlNodeSyntax> Children { get; set; }
        IEnumerable<XmlAttributeSyntax> Attributes { get; set; }
        XmlAttributeSyntax this[string attributeName] { get; set; }
        IXmlElementSyntax AsSyntaxElement { get; }
        IXmlElement AsElement { get; }

        void AppendChild(XmlNodeSyntax node);
        void InsertChildAt(XmlNodeSyntax node, int position);
        void RemoveChildAt(int position);
        void RemoveChild(XmlNodeSyntax node);
        void AddAttribute(XmlAttributeSyntax attribute);
        void RemoveAttribute(XmlAttributeSyntax attribute);
        void RemoveAttribute(string name);
    }
}
