using HtmlAgilityPack;

using Library.Application.Common.Interfaces;
using Library.Domain.Entities;

using System.Net;
using System.Text;
using System.Xml;

namespace Library.Infrastructure.Xml;

public class XmlTocConverter : IXmlTocConverter
{
    public TableOfContentsNode? ConvertHtmlToNode(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return null;

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var rootList = doc.DocumentNode.SelectSingleNode("//ol|//ul");
        if (rootList is null)
            return null;

        var root = new TableOfContentsNode { Title = string.Empty, Level = 0, Order = 0 };
        BuildChildren(rootList, root, level: 1);

        return root.Children.Count > 0 ? root : null;
    }

    private static void BuildChildren(HtmlNode listNode, TableOfContentsNode parent, int level)
    {
        var order = 1;
        foreach (var li in listNode.ChildNodes.Where(n => n.Name == "li"))
        {
            var titleText = ExtractTitle(li);

            var node = new TableOfContentsNode
            {
                Title = HtmlEntity.DeEntitize(titleText),
                Level = level,
                Order = order++,
                ParentNode = parent
            };

            var childList = li.SelectSingleNode("./ol|./ul");
            if (childList is not null)
                BuildChildren(childList, node, level + 1);

            parent.Children.Add(node);
        }
    }

    private static string ExtractTitle(HtmlNode listItem)
    {
        var text = new StringBuilder();
        AppendVisibleText(listItem, text);

        return HtmlEntity.DeEntitize(text.ToString()).Trim();
    }

    private static void AppendVisibleText(HtmlNode node, StringBuilder text)
    {
        foreach (var child in node.ChildNodes)
        {
            if (child.NodeType == HtmlNodeType.Element
                && (child.Name.Equals("ol", StringComparison.OrdinalIgnoreCase)
                    || child.Name.Equals("ul", StringComparison.OrdinalIgnoreCase)
                    || child.Name.Equals("script", StringComparison.OrdinalIgnoreCase)
                    || child.Name.Equals("style", StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            if (child.NodeType == HtmlNodeType.Text)
            {
                AppendNormalizedText(child.InnerText, text);
                continue;
            }

            var lengthBefore = text.Length;
            AppendVisibleText(child, text);
            if (text.Length > lengthBefore)
                text.Append(' ');
        }
    }

    private static void AppendNormalizedText(string value, StringBuilder text)
    {
        var words = value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0)
            return;

        if (text.Length > 0 && !char.IsWhiteSpace(text[^1]))
            text.Append(' ');

        text.Append(string.Join(' ', words));
    }

    public string? ConvertNodeToHtml(TableOfContentsNode? node)
    {
        if (node is null || node.Children.Count == 0)
            return null;

        var sb = new StringBuilder();
        WriteList(node.Children, sb);
        return sb.ToString();
    }

    private static void WriteList(IEnumerable<TableOfContentsNode> nodes, StringBuilder sb)
    {
        var ordered = nodes.OrderBy(n => n.Order).ToList();
        if (ordered.Count == 0) return;

        sb.Append("<ol>");
        foreach (var node in ordered)
        {
            sb.Append("<li>").Append(WebUtility.HtmlEncode(node.Title));
            if (node.Children.Count > 0)
                WriteList(node.Children, sb);
            sb.Append("</li>");
        }
        sb.Append("</ol>");
    }
}
