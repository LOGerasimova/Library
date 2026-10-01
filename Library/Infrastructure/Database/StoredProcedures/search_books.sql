DROP FUNCTION IF EXISTS search_books(integer, text, text, timestamptz, text, integer, integer);

CREATE OR REPLACE FUNCTION search_books(
    p_id integer,
    p_title text,
    p_author text,
    p_year_of_publication integer,
    p_toc_keyword text,
    p_page_number integer,
    p_page_size integer)
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
    WHERE (p_id IS NULL OR b.id = p_id)
      AND (NULLIF(btrim(p_title), '') IS NULL
           OR b.title ILIKE '%' || btrim(p_title) || '%')
      AND (NULLIF(btrim(p_author), '') IS NULL
           OR b.author ILIKE '%' || btrim(p_author) || '%')
      AND (p_year_of_publication IS NULL
           OR EXTRACT(YEAR FROM b.year_of_publication)::integer = p_year_of_publication)
      AND (NULLIF(btrim(p_toc_keyword), '') IS NULL
           OR to_tsvector('simple', COALESCE(xmlserialize(CONTENT b.contents_node AS text), ''))
              @@ websearch_to_tsquery('simple', btrim(p_toc_keyword))
           OR xmlserialize(CONTENT b.contents_node AS text)
              ILIKE '%' || btrim(p_toc_keyword) || '%')
    ORDER BY b.title, b.author, b.id
    LIMIT LEAST(GREATEST(COALESCE(p_page_size, 20), 1), 100)
    OFFSET (GREATEST(COALESCE(p_page_number, 1), 1) - 1)
           * LEAST(GREATEST(COALESCE(p_page_size, 20), 1), 100);
$$;
