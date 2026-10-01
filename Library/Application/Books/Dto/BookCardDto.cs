namespace Library.Application.Books.Dto;

public class BookCardDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public DateTimeOffset YearOfPublication { get; set; }
    public string? TableOfContentsHtml { get; set; }
}
