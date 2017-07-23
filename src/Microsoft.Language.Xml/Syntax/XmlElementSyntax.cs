using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Language.Xml
{
    public class XmlElementSyntax : XmlElementSyntaxBase
    {
        public XmlElementStartTagSyntax StartTag { get; set; }
        public XmlElementEndTagSyntax EndTag { get; set; }
        public override SyntaxNode Content { get; set; }

        public XmlElementSyntax(XmlElementStartTagSyntax start, SyntaxNode content, XmlElementEndTagSyntax end) : 
            base(SyntaxKind.XmlElement, start?.NameNode, start?.Attributes)
        {
            StartTag = start;
            Content = content;
            EndTag = end;
            SlotCount = 3;
        }

        public override SyntaxNode GetSlot(int index)
        {
            switch (index)
            {
                case 0: return StartTag;
                case 1: return Content;
                case 2: return EndTag;
                default:
                    throw new InvalidOperationException();
            }
        }

        public override SyntaxNode Accept(SyntaxVisitor visitor)
        {
            return visitor.VisitXmlElement(this);
        }

        protected override IEnumerable<XmlNodeSyntax> SyntaxNodes
        {
            get
            {
                if (Content is SyntaxList)
                {
                    return ((SyntaxList)Content).ChildNodes.OfType<XmlNodeSyntax>().ToList();
                }
                else if (Content is IXmlElementSyntax)
                {
                    return new XmlNodeSyntax [] { (XmlNodeSyntax)Content };
                }

                return new List<XmlNodeSyntax>();
            }

            set
            {
                SyntaxListPool pool = new SyntaxListPool();
                var syntaxNodeList = pool.Allocate<XmlNodeSyntax>();

                foreach (var element in value)
                {
                    syntaxNodeList.Add(element);
                }

                Content = syntaxNodeList.ToList().Node;
            }
        }

        public override IEnumerable<XmlAttributeSyntax> Attributes
        {
            get { return base.Attributes; }
            set
            {
                base.Attributes = value;
                StartTag.Attributes = base.AttributesNode;
            }
        }

        public override SyntaxNode WithLeadingTrivia(SyntaxNode trivia)
        {
            return new XmlElementSyntax((XmlElementStartTagSyntax)StartTag.WithLeadingTrivia(trivia),
                                        Content,
                                        EndTag);
        }

        public override SyntaxNode WithTrailingTrivia(SyntaxNode trivia)
        {
            return new XmlElementSyntax(StartTag,
                                        Content,
                                        (XmlElementEndTagSyntax)EndTag.WithTrailingTrivia(trivia));
        }

        public override void AppendChild(XmlNodeSyntax node)
        {
            SyntaxListPool pool = new SyntaxListPool();
            var syntaxNodeList = pool.Allocate<XmlNodeSyntax>();

            if (this == node || AncestorNode(node))
            {
                throw new ArgumentException("node is already in the tree");
            }

            if (node.Parent is IXmlElement)
            {
                ((IXmlElement)node.Parent).RemoveChild(node);
            }

            if (Content is SyntaxList)
            {
                var nodes = ((SyntaxList)Content).ChildNodes.OfType<XmlNodeSyntax>();

                foreach (var n in nodes)
                {
                    syntaxNodeList.Add(n);
                }

                syntaxNodeList.Add(node);    
            }
            else if (Content is IXmlElementSyntax)
            {
                syntaxNodeList.Add(Content as XmlNodeSyntax);
                syntaxNodeList.Add(node);
            }
            else if (Content == null)
            {
                syntaxNodeList.Add(node);
            }

            node.Parent = this;
            Content = syntaxNodeList.ToList().Node;
        }

        public override void InsertChildAt(XmlNodeSyntax node, int position)
        {
            SyntaxListPool pool = new SyntaxListPool();
            var syntaxNodeList = pool.Allocate<XmlNodeSyntax>();

            if (this == node || AncestorNode(node))
            {
                throw new ArgumentException("node is already in the tree");
            }

            if (position < 0)
            {
                throw new ArgumentException("position must be a positive number");
            }

            if (Content is SyntaxList)
            {
                var nodes = ((SyntaxList)Content).ChildNodes.OfType<XmlNodeSyntax>();

                if (position > nodes.Count())
                {
                    throw new ArgumentException("position cannot be greather then the number of child nodes");
                }

                bool added = false;

                for (var i = 0; i < nodes.Count(); i++)
                {
                    if (position == i)
                    {
                        syntaxNodeList.Add(node);
                        added = true;
                    }
                    syntaxNodeList.Add(nodes.ElementAt(i));
                }

                if (!added)
                {
                    syntaxNodeList.Add(node);
                }
            }
            else if (Content is IXmlElementSyntax)
            {
                if (position != 0)
                {
                    throw new ArgumentException("Element has only one child node! New node can only be inserted at position 0");
                }

                syntaxNodeList.Add(node);
                syntaxNodeList.Add(Content as XmlNodeSyntax);
            }
            else if (Content == null)
            {
                if (position != 0)
                {
                    throw new ArgumentException("Element has no child nodes! New node can only be inserted at position 0");
                }
                syntaxNodeList.Add(node);
            }

            if (node.Parent is IXmlElement)
            {
                ((IXmlElement)node.Parent).RemoveChild(node);
            }

            node.Parent = this;

            Content = syntaxNodeList.ToList().Node;
        }

        public override void RemoveChildAt(int position)
        {
            SyntaxListPool pool = new SyntaxListPool();
            var syntaxNodeList = pool.Allocate<XmlNodeSyntax>();

            if (position < 0)
            {
                throw new ArgumentException("position must be a positive number");
            }

            if (Content is SyntaxList)
            {
                var nodes = ((SyntaxList)Content).ChildNodes.OfType<XmlNodeSyntax>();

                if (position > nodes.Count())
                {
                    throw new Exception("position cannot be greather then the number of child nodes");
                }

                for (var i = 0; i < nodes.Count(); i++)
                {
                    if (position != i)
                    {
                        syntaxNodeList.Add(nodes.ElementAt(i));
                    }
                }

                Content = syntaxNodeList.ToList().Node;
            }
            else if (Content is IXmlElementSyntax)
            {
                if (position == 0)
                {
                    Content = null;
                }
            }
        }

        public override void RemoveChild(XmlNodeSyntax node)
        {
            SyntaxListPool pool = new SyntaxListPool();
            var syntaxNodeList = pool.Allocate<XmlNodeSyntax>();

            if (this != node.Parent)
            {
                throw new ArgumentException("node is not child of this element");
            }

            if (Content is SyntaxList)
            {
                var nodes = ((SyntaxList)Content).ChildNodes.OfType<XmlNodeSyntax>();

                foreach (var n in nodes)
                {
                    if (node != n)
                    {
                        syntaxNodeList.Add(n);
                    }
                }

                Content = syntaxNodeList.ToList().Node;
            }
            else if (Content is IXmlElementSyntax)
            {
                if (Content == node)
                {
                    Content = null;
                }
            }
        }

        public override void AddAttribute(XmlAttributeSyntax attribute)
        {
            try
            {
                base.AddAttribute(attribute);
                StartTag.Attributes = AttributesNode;
            }
            catch (ArgumentException ae)
            {
                throw ae;
            }
        }

        public override void RemoveAttribute(XmlAttributeSyntax attribute)
        {
            try
            {
                base.RemoveAttribute(attribute);
                StartTag.Attributes = AttributesNode;
            }
            catch (ArgumentException ae)
            {
                throw ae;
            }
        }

        public override void RemoveAttribute(string name)
        {
            base.RemoveAttribute(name);
            StartTag.Attributes = AttributesNode;
        }
    }
}
