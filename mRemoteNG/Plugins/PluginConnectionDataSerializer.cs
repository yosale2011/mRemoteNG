using System.Collections.Generic;
using Newtonsoft.Json;

namespace mRemoteNG.Plugins;

internal static class PluginConnectionDataSerializer
{
    public static IReadOnlyDictionary<string, string> Deserialize(string serializedProperties)
    {
        if (string.IsNullOrWhiteSpace(serializedProperties))
        {
            return new Dictionary<string, string>();
        }

        try
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedProperties) ??
                   new Dictionary<string, string>();
        }
        catch (JsonException)
        {
            // Malformed plugin data (e.g. a misaligned CSV column) should not prevent the
            // connection itself from loading; the plugin simply sees no stored properties.
            return new Dictionary<string, string>();
        }
    }

    public static string Serialize(IReadOnlyDictionary<string, string> properties)
    {
        if (properties.Count == 0)
        {
            return string.Empty;
        }

        return JsonConvert.SerializeObject(properties);
    }
}
