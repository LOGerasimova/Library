CREATE OR REPLACE PROCEDURE update_book(
    p_id integer,
    p_title varchar(500),
    p_author varchar(300),
    p_year_of_publication timestamptz,
    p_contents_node xml)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE books
    SET title = btrim(p_title),
        author = btrim(p_author),
        year_of_publication = p_year_of_publication,
        contents_node = p_contents_node
    WHERE id = p_id;
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Book with id % was not found', p_id
            USING ERRCODE = 'P0002';
    END IF;
    COMMIT;
END;
$$;
