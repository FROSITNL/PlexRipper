using Serilog.Events;

namespace Environment;

public static class EnvironmentExtensions
{
    public const string IntegrationTestModeKey = "IntegrationTestMode";

    public const string UnmaskedModeKey = "UNMASKED";

    public const string LogEnvVarsKey = "LOG_ENV_VARS";

    public const string LogLevelKey = "LOG_LEVEL";

    public const string VersionKey = "VERSION";

    public const string InformationalVersionKey = "INFORMATIONAL_VERSION";

    public const string DevelopmentRootPathKey = "DEVELOPMENT_ROOT_PATH";

    public const string CorsOriginsKey = "CORS_ORIGINS";
    
    public const string SpaRootPath = "SPA_ROOT_PATH";

    public const string CorsAllowAnyOrigin = "CORS_ALLOW_ANY_ORIGIN";

    public const string IngressEntryKey = "INGRESS_ENTRY";

    /// <summary>
    /// Parses command-line arguments into a dictionary.
    /// </summary>
    private static Dictionary<string, string> ParseArguments(string[] args)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var arg in args)
        {
            if (arg.StartsWith("--"))
            {
                var parts = arg.Substring(2).Split('=', 2);
                if (parts.Length == 2)
                {
                    result[parts[0]] = parts[1];
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Overwrite environment variables with command-line arguments if they are set.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static bool? OverwriteEnvsWithArgs(string[] args)
    {
        var arguments = ParseArguments(args);
        foreach (var argKey in arguments)
        {
            var envKey = argKey.Key.Replace("-", "_").ToUpper();
            var envValue = argKey.Value;
            if (!string.IsNullOrWhiteSpace(envValue))
            {
                System.Environment.SetEnvironmentVariable(envKey, envValue);
            }
            else
            {
                // If the value is empty, remove the environment variable
                System.Environment.SetEnvironmentVariable(envKey, null);
            }
        }
        return arguments.Count > 0 ? true : false;
    }

    /// <summary>
    /// Checks if the specified key exists in the command-line arguments.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="args"></param>
    /// <returns>bool</returns>
    public static bool? HasArg(string key, string[] args)
    {
        var arguments = ParseArguments(args);
        return arguments.ContainsKey(key);
    }

    /// <summary>
    /// Get the value of the specified key from the command-line arguments.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string? GetArg(string key, string[] args)
    {
        var arguments = ParseArguments(args);
        return arguments.TryGetValue(key, out var value) ? value : null;
    }

    private static readonly string TrueValue = Convert.ToString(true);

    /// <summary>
    /// Determines if the value is true.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static bool IsTrue(string? value) =>
        value == TrueValue || value == "1" || value == "true" || value == "TRUE";

    public static bool IsIntegrationTestMode() =>
        System.Environment.GetEnvironmentVariable(IntegrationTestModeKey) == TrueValue;

    /// <summary>
    /// This is the path that is used to store the /config, /downloads, /movies and /tvshows folders required to boot PlexRipper in development mode in a non-docker environment.
    /// </summary>
    /// <returns></returns>
    public static string? GetDevelopmentRootPath() => System.Environment.GetEnvironmentVariable(DevelopmentRootPathKey);

    /// <summary>
    /// When set to true, the application will not mask/censor sensitive data in the logs.
    /// </summary>
    public static bool IsUnmasked() => IsTrue(System.Environment.GetEnvironmentVariable(UnmaskedModeKey));

    /// <summary>
    /// When set to true, the application will log all environment variables set on startup
    /// </summary>
    /// <returns></returns>
    public static bool ShouldLogEnvVars() => IsTrue(System.Environment.GetEnvironmentVariable(LogEnvVarsKey));

    public static LogEventLevel GetLogLevel()
    {
        var success = Enum.TryParse<LogEventLevel>(
            System.Environment.GetEnvironmentVariable(LogLevelKey),
            true,
            out var logLevel
        );

        return success ? logLevel : LogEventLevel.Debug;
    }

    public static string GetVersion() =>
        System.Environment.GetEnvironmentVariable(InformationalVersionKey)
        ?? System.Environment.GetEnvironmentVariable(VersionKey)
        ?? "0.0.0";

    public static bool IsDevRelease() => GetVersion().Contains("dev");

    public static int GetPuid() => int.Parse(System.Environment.GetEnvironmentVariable("PUID") ?? "-1");

    public static int GetPgid() => int.Parse(System.Environment.GetEnvironmentVariable("PGID") ?? "-1");

    public static void SetLogLevel(LogEventLevel logLevel)
    {
        System.Environment.SetEnvironmentVariable(LogLevelKey, logLevel.ToString().ToUpper());
    }

    public static void SetIntegrationTestMode(bool state)
    {
        System.Environment.SetEnvironmentVariable(IntegrationTestModeKey, state.ToString());
    }

    /// <summary>
    /// When set to true, the application will not mask/censor sensitive data in the logs.
    /// </summary>
    public static void EnableUnmaskedLog(bool state)
    {
        System.Environment.SetEnvironmentVariable(UnmaskedModeKey, state.ToString());
    }

    /// <summary>
    /// When set to true, the application will log all environment variables set on startup.
    /// </summary>
    public static void EnableLogEnvVars(bool state)
    {
        System.Environment.SetEnvironmentVariable(LogEnvVarsKey, state.ToString());
    }

    /// <summary>
    /// Gets the CORS origins from the environment variable "CORS_ORIGINS" as a string array, or returns null if not set.
    /// </summary>
    public static string[]? GetCorsOrigins()
    {
        var corsOriginsEnv = System.Environment.GetEnvironmentVariable(CorsOriginsKey);
        return string.IsNullOrWhiteSpace(corsOriginsEnv)
            ? null
            : corsOriginsEnv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    /// <summary>
    /// Checks if the CORS policy should allow any origin by checking the environment variable "CORS_ALLOW_ANY_ORIGIN".
    /// </summary>
    public static bool IsCorsAllowAny() => IsTrue(System.Environment.GetEnvironmentVariable(CorsAllowAnyOrigin));

    /// <summary>
    /// Gets the ingress entry point from the environment variable "INGRESS_ENTRY".
    /// </summary>
    public static string? GetIngressEntry()
    {
        return System.Environment.GetEnvironmentVariable(IngressEntryKey) ?? null;
    }
    
    public static string? GetCustomSpaRoot()
    {
        return System.Environment.GetEnvironmentVariable(SpaRootPath) ?? null;
    }

    public static string[]? GetCommandLineArgs()
    {
        var args = System.Environment.GetCommandLineArgs();
        // parse args

        return args.Length > 1
            ? args[1..] // Skip the first argument which is the executable path
            : null;
    }
}
