using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Language.Xml
{
    public class XmlEmptyElementSyntax : XmlElementSyntaxBase
    {
        public PunctuationSyntax LessThanToken;
        public PunctuationSyntax SlashGreaterThanToken;

        public XmlEmptyElementSyntax(
            PunctuationSyntax lessThanToken,
            XmlNameSyntax name,
            SyntaxNode attributes,
            PunctuationSyntax slashGreaterThanToken) : base(
                SyntaxKind.XmlEmptyElement,
                name, 
                attributes)
        {
            this.LessThanToken = lessThanToken;
            this.NameNode = name;
            this.AttributesNode = attributes;
            this.SlashGreaterThanToken = slashGreaterThanToken;
            this.SlotCount = 4;
        }

        public override SyntaxNode GetSlot(int index)
        {
            switch (index)
            {
                case 0: return LessThanToken;
                case 1: return NameNode;
                case 2: return AttributesNode;
                case 3: return SlashGreaterThanToken;
                default:
                    throw new InvalidOperationException();
            }
        }

        public override SyntaxNode Accept(SyntaxVisitor visitor)
        {
            return visitor.VisitXmlEmptyElement(this);
        }

        public override SyntaxNode Content
        {
            get
            {
                return null;
            }

            set{}
        }

        public override void AppendChild(XmlNodeSyntax node)
        {
            throw new NotSupportedException();
        }

        public override void InsertChildAt(XmlNodeSyntax node, int position)
        {
            throw new NotSupportedException();
        }

        public override void RemoveChildAt(int position)
        {
            throw new NotSupportedException();
        }

        public override void RemoveChild(XmlNodeSyntax node)
        {
            throw new NotSupportedException();
        }

        protected override IEnumerable<XmlNodeSyntax> SyntaxNodes
        {
            get
            {
                return new List<XmlNodeSyntax>();
            }

            set {}
        }
    }
}
