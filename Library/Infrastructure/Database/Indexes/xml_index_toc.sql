CREATE INDEX IF NOT EXISTS ix_books_title
    ON books (title);

CREATE INDEX IF NOT EXISTS ix_books_author
    ON books (author);

CREATE INDEX IF NOT EXISTS ix_books_year_of_publication
    ON books (year_of_publication);


CREATE INDEX IF NOT EXISTS ix_books_contents_node_search
    ON books
    USING gin (to_tsvector('simple', COALESCE(xmlserialize(CONTENT contents_node AS text), '')));
