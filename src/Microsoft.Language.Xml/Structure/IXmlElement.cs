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
        IEnumerable<IXmlElement> Elements { get; set; }
        IEnumerable<KeyValuePair<string, string>> Attributes { get; set; }
        string this[string attributeName] { get; set; }
        IXmlElementSyntax AsSyntaxElement { get; }
    }
}
