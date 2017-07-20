using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Language.Xml
{
    public abstract class XmlElementSyntaxBase : XmlNodeSyntax, IXmlElement, IXmlElementSyntax
    {
        public XmlNameSyntax NameNode;
        public SyntaxNode AttributesNode;

        public XmlElementSyntaxBase(
            SyntaxKind syntaxKind,
            XmlNameSyntax name,
            SyntaxNode attributes) : base(syntaxKind)
        {
            this.NameNode = name;
            this.AttributesNode = attributes;
        }

        protected abstract IEnumerable<IXmlElementSyntax> SyntaxElements { get; set; }

        public abstract SyntaxNode Content { get; set; }

        IXmlElement IXmlElement.Parent
        {
            get
            {
                var current = this.Parent;
                while (current != null)
                {
                    if (current is IXmlElement)
                    {
                        return current as IXmlElement;
                    }

                    current = current.Parent;
                }

                return null;
            }

            set { Parent = value as SyntaxNode ?? Parent; }
        }

        public string Name
        {
            get
            {
                if (NameNode == null)
                {
                    return null;
                }

                return NameNode.Name;
            }

            set
            {
                if (NameNode != null)
                {
                    NameNode.Name = value;
                }
            }
        }

        public IEnumerable<IXmlElement> Elements
        {
            get
            {
                return SyntaxElements.Select(el => el.AsElement);
            }

            set { SyntaxElements = value.Select(el => el.AsSyntaxElement); }
        }

        public virtual IEnumerable<KeyValuePair<string, string>> Attributes
        {
            get
            {
                if (AttributesNode == null)
                {
                    yield break;
                }

                var singleAttribute = AttributesNode as XmlAttributeSyntax;
                if (singleAttribute != null)
                {
                    yield return new KeyValuePair<string, string>(singleAttribute.Name, singleAttribute.Value);
                    yield break;
                }

                foreach (var attribute in AttributesNode.ChildNodes.OfType<XmlAttributeSyntax>())
                {
                    yield return new KeyValuePair<string, string>(attribute.Name, attribute.Value);
                }
            }

            set
            {
                SyntaxListPool pool = new SyntaxListPool();
                var attributeSyntaxList = pool.Allocate<XmlAttributeSyntax>();
                
                foreach (var pair in value)
                {
                    var nameToken = SyntaxFactory.XmlNameToken(pair.Key, null, null);
                    var name = SyntaxFactory.XmlName(null, nameToken);
                    var equals = SyntaxFactory.Token(null, SyntaxKind.EqualsToken, null, "=");
                    var equalsPunct = equals as PunctuationSyntax;
                    var doubleQuote = SyntaxFactory.Token(null, SyntaxKind.DoubleQuoteToken, null, "\"");
                    var doubleQuotePunct = doubleQuote as PunctuationSyntax;
                    var textNode = SyntaxFactory.XmlTextLiteralToken(pair.Value, pair.Value, null, null);

                    var textTokens = pool.Allocate<XmlTextTokenSyntax>();
                    textTokens.Add(textNode);
                    var val = SyntaxFactory.XmlString(doubleQuotePunct, textTokens.ToList(), doubleQuotePunct);
                    attributeSyntaxList.Add(SyntaxFactory.XmlAttribute(name, equalsPunct, val) as XmlAttributeSyntax);
                }

                AttributesNode = attributeSyntaxList.ToList().Node;
            }
        }

        public string Value
        {
            get
            {
                return Content?.ToFullString() ?? "";
            }

            set { Content = Parser.ParseText(value); }
        }

        XmlNameSyntax IXmlElementSyntax.Name
        {
            get
            {
                return NameNode;
            }

            set { NameNode = value; }
        }


        public IXmlElementSyntax AsSyntaxElement
        {
            get
            {
                return this;
            }
        }

        IXmlElementSyntax IXmlElementSyntax.Parent
        {
            get
            {
                var current = this.Parent;
                while (current != null)
                {
                    if (current is IXmlElementSyntax)
                    {
                        return current as IXmlElementSyntax;
                    }

                    current = current.Parent;
                }

                return null;
            }

            set { Parent = value as SyntaxNode; }
        }

        IEnumerable<IXmlElementSyntax> IXmlElementSyntax.Elements
        {
            get
            {
                return SyntaxElements;
            }

            set { SyntaxElements = value; }
        }

        IEnumerable<XmlAttributeSyntax> IXmlElementSyntax.Attributes
        {
            get
            {
                if (AttributesNode == null)
                {
                    yield break;
                }

                var singleAttribute = AttributesNode as XmlAttributeSyntax;
                if (singleAttribute != null)
                {
                    yield return singleAttribute;
                    yield break;
                }

                foreach (var attribute in AttributesNode.ChildNodes.OfType<XmlAttributeSyntax>())
                {
                    yield return attribute;
                }
            }

            set
            {
                SyntaxListPool pool = new SyntaxListPool();
                var attributeList = pool.Allocate<XmlAttributeSyntax>();

                foreach (var attrSyntax in value)
                {
                    attributeList.Add(attrSyntax);
                }

                AttributesNode = attributeList.ToList().Node;
            }
        }

        public IXmlElement AsElement
        {
            get
            {
                return this;
            }
        }

        XmlAttributeSyntax IXmlElementSyntax.this[string attributeName]
        {
            get
            {
                foreach (var attribute in AsSyntaxElement.Attributes)
                {
                    if (attribute.Name == attributeName)
                    {
                        return attribute;
                    }
                }

                return null;
            }

            set
            {
                foreach (var attribute in AsSyntaxElement.Attributes)
                {
                    if (attribute.Name == attributeName)
                    {
                       // attribute.Value = value;
                    }
                }
            }
        }

        public string this[string attributeName]
        {
            get
            {
                foreach (var attribute in Attributes)
                {
                    if (attribute.Key == attributeName)
                    {
                        return attribute.Value;
                    }
                }

                return null;
            }
            set
            {
                foreach (var attribute in Attributes)
                {
                    if (attribute.Key == attributeName)
                    {
                        
                        //attribute.Value = value;
                        break;
                    }
                }
            }
        }
    }
}
