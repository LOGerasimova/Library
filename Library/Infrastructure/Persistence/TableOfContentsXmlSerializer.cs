using Library.Domain.Entities;
using System.Xml.Linq;

namespace Library.Infrastructure.Persistence;

internal static class TableOfContentsXmlSerializer
{
    public static string? Serialize(TableOfContentsNode? root)
    {
        if (root is null || root.Children.Count == 0)
            return null;

        var xml = new XElement("toc", root.Children.Select(SerializeNode));
        return xml.ToString(SaveOptions.DisableFormatting);
    }

    private static XElement SerializeNode(TableOfContentsNode node) =>
        new("node",
            new XAttribute("title", node.Title),
            new XAttribute("order", node.Order),
            node.Children.Select(SerializeNode));

    public static TableOfContentsNode? Deserialize(string? xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return null;

        var root = XElement.Parse(xml);
        var virtualRoot = new TableOfContentsNode { Title = string.Empty, Level = 0, Order = 0 };
        DeserializeChildren(root, virtualRoot, level: 1);

        return virtualRoot.Children.Count > 0 ? virtualRoot : null;
    }

    private static void DeserializeChildren(XElement parentXml, TableOfContentsNode parentNode, int level)
    {
        foreach (var nodeXml in parentXml.Elements("node"))
        {
            var node = new TableOfContentsNode
            {
                Title = nodeXml.Attribute("title")?.Value ?? string.Empty,
                Order = int.Parse(nodeXml.Attribute("order")?.Value ?? "0"),
                Level = level,
                ParentNode = parentNode
            };
            DeserializeChildren(nodeXml, node, level + 1);
            parentNode.Children.Add(node);
        }
    }
}
