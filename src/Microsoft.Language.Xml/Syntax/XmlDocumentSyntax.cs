using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Language.Xml
{
    public class XmlDocumentSyntax : XmlNodeSyntax, IXmlElement
    {
        public XmlNodeSyntax Body { get; private set; }
        public SyntaxNode PrecedingMisc { get; private set; }
        public SyntaxNode FollowingMisc { get; private set; }
        public XmlDeclarationSyntax Prologue { get; private set; }
        public SyntaxToken Eof { get; set; }

        public XmlDocumentSyntax(
            SyntaxKind kind,
            XmlDeclarationSyntax prologue,
            SyntaxNode precedingMisc,
            XmlNodeSyntax body,
            SyntaxNode followingMisc,
            SyntaxToken eof) : base(kind)
        {
            this.Prologue = prologue;
            this.PrecedingMisc = precedingMisc;
            this.Body = body;
            this.FollowingMisc = followingMisc;
            this.Eof = eof;
            SlotCount = 5;
        }

        public IXmlElement Root
        {
            get
            {
                return Body as IXmlElement;
            }

            set { Body = value as XmlNodeSyntax; }
        }

        public IXmlElementSyntax RootSyntax
        {
            get
            {
                return Body as IXmlElementSyntax;
            }
        }

        public string Name
        {
            get
            {
                if (Root == null)
                {
                    return null;
                }

                return Root.Name;
            }

            set
            {
                if (Root != null)
                {
                    Root.Name = value;
                }
            }
        }

        IXmlElement IXmlElement.Parent
        {
            get
            {
                return null;
            }
            set { }
        }

        public IEnumerable<XmlNodeSyntax> Children
        {
            get
            {
                if (Root == null)
                {
                    return null;
                }

                return Root.Children;
            }

            set
            {
                if (Root != null)
                {
                    Root.Children = value;
                }
            }
        }

        public IEnumerable<XmlAttributeSyntax> Attributes
        {
            get
            {
                if (Root == null)
                {
                    return null;
                }

                return Root.Attributes;
            }

            set
            {
                if (Root != null)
                {
                    Root.Attributes = value;
                }
            }
        }

        public string Value
        {
            get
            {
                if (Root == null)
                {
                    return null;
                }

                return Root.Value;
            }

            set
            {
                if (Root != null)
                {
                    Root.Value = value;
                }
            }
        }

        public IXmlElementSyntax AsSyntaxElement
        {
            get
            {
                return Root as IXmlElementSyntax;
            }
        }

        public string this[string attributeName]
        {
            get
            {
                if (Root == null)
                {
                    return null;
                }

                return Root[attributeName];
            }

            set
            {
                if (Root != null)
                {
                    Root[attributeName] = value;
                }
            }
        }

        public override SyntaxNode GetSlot(int index)
        {
            switch (index)
            {
                case 0:
                    return Prologue;
                case 1:
                    return PrecedingMisc;
                case 2:
                    return Body;
                case 3:
                    return FollowingMisc;
                case 4:
                    return Eof;
                default:
                    throw null;
            }
        }

        public override SyntaxNode Accept(SyntaxVisitor visitor)
        {
            return visitor.VisitXmlDocument(this);
        }

        public void AppendChild(XmlNodeSyntax node)
        {
            throw new NotSupportedException();
        }

        public void InsertChildAt(XmlNodeSyntax node, int position)
        {
            throw new NotSupportedException();
        }

        public void RemoveChildAt(int position)
        {
            throw new NotSupportedException();
        }

        public void RemoveChild(XmlNodeSyntax node)
        {
            if (Root == node)
            {
                Root = null;
            }
        }

        public void AddAttribute(XmlAttributeSyntax attribute)
        {
            throw new NotSupportedException();
        }

        public void RemoveAttribute(XmlAttributeSyntax attribute)
        {
            throw new NotSupportedException();
        }

        public void RemoveAttribute(string name)
        {
            throw new NotSupportedException();
        }
    }
}
