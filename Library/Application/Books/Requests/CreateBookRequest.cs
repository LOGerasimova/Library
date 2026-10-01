using System.ComponentModel.DataAnnotations;

namespace Library.Application.Books.Requests;

public class CreateBookRequest
{
    [Required(ErrorMessage = "Название обязательно.")]
    [StringLength(500, ErrorMessage = "Название не должно превышать 500 символов.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Автор обязателен.")]
    [StringLength(300, ErrorMessage = "Имя автора не должно превышать 300 символов.")]
    public string Author { get; set; }

    public DateTimeOffset YearOfPublication { get; set; }

    [StringLength(100_000, ErrorMessage = "HTML оглавления не должен превышать 100 000 символов.")]
    public string? TableOfContentsHtml { get; set; }
}
