using System.ComponentModel.DataAnnotations;

namespace Library.Application.Books.Requests;

public class SearchBooksRequest
{
    public int? Id { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }

    [Range(1450, 9999, ErrorMessage = "Год издания должен быть в диапазоне 1450–9999.")]
    public int? YearOfPublication { get; set; }

    public string? TocKeyword { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
