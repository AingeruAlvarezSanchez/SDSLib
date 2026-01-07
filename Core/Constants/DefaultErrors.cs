namespace SDSLib.Core.Constants;

public static class DefaultErrors {
    public static string FailedToRegister<T>(string key) {
        return $"Failed to register one or more '{key}' in {typeof(T).Name}.";
    }

    public static string NotFound<T>(string key) {
        return $"Key '{key}' loaded in {typeof(T).Name} was not found.";
    }

    public static string DuplicateKey<T>(string key) {
        return $"Ignoring duplicate key '{key}' loaded in {typeof(T).Name}.";
    }

    public static string MissingParameter<T>(string key) {
        return $"Missing parameter '{key}' in {typeof(T).Name}.";
    }

    public static string Unused<T>(string key) {
        return $"Unused parameter '{key}' of type {typeof(T).Name}.";
    }

    public static string InvalidType<T>(string key) {
        return $"Invalid type {typeof(T).Name} for parameter '{key}'.";
    }
}