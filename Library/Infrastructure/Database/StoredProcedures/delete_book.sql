CREATE OR REPLACE PROCEDURE delete_book(p_id integer)
LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM books
    WHERE id = p_id;
    IF NOT FOUND THEN
        RAISE EXCEPTION 'Book with id % was not found', p_id
            USING ERRCODE = 'P0002';
    END IF;
    COMMIT;
END;
$$;
