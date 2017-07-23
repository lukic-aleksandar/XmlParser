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

        protected abstract IEnumerable<XmlNodeSyntax> SyntaxNodes { get; set; }

        public abstract SyntaxNode Content { get; set; }

        public abstract void AppendChild(XmlNodeSyntax node);
        public abstract void InsertChildAt(XmlNodeSyntax node, int position);
        public abstract void RemoveChildAt(int position);
        public abstract void RemoveChild(XmlNodeSyntax node);
      
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

        public IEnumerable<XmlNodeSyntax> Children
        {
            get { return SyntaxNodes; }
            set { SyntaxNodes = value; }
        }

        public virtual IEnumerable<XmlAttributeSyntax> Attributes
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
                var attributeSyntaxList = pool.Allocate<XmlAttributeSyntax>();
                
                foreach (var attr in value)
                { 
                    attributeSyntaxList.Add(attr);
                }

                AttributesNode = attributeSyntaxList.ToList().Node;
            }
        }

        public string Value
        {
            get { return Content?.ToFullString() ?? ""; }
            set { Content = Parser.ParseText(value); }
        }

        XmlNameSyntax IXmlElementSyntax.Name
        {
            get { return NameNode; }
            set { NameNode = value; }
        }


        public IXmlElementSyntax AsSyntaxElement
        {
            get{ return this; }
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

            set{ Parent = value as SyntaxNode; }
        }

        public IXmlElement AsElement
        {
            get { return this; }
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
                        //attribute.Value = value;
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
                    if (attribute.Name == attributeName)
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
                    if (attribute.Name == attributeName)
                    {
                        //attribute.Value = value;
                        break;
                    }
                }
            }
        }

        public virtual void AddAttribute(XmlAttributeSyntax attribute)
        {
            SyntaxListPool pool = new SyntaxListPool();
            var attributeList = pool.Allocate<XmlAttributeSyntax>();

            if (attribute.Parent is IXmlElement)
            {
                ((IXmlElement)attribute.Parent).RemoveAttribute(attribute);
            }

            foreach (var attrSyntax in Attributes)
            {
                attributeList.Add(attrSyntax);
            }

            attribute.Parent = this;
            attributeList.Add(attribute);

            AttributesNode = attributeList.ToList().Node;
        }

        public virtual void RemoveAttribute(XmlAttributeSyntax attribute)
        {
            SyntaxListPool pool = new SyntaxListPool();
            var attributeList = pool.Allocate<XmlAttributeSyntax>();

            if (this != attribute.Parent)
            {
                throw new ArgumentException("This element does not contain provided attribute");
            }

            foreach (var attrSyntax in Attributes)
            {
                if (attrSyntax != attribute)
                {
                    attributeList.Add(attrSyntax);
                }
            }

            AttributesNode = attributeList.ToList().Node;
        }

        public virtual void RemoveAttribute(string name)
        {

            SyntaxListPool pool = new SyntaxListPool();
            var attributeList = pool.Allocate<XmlAttributeSyntax>();

            foreach (var attrSyntax in Attributes)
            {
                if (attrSyntax.Name != name)
                {
                    attributeList.Add(attrSyntax);
                }
            }

            AttributesNode = attributeList.ToList().Node;
        }

        internal bool AncestorNode(XmlNodeSyntax node)
        {
            for (var parentNode = this.Parent; parentNode != null && parentNode != this; parentNode = parentNode.Parent)
            {
                if (parentNode == node)
                    return true;
            }
            return false;
        }
    }
}
