using Library.Application.Books.Dto;
using Library.Application.Books.Requests;
using Library.Application.Common.Interfaces;
using Library.Application.Common.Models;
using Library.Domain.Entities;
using Library.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Library.Application.Books
{
    public class BookAppService : IBookAppService
    {
        private readonly IBookRepository _repository;
        private readonly IXmlTocConverter _tocConverter;

        public BookAppService(IBookRepository repository, IXmlTocConverter tocConverter)
        {
            _repository = repository;
            _tocConverter = tocConverter;
        }

        public async Task<int> CreateAsync(CreateBookRequest request, CancellationToken ct = default)
        {
            var yearOfPublicationUtc = TreatInputAsUtc(request.YearOfPublication);
            var contentsNode = ValidateAndConvert(request.Title, request.Author,
                yearOfPublicationUtc, request.TableOfContentsHtml);

            var book = new Book
            {
                Title = request.Title.Trim(),
                Author = request.Author.Trim(),
                YearOfPublication = yearOfPublicationUtc,
                ContentsNode = contentsNode
            };

            return await _repository.InsertAsync(book, ct);
        }

        public async Task UpdateAsync(UpdateBookRequest request, CancellationToken ct = default)
        {
            if (request.Id < 1)
                throw new ValidationException("Идентификатор книги должен быть положительным.");

            var yearOfPublicationUtc = TreatInputAsUtc(request.YearOfPublication);
            var contentsNode = ValidateAndConvert(request.Title, request.Author,
                yearOfPublicationUtc, request.TableOfContentsHtml);

            var book = new Book
            {
                Id = request.Id,
                Title = request.Title.Trim(),
                Author = request.Author.Trim(),
                YearOfPublication = yearOfPublicationUtc,
                ContentsNode = contentsNode
            };

            await _repository.UpdateAsync(book, ct);
        }

        public Task DeleteAsync(int id, CancellationToken ct = default) =>
            _repository.DeleteAsync(id, ct);

        public async Task<BookCardDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var book = await _repository.GetByIdAsync(id, ct)
                ?? throw new KeyNotFoundException($"Книга с Id={id} не найдена");

            return new BookCardDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                YearOfPublication = book.YearOfPublication.ToUniversalTime(),
                TableOfContentsHtml = _tocConverter.ConvertNodeToHtml(book.ContentsNode)
            };
        }

        public async Task<PagedResult<BookSearchResultDto>> SearchAsync(
            SearchBooksRequest request, CancellationToken ct = default)
        {
            var (books, totalCount) = await _repository.SearchAsync(
                request.Id, request.Title, request.Author,
                request.YearOfPublication, request.TocKeyword,
                request.PageNumber, request.PageSize, ct);

            var items = books.Select(book => MapToSearchResult(book, request)).ToList();

            return new PagedResult<BookSearchResultDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        private static BookSearchResultDto MapToSearchResult(Book book, SearchBooksRequest request)
        {
            var tocMatch = FindTocMatch(book.ContentsNode, request.TocKeyword);

            return new BookSearchResultDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                YearOfPublication = book.YearOfPublication.ToUniversalTime(),
                MatchedInTitle = !string.IsNullOrEmpty(request.Title)
                    && book.Title.Contains(request.Title, StringComparison.OrdinalIgnoreCase),
                MatchedInAuthor = !string.IsNullOrEmpty(request.Author)
                    && book.Author.Contains(request.Author, StringComparison.OrdinalIgnoreCase),
                MatchedInToc = tocMatch is not null,
                TocSnippet = tocMatch?.Title
            };
        }

        private static TableOfContentsNode? FindTocMatch(TableOfContentsNode? root, string? keyword)
        {
            if (root is null || string.IsNullOrWhiteSpace(keyword))
                return null;

            return SearchRecursive(root, keyword);
        }

        private static TableOfContentsNode? SearchRecursive(TableOfContentsNode node, string keyword)
        {
            if (node.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return node;

            foreach (var child in node.Children)
            {
                var found = SearchRecursive(child, keyword);
                if (found is not null)
                    return found;
            }

            return null;
        }

        private TableOfContentsNode? ValidateAndConvert(
            string title,
            string author,
            DateTimeOffset yearOfPublication,
            string? tableOfContentsHtml)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ValidationException("Название обязательно.");

            if (title.Trim().Length > 500)
                throw new ValidationException("Название не должно превышать 500 символов.");

            if (string.IsNullOrWhiteSpace(author))
                throw new ValidationException("Автор обязателен.");

            if (author.Trim().Length > 300)
                throw new ValidationException("Имя автора не должно превышать 300 символов.");

            var maximumYear = DateTimeOffset.UtcNow.Year + 1;
            if (yearOfPublication.Year < 1450 || yearOfPublication.Year > maximumYear)
                throw new ValidationException($"Год издания должен быть в диапазоне 1450–{maximumYear}.");

            if (string.IsNullOrWhiteSpace(tableOfContentsHtml))
                return null;

            if (tableOfContentsHtml.Length > 100_000)
                throw new ValidationException("HTML оглавления не должен превышать 100 000 символов.");

            var contentsNode = _tocConverter.ConvertHtmlToNode(tableOfContentsHtml);
            if (contentsNode is null || !HasValidNodes(contentsNode))
            {
                throw new ValidationException(
                    "Оглавление должно содержать хотя бы один пункт в списке <ol> или <ul>.");
            }

            return contentsNode;
        }

        private static DateTimeOffset TreatInputAsUtc(DateTimeOffset value) =>
            new(value.DateTime, TimeSpan.Zero);

        private static bool HasValidNodes(TableOfContentsNode root)
        {
            var pendingNodes = new Stack<TableOfContentsNode>(root.Children);
            var nodeCount = 0;

            while (pendingNodes.TryPop(out var node))
            {
                if (string.IsNullOrWhiteSpace(node.Title) || node.Title.Length > 500)
                    return false;

                if (++nodeCount > 1_000)
                    return false;

                foreach (var child in node.Children)
                    pendingNodes.Push(child);
            }

            return nodeCount > 0;
        }
    }
}
