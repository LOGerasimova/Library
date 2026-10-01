using Library.Domain.Entities;
using Library.Domain.Interfaces;
using Library.Infrastructure.Persistence.DbConstants;
using Microsoft.EntityFrameworkCore;

namespace Library.Infrastructure.Persistence.Repositories;

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _context;

    public BookRepository(LibraryDbContext context) => _context = context;

    public async Task<int> InsertAsync(Book book, CancellationToken ct = default)
    {
        var xml = TableOfContentsXmlSerializer.Serialize(book.ContentsNode);
        var yearOfPublicationUtc = book.YearOfPublication.ToUniversalTime();

        var newId = await _context.Database.SqlQueryRaw<int>(
            $@"SELECT {StoredProcedureNames.InsertBook}(
                    {{0}}, {{1}}, {{2}}, {{3}}::xml) AS ""Value""",
            book.Title, book.Author, yearOfPublicationUtc, xml)
            .SingleAsync(ct);

        book.Id = newId;
        return newId;
    }

    public async Task UpdateAsync(Book book, CancellationToken ct = default)
    {
        var xml = TableOfContentsXmlSerializer.Serialize(book.ContentsNode);
        var yearOfPublicationUtc = book.YearOfPublication.ToUniversalTime();

        await _context.Database.ExecuteSqlRawAsync(
            $@"CALL {StoredProcedureNames.UpdateBook}(
                    {{0}}, {{1}}, {{2}}, {{3}}, {{4}}::xml)",
            new object?[] { book.Id, book.Title, book.Author, yearOfPublicationUtc, xml },
            ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await _context.Database.ExecuteSqlRawAsync(
            $"CALL {StoredProcedureNames.DeleteBook}({{0}})",
            new object?[] { id }, ct);
    }

    public async Task<Book?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Books
            .FromSqlRaw(StoredProcedureNames.GetBookByIdSql, id)
            .SingleOrDefaultAsync(ct);
    }

    public async Task<(IReadOnlyList<Book> Items, int TotalCount)> SearchAsync(
        int? id, string? title, string? author, int? yearOfPublication,
        string? tocKeyword, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var items = await _context.Books
            .FromSqlRaw(StoredProcedureNames.SearchBooksSql,
                id, title, author, yearOfPublication, tocKeyword, pageNumber, pageSize)
            .ToListAsync(ct);

        var totalCount = await _context.Database.SqlQueryRaw<int>(
                StoredProcedureNames.SearchBooksCountSql,
                id, title, author, yearOfPublication, tocKeyword)
            .SingleAsync(ct);

        return (items, totalCount);
    }
}
