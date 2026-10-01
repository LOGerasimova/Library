namespace Library.Infrastructure.Persistence;

public sealed class PostgresSettings
{
    public const string SectionName = "Postgres";

    public required string Host { get; init; }

    public required int Port { get; init; }

    public required string Database { get; init; }

    public required string User { get; init; }

    public required string Password { get; init; }

    public required bool TrustServerCertificate { get; init; }
}
