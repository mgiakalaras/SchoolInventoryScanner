using System.Net.Http.Json;
using System.Text.Json;
using SchoolInventoryScanner.Models;

namespace SchoolInventoryScanner.Services;

public sealed class ScannerApiClient
{
    private readonly HttpClient _httpClient = new();
    private readonly ScannerSettings _settings;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ScannerApiClient(ScannerSettings settings)
    {
        _settings = settings;
    }

    public async Task<HealthResponse?> GetHealthAsync()
    {
        return await GetAsync<HealthResponse>("/api/mobile/health");
    }

    public async Task<AuditFoldersResponse?> GetAuditFoldersAsync()
    {
        return await GetAsync<AuditFoldersResponse>("/api/mobile/audit-folders");
    }

    public async Task<AuditFolderRoomsResponse?> GetRoomsAsync(int folderId)
    {
        return await GetAsync<AuditFolderRoomsResponse>($"/api/mobile/audit-folders/{folderId}/rooms");
    }

    public async Task<RoomSessionResponse?> GetRoomSessionAsync(int roomSessionId)
    {
        return await GetAsync<RoomSessionResponse>($"/api/mobile/room-sessions/{roomSessionId}");
    }


    public async Task<CreateRoomResponse?> PostCreateRoomAsync(int folderId, CreateRoomRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            BuildUrl($"/api/mobile/audit-folders/{folderId}/rooms/create"),
            request);

        var json = await response.Content.ReadAsStringAsync();

        try
        {
            var result = JsonSerializer.Deserialize<CreateRoomResponse>(json, JsonOptions);

            if (result != null)
            {
                return result;
            }

            return new CreateRoomResponse
            {
                Ok = false,
                Message = response.IsSuccessStatusCode
                    ? "Ο server απάντησε, αλλά το αποτέλεσμα δεν διαβάστηκε."
                    : $"Σφάλμα server: {(int)response.StatusCode}"
            };
        }
        catch
        {
            return new CreateRoomResponse
            {
                Ok = false,
                Message = response.IsSuccessStatusCode
                    ? "Ο server απάντησε, αλλά το αποτέλεσμα δεν διαβάστηκε."
                    : $"Σφάλμα server: {(int)response.StatusCode}"
            };
        }
    }

    public async Task<ScanResponse?> PostScanAsync(int roomSessionId, string code)
    {
        var response = await _httpClient.PostAsJsonAsync(
            BuildUrl($"/api/mobile/room-sessions/{roomSessionId}/scan"),
            new { code });

        var json = await response.Content.ReadAsStringAsync();

        try
        {
            return JsonSerializer.Deserialize<ScanResponse>(json, JsonOptions);
        }
        catch
        {
            return new ScanResponse
            {
                Ok = false,
                Message = response.IsSuccessStatusCode
                    ? "Ο server απάντησε, αλλά το αποτέλεσμα δεν διαβάστηκε."
                    : $"Σφάλμα server: {(int)response.StatusCode}"
            };
        }
    }



    public async Task<QuickAddOptionsResponse?> GetQuickAddOptionsAsync()
    {
        try
        {
            return await GetAsync<QuickAddOptionsResponse>("/api/mobile/quick-add-options");
        }
        catch (Exception ex)
        {
            return new QuickAddOptionsResponse
            {
                Ok = false,
                Message = $"Δεν φορτώθηκαν οι επιλογές γρήγορης καταχώρησης: {ex.Message}"
            };
        }
    }

    public async Task<QuickAddItemResponse?> PostQuickAddItemAsync(int roomSessionId, MobileQuickAddItemRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            BuildUrl($"/api/mobile/room-sessions/{roomSessionId}/add-item"),
            request);

        var json = await response.Content.ReadAsStringAsync();

        try
        {
            var result = JsonSerializer.Deserialize<QuickAddItemResponse>(json, JsonOptions);

            if (result != null)
            {
                return result;
            }

            return new QuickAddItemResponse
            {
                Ok = false,
                Message = response.IsSuccessStatusCode
                    ? "Ο server απάντησε, αλλά το αποτέλεσμα δεν διαβάστηκε."
                    : $"Σφάλμα server: {(int)response.StatusCode}"
            };
        }
        catch
        {
            return new QuickAddItemResponse
            {
                Ok = false,
                Message = response.IsSuccessStatusCode
                    ? "Ο server απάντησε, αλλά το αποτέλεσμα δεν διαβάστηκε."
                    : $"Σφάλμα server: {(int)response.StatusCode}"
            };
        }
    }

    private async Task<T?> GetAsync<T>(string path)
    {
        var response = await _httpClient.GetAsync(BuildUrl(path));
        response.EnsureSuccessStatusCode();

        var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions);
    }

    private string BuildUrl(string path)
    {
        return $"{_settings.ServerUrl}{path}";
    }
}
