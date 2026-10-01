namespace Library.Infrastructure.Persistence.DbConstants;

internal static class StoredProcedureNames
{
    public const string InsertBook = "insert_book";
    public const string UpdateBook = "update_book";
    public const string DeleteBook = "delete_book";


    public const string GetBookByIdSql = "SELECT * FROM get_book_by_id({0})";
    public const string SearchBooksSql =
        "SELECT * FROM search_books({0}::integer, {1}::text, {2}::text, {3}::integer, {4}::text, {5}::integer, {6}::integer)";
    public const string SearchBooksCountSql =
        "SELECT search_books_count({0}::integer, {1}::text, {2}::text, {3}::integer, {4}::text) AS \"Value\"";
}
