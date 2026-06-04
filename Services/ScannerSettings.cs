using Android.Content;

namespace SchoolInventoryScanner.Services;

public sealed class ScannerSettings
{
    private const string PreferencesName = "school_inventory_scanner";
    private const string ServerUrlKey = "server_url";

    private readonly ISharedPreferences _preferences;

    public ScannerSettings(Context context)
    {
        _preferences = context.GetSharedPreferences(PreferencesName, FileCreationMode.Private)!;
    }

    public string ServerUrl
    {
        get => _preferences.GetString(ServerUrlKey, "http://172.26.0.1:5148") ?? "http://172.26.0.1:5148";
        set
        {
            var editor = _preferences.Edit();

            if (editor == null)
            {
                return;
            }

            editor.PutString(ServerUrlKey, NormalizeServerUrl(value));
            editor.Apply();
        }
    }

    public static string NormalizeServerUrl(string value)
    {
        var url = (value ?? "").Trim();

        if (string.IsNullOrWhiteSpace(url))
        {
            return "http://172.26.0.1:5148";
        }

        return url.TrimEnd('/');
    }
}
