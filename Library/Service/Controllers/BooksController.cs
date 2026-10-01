using Library.Application.Books;
using Library.Application.Books.Requests;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.ComponentModel.DataAnnotations;

namespace Library.Service.Controllers;

[Route("books")]
public class BooksController : Controller
{
    private readonly IBookAppService _bookAppService;

    public BooksController(IBookAppService bookAppService)
    {
        _bookAppService = bookAppService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] SearchBooksRequest request, CancellationToken ct)
    {
        request.PageNumber = Math.Max(request.PageNumber, 1);
        request.PageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;

        var result = await _bookAppService.SearchAsync(request, ct);
        ViewData["SearchRequest"] = request;
        return View(result);
    }

    [HttpGet("{bookId:int}")]
    public async Task<IActionResult> Details(int bookId, CancellationToken ct)
    {
        try
        {
            var book = await _bookAppService.GetByIdAsync(bookId, ct);
            return View(book);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(new CreateBookRequest
        {
            YearOfPublication = new DateTimeOffset(DateTime.UtcNow.Year, 1, 1, 0, 0, 0, TimeSpan.Zero)
        });
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(request);

        try
        {
            var bookId = await _bookAppService.CreateAsync(request, ct);
            TempData["SuccessMessage"] = "Книга добавлена.";
            return RedirectToAction(nameof(Details), new { bookId });
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(request);
        }
    }

    [HttpGet("{bookId:int}/edit")]
    public async Task<IActionResult> Edit(int bookId, CancellationToken ct)
    {
        try
        {
            var book = await _bookAppService.GetByIdAsync(bookId, ct);
            return View(new UpdateBookRequest
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                YearOfPublication = book.YearOfPublication.ToUniversalTime(),
                TableOfContentsHtml = book.TableOfContentsHtml
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{bookId:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int bookId, UpdateBookRequest request, CancellationToken ct)
    {
        if (bookId != request.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(request);

        try
        {
            await _bookAppService.UpdateAsync(request, ct);
            TempData["SuccessMessage"] = "Изменения сохранены.";
            return RedirectToAction(nameof(Details), new { bookId });
        }
        catch (ValidationException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(request);
        }
        catch (PostgresException exception) when (exception.SqlState == "P0002")
        {
            return NotFound();
        }
    }

    [HttpGet("{bookId:int}/delete")]
    public async Task<IActionResult> Delete(int bookId, CancellationToken ct)
    {
        try
        {
            var book = await _bookAppService.GetByIdAsync(bookId, ct);
            return View(book);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("{bookId:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int bookId, CancellationToken ct)
    {
        try
        {
            await _bookAppService.DeleteAsync(bookId, ct);
            TempData["SuccessMessage"] = "Книга удалена.";
            return RedirectToAction(nameof(Index));
        }
        catch (PostgresException exception) when (exception.SqlState == "P0002")
        {
            return NotFound();
        }
    }
}
