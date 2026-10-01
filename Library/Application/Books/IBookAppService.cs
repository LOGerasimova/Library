using Library.Application.Books.Dto;
using Library.Application.Books.Requests;
using Library.Application.Common.Models;

namespace Library.Application.Books;

public interface IBookAppService
{
    Task<int> CreateAsync(CreateBookRequest request, CancellationToken ct);
    Task UpdateAsync(UpdateBookRequest request, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
    Task<BookCardDto> GetByIdAsync(int id, CancellationToken ct);
    Task<PagedResult<BookSearchResultDto>> SearchAsync(SearchBooksRequest request, CancellationToken ct);
}
