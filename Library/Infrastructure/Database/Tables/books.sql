CREATE TABLE IF NOT EXISTS books
(
    id                  integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    title               varchar(500) NOT NULL,
    author              varchar(300) NOT NULL,
    year_of_publication timestamptz  NOT NULL,
    contents_node       xml,
    created_at          timestamptz  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at          timestamptz  NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT ck_books_title_not_blank CHECK (btrim(title) <> ''),
    CONSTRAINT ck_books_author_not_blank CHECK (btrim(author) <> ''),
    CONSTRAINT ck_books_toc_root CHECK
        (contents_node IS NULL OR xpath_exists('/toc', contents_node))
);

CREATE OR REPLACE FUNCTION set_books_updated_at()
RETURNS trigger
LANGUAGE plpgsql
AS $$
BEGIN
    NEW.updated_at := CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$;

DROP TRIGGER IF EXISTS trg_books_set_updated_at ON books;

CREATE TRIGGER trg_books_set_updated_at
BEFORE UPDATE ON books
FOR EACH ROW
EXECUTE FUNCTION set_books_updated_at();
