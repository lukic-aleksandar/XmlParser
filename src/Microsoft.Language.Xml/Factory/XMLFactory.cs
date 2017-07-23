using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Language.Xml.Factory
{
    public class XmlFactory
    {
        public static XmlDocumentSyntax CreateDocument()
        {
            return SyntaxFactory.XmlDocument(null, null, null, null, SyntaxFactory.EofToken);
        }

        public static XmlDocumentSyntax CreateDocumentWithDeclaration(string version, string encoding = "utf-8", string standalone = "yes")
        {
            var xmlDeclaration = CreateDeclaration(version, encoding, standalone);

            return SyntaxFactory.XmlDocument(xmlDeclaration, null, null, null, SyntaxFactory.EofToken);
        }

        public static IXmlElementSyntax CreateElement(string name, string prefix = null, string leadingWhitespace = null, string trailingWhitespace = null)
        {
            SyntaxTrivia leadingTrivia = CreateWhitespace(leadingWhitespace);
            SyntaxTrivia trailingTrivia = CreateWhitespace(trailingWhitespace);

            var ltToken = SyntaxFactory.Token(leadingTrivia, SyntaxKind.LessThanToken, null, "<") as PunctuationSyntax;
            var gtToken = SyntaxFactory.Token(null, SyntaxKind.GreaterThanToken, null, ">") as PunctuationSyntax;
            var ltSlashToken = SyntaxFactory.Token(leadingTrivia, SyntaxKind.LessThanSlashToken, null, "</") as PunctuationSyntax;
            var trailingGtToken = gtToken.AddTrailingTrivia(trailingTrivia);

            XmlNameSyntax elementName = CreateQualifiedName(name, prefix);
            XmlElementStartTagSyntax startTag = SyntaxFactory.XmlElementStartTag(ltToken, elementName, null, gtToken);
            XmlElementEndTagSyntax endTag = SyntaxFactory.XmlElementEndTag(ltSlashToken, elementName, trailingGtToken);

            var xmlElementSyntax = (XmlElementSyntax)SyntaxFactory.XmlElement(startTag, null, endTag);

            return xmlElementSyntax;
        }

        public static XmlEmptyElementSyntax CreateEmptyElement(string name, string prefix = null, string leadingWhitespace = null, string trailingWhitespace = null)
        {
            SyntaxTrivia leadingTrivia = CreateWhitespace(leadingWhitespace);
            SyntaxTrivia trailingTrivia = CreateWhitespace(trailingWhitespace);

            var ltToken = SyntaxFactory.Token(leadingTrivia, SyntaxKind.LessThanToken, null, "<") as PunctuationSyntax;
            var sgtToken = SyntaxFactory.Token(null, SyntaxKind.SlashGreaterThanToken, trailingTrivia, "/>") as PunctuationSyntax;
            XmlNameSyntax elementName = CreateQualifiedName(name, prefix);
           
            var xmlElementSyntax = (XmlEmptyElementSyntax)SyntaxFactory.XmlEmptyElement(ltToken, elementName, null, sgtToken);

            return xmlElementSyntax;
        }

        public static XmlAttributeSyntax CreateAttribute(string name, string value, string prefix = null, string leadingWhitespace = null, string trailingWhitespace = null)
        {
            var equalsToken = SyntaxFactory.Token(null, SyntaxKind.EqualsToken, null, "=") as PunctuationSyntax;
            XmlNameSyntax attributeName = CreateQualifiedName(name, prefix, leadingWhitespace);
            XmlStringSyntax valueXmlString = CreateString(value);

            return (XmlAttributeSyntax)SyntaxFactory.XmlAttribute(attributeName, equalsToken, valueXmlString);
        }

        public static XmlCommentSyntax CreateComment(string text, string leadingWhitespace = null, string trailingWhitespace = null)
        {
            SyntaxTrivia leadingTrivia = CreateWhitespace(leadingWhitespace);
            SyntaxTrivia trailingTrivia = CreateWhitespace(trailingWhitespace);
            SyntaxTrivia spaceTrivia = CreateWhitespace(" ");

            var beginComment = SyntaxFactory.Token(leadingTrivia, SyntaxKind.LessThanExclamationMinusMinusToken, spaceTrivia, "<!--") as PunctuationSyntax;
            var endComment = SyntaxFactory.Token(spaceTrivia, SyntaxKind.MinusMinusGreaterThanToken, trailingTrivia, "-->") as PunctuationSyntax;
            var commentText = CreateText(text);

            return (XmlCommentSyntax)SyntaxFactory.XmlComment(beginComment, commentText, endComment);
        }

        public static XmlDeclarationSyntax CreateDeclaration(string version, string encoding = null, string standalone = null)
        {
            var ltqToken = SyntaxFactory.Token(null, SyntaxKind.LessThanQuestionToken, null, "<?") as PunctuationSyntax;
            var qgtToken = SyntaxFactory.Token(null, SyntaxKind.QuestionGreaterThanToken, null, "?>") as PunctuationSyntax;
            var xmlKeyWord = SyntaxFactory.Token(null, SyntaxKind.XmlKeyword, null, "xml") as PunctuationSyntax;

            XmlDeclarationOptionSyntax versionOption = null;
            XmlDeclarationOptionSyntax encodingOption = null;
            XmlDeclarationOptionSyntax standaloneOption = null;

            if (!string.IsNullOrWhiteSpace(version))
            {
                versionOption = CreateDeclarationOption("version", version);
            }

            if (!string.IsNullOrWhiteSpace(encoding))
            {
                encodingOption = CreateDeclarationOption("encoding", encoding);
            }

            if (!string.IsNullOrWhiteSpace(standalone))
            {
                standaloneOption = CreateDeclarationOption("standalone", standalone);
            }

            return SyntaxFactory.XmlDeclaration(ltqToken, xmlKeyWord, versionOption, encodingOption, standaloneOption, qgtToken); 
        }

        public static XmlTextTokenSyntax CreateText(string value, string leadingWhitespace = null, string trailingWhitespace = null)
        {
            SyntaxTrivia leadingTrivia = CreateWhitespace(leadingWhitespace);
            SyntaxTrivia trailingTrivia = CreateWhitespace(trailingWhitespace);

            return SyntaxFactory.XmlTextLiteralToken(value, value, leadingTrivia, trailingTrivia);
        }

        private static XmlNameSyntax CreateQualifiedName(string name, string prefix, string leadingWhitespace = null, string trailingWhitespace = null)
        {
            SyntaxTrivia leadingTrivia = CreateWhitespace(leadingWhitespace);
            SyntaxTrivia trailingTrivia = CreateWhitespace(trailingWhitespace);

            XmlPrefixSyntax prefixToken = null;

            if (!string.IsNullOrEmpty(prefix))
            {
                XmlNameTokenSyntax prefixNameToken = SyntaxFactory.XmlNameToken(prefix, leadingTrivia, null);
                var colonToken = SyntaxFactory.Token(null, SyntaxKind.ColonToken, null, ":") as PunctuationSyntax;

                prefixToken = SyntaxFactory.XmlPrefix(prefixNameToken, colonToken);
            }

            var leadingNameTrivia = (prefixToken == null) ? leadingTrivia : null;
            XmlNameTokenSyntax nameToken = SyntaxFactory.XmlNameToken(name, leadingNameTrivia, trailingTrivia);

            return SyntaxFactory.XmlName(prefixToken, nameToken);
        }

        private static XmlDeclarationOptionSyntax CreateDeclarationOption(string name, string value)
        {
            var spaceTrivia = SyntaxFactory.WhitespaceTrivia(" ");
            var equalsToken = SyntaxFactory.Token(null, SyntaxKind.EqualsToken, null, "=") as PunctuationSyntax;
            var nameToken = SyntaxFactory.XmlNameToken(name, spaceTrivia, null);
            var valueToken = CreateString(value);

            return SyntaxFactory.XmlDeclarationOption(nameToken, equalsToken, valueToken);
        }

        private static XmlStringSyntax CreateString(string value)
        {
            var quote = SyntaxFactory.Token(null, SyntaxKind.DoubleQuoteToken, null, "\"") as PunctuationSyntax;
            var text = SyntaxFactory.XmlTextLiteralToken(value, value, null, null);

            return SyntaxFactory.XmlString(quote, text, quote);
        }

        private static SyntaxTrivia CreateWhitespace(string value)
        {
            SyntaxTrivia trivia = null;

            if (!string.IsNullOrEmpty(value))
            {
                trivia = SyntaxFactory.WhitespaceTrivia(value);
            }

            return trivia;
        }

        public static XmlCDataSectionSyntax CreateCDataSection(string content, string leadingWhitespace = null, string trailingWhitespace = null)
        {
            SyntaxTrivia leadingTrivia = CreateWhitespace(leadingWhitespace);
            SyntaxTrivia trailingTrivia = CreateWhitespace(trailingWhitespace);

            var bCDataToken = SyntaxFactory.Token(leadingTrivia, SyntaxKind.BeginCDataToken, null, "<![CDATA[") as PunctuationSyntax;
            var eCDataToken = SyntaxFactory.Token(null, SyntaxKind.EndCDataToken, trailingTrivia, "]]>") as PunctuationSyntax;
            var text = CreateText(content);

            return SyntaxFactory.XmlCDataSection(bCDataToken, text, eCDataToken);
        }

        public static XmlProcessingInstructionSyntax CreateProcessingInstruction()
        {
            throw new NotImplementedException();
        }

        public static XmlNodeSyntax ParseText(string text)
        {
            return Parser.ParseText(text)?.Root?.Children?.First();
        }
    }
}
