using System.Collections.Generic;

namespace Microsoft.Language.Xml
{
    public interface IXmlElementSyntax
    {
        XmlNameSyntax Name { get; set; }
        SyntaxNode Content { get; set; }
        IXmlElementSyntax Parent { get; set; }
        IEnumerable<IXmlElementSyntax> Elements { get; set; }
        IEnumerable<XmlAttributeSyntax> Attributes { get; set; }
        XmlAttributeSyntax this[string attributeName] { get; set; }
        IXmlElementSyntax AsSyntaxElement { get; }
        IXmlElement AsElement { get; }
    }
}
