using Microsoft.Extensions.Configuration;

namespace Infrastructure.Configuration;

public static class AppConfiguration
{
    public static IConfiguration Build()
    {
        var rootDirectory =
            FindRepositoryRoot();

        var envPath =
            Path.Combine(
                rootDirectory,
                ".env");

        if (File.Exists(envPath))
        {
            DotNetEnv.Env.Load(envPath);
        }

        return new ConfigurationBuilder()
            .SetBasePath(rootDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false,
                reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();
    }

    private static string FindRepositoryRoot()
    {
        var directory =
            new DirectoryInfo(
                AppContext.BaseDirectory);

        while (directory != null)
        {
            if (
                File.Exists(
                    Path.Combine(
                        directory.FullName,
                        "fullstack_ai-candidate-assistant.slnx")))
            {
                return directory.FullName;
            }

            directory =
                directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find repository root.");
    }
}