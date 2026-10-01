namespace Library.Application.Books.Dto;

public class BookSearchResultDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public DateTimeOffset YearOfPublication { get; set; }
    public bool MatchedInTitle { get; set; }
    public bool MatchedInAuthor { get; set; }
    public bool MatchedInToc { get; set; }
    public string? TocSnippet { get; set; }
}
