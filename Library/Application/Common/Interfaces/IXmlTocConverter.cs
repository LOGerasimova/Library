using Library.Domain.Entities;

namespace Library.Application.Common.Interfaces;

public interface IXmlTocConverter
{
    TableOfContentsNode? ConvertHtmlToNode(string? html);
    string? ConvertNodeToHtml(TableOfContentsNode? node);
}
