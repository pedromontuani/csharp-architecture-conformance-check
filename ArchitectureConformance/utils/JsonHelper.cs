using System.Text.Json;

namespace ArchitetureConformance.utils;

public static class JsonHelper
{
    public static (T? result, bool isValid, string? errorMessage) TryDeserialize<T>(string json)
    {
        try
        {
            T? result = JsonSerializer.Deserialize<T>(json);
            return (result, true, null);
        }
        catch (JsonException ex)
        {
            return (default, false, ex.Message);
        }
    }
}