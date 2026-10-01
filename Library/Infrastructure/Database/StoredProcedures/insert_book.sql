CREATE OR REPLACE FUNCTION insert_book(
    p_title varchar(500),
    p_author varchar(300),
    p_year_of_publication timestamptz,
    p_contents_node xml)
RETURNS integer
LANGUAGE plpgsql
AS $$
DECLARE
    v_book_id integer;
BEGIN
    INSERT INTO books (title, author, year_of_publication, contents_node)
    VALUES (btrim(p_title), btrim(p_author), p_year_of_publication, p_contents_node)
    RETURNING id INTO v_book_id;
    RETURN v_book_id;
END;
$$;
