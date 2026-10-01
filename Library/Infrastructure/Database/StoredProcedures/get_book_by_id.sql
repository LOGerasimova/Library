CREATE OR REPLACE FUNCTION get_book_by_id(p_id integer)
RETURNS TABLE
(
    id integer,
    title varchar(500),
    author varchar(300),
    year_of_publication timestamptz,
    contents_node xml
)
LANGUAGE sql
STABLE
AS $$
    SELECT b.id, b.title, b.author, b.year_of_publication, b.contents_node
    FROM books AS b
    WHERE b.id = p_id;
$$;
