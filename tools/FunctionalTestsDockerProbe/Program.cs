using HuaweiCloud.GaussDB;

namespace FunctionalTestsDockerProbe;

internal static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var connectionString = GetOption(args, "--connection")
            ?? Environment.GetEnvironmentVariable("Test__GaussDB__DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.Error.WriteLine("Missing --connection or Test__GaussDB__DefaultConnection.");
            return 2;
        }

        try
        {
            var builder = new GaussDBConnectionStringBuilder(connectionString);
            if (string.IsNullOrWhiteSpace(builder.Database))
            {
                builder.Database = "postgres";
            }

            await using var connection = new GaussDBConnection(builder.ConnectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT current_database(), current_user";

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                Console.Error.WriteLine("Authentication probe completed but returned no rows.");
                return 1;
            }

            Console.WriteLine(
                $"AUTH_OK Database={reader.GetString(0)} User={reader.GetString(1)} ServerVersion={connection.PostgreSqlVersion}");

            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception);
            return 1;
        }
    }

    private static string? GetOption(IReadOnlyList<string> args, string optionName)
    {
        for (var i = 0; i < args.Count - 1; i++)
        {
            if (string.Equals(args[i], optionName, StringComparison.OrdinalIgnoreCase))
            {
                return args[i + 1];
            }
        }

        return null;
    }
}
