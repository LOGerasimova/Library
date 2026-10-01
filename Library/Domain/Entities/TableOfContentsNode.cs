namespace Library.Domain.Entities;

public class TableOfContentsNode
{
    public string Title { get; set; }
    public int Level { get; set; }
    public int Order { get; set; }
    public TableOfContentsNode? ParentNode { get; set; }
    public ICollection<TableOfContentsNode> Children { get; set; } = [];
}
