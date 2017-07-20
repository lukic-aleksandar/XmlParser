using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Language.Xml.Factory
{
    public class XmlFactory
    {
        public static XmlDocumentSyntax CreateDocument()
        {
            return SyntaxFactory.XmlDocument(CreateDeclaration(), null, null, null, SyntaxFactory.EofToken);
        }

        public static IXmlElement CreateElement(string name, string prefix = null, string leadingWhitespace = null, string trailingWhitespace = null)
        {
            SyntaxTrivia trailingTrivia = null;
            SyntaxTrivia leadingTrivia = null;

            if (!string.IsNullOrEmpty(leadingWhitespace))
            {
                leadingTrivia = SyntaxFactory.WhitespaceTrivia(leadingWhitespace);
            }

            if (!string.IsNullOrEmpty(trailingWhitespace))
            {
                trailingTrivia = SyntaxFactory.EndOfLineTrivia(trailingWhitespace);
            }

            var ltToken = SyntaxFactory.Token(leadingTrivia, SyntaxKind.LessThanToken, null, "<") as PunctuationSyntax;
            var gtToken = SyntaxFactory.Token(null, SyntaxKind.GreaterThanToken, null, ">") as PunctuationSyntax;
            var ltSlashToken = SyntaxFactory.Token(leadingTrivia, SyntaxKind.LessThanSlashToken, null, "</") as PunctuationSyntax;
            var trailingGtToken = gtToken.AddTrailingTrivia(trailingTrivia);

            XmlNameSyntax elementName = CreateQualifiedName(name, prefix);
            XmlElementStartTagSyntax startTag = SyntaxFactory.XmlElementStartTag(ltToken, elementName, null, gtToken);
            XmlElementEndTagSyntax endTag = SyntaxFactory.XmlElementEndTag(ltSlashToken, elementName, trailingGtToken);

            var xmlElementSyntax = (XmlElementSyntax)SyntaxFactory.XmlElement(startTag, null, endTag);

            return xmlElementSyntax.AsElement;
        }

        public static XmlAttributeSyntax CreateAttribute(string name, string value, string prefix = null)
        {
            XmlNameSyntax attributeName = CreateQualifiedName(name, prefix);

            var pool = new SyntaxListPool();
            var list = pool.Allocate<SyntaxToken>();
            var text = SyntaxFactory.XmlText(list);

            var equals = SyntaxFactory.Token(null, SyntaxKind.EqualsToken, null, "=") as PunctuationSyntax;

            return (XmlAttributeSyntax)SyntaxFactory.XmlAttribute(attributeName, equals, text);
        }

        public static XmlCommentSyntax CreateComment()
        {
            throw new NotImplementedException();
        }

        public static XmlCDataSectionSyntax CreateCDataSection()
        {
            throw new NotImplementedException();
        }

        public static XmlProcessingInstructionSyntax CreateProcessingInstruction()
        {
            throw new NotImplementedException();
        }

        public static XmlDeclarationSyntax CreateDeclaration()
        {
            return null;
        }

        public static XmlTextSyntax CreateText()
        {
            throw new NotImplementedException();
        }

        private static XmlNameSyntax CreateQualifiedName(string name, string prefix)
        {
            XmlNameTokenSyntax nameToken = SyntaxFactory.XmlNameToken(name, null, null);
            XmlPrefixSyntax prefixToken = null;

            if (!string.IsNullOrEmpty(prefix))
            {
                XmlNameTokenSyntax prefixNameToken = SyntaxFactory.XmlNameToken(prefix, null, null);
                var colonToken = SyntaxFactory.Token(null, SyntaxKind.ColonToken, null, ":") as PunctuationSyntax;

                prefixToken = SyntaxFactory.XmlPrefix(prefixNameToken, colonToken);
            }

            return SyntaxFactory.XmlName(prefixToken, nameToken);
        }
    }
}
