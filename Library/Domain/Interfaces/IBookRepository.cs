using Library.Domain.Entities;

namespace Library.Domain.Interfaces;

public interface IBookRepository
{
    Task<int> InsertAsync(Book book, CancellationToken ct = default);
    Task UpdateAsync(Book book, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<Book?> GetByIdAsync(int id, CancellationToken ct = default);

    Task<(IReadOnlyList<Book> Items, int TotalCount)> SearchAsync(
        int? id,
        string? title,
        string? author,
        int? yearOfPublication,
        string? tocKeyword,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);
}
