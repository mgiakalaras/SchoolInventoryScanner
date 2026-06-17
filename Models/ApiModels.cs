using System.Text.Json.Serialization;

namespace SchoolInventoryScanner.Models;

public sealed class HealthResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("app")]
    public string? App { get; set; }

    [JsonPropertyName("api")]
    public string? Api { get; set; }

    [JsonPropertyName("apiVersion")]
    public string? ApiVersion { get; set; }

    [JsonPropertyName("appVersion")]
    public string? AppVersion { get; set; }

    [JsonPropertyName("serverTime")]
    public DateTime? ServerTime { get; set; }
}

public sealed class AuditFoldersResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("folders")]
    public List<AuditFolderDto> Folders { get; set; } = new();
}

public sealed class AuditFolderDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("auditDate")]
    public DateTime AuditDate { get; set; }

    [JsonPropertyName("schoolName")]
    public string? SchoolName { get; set; }

    [JsonPropertyName("schoolYear")]
    public string? SchoolYear { get; set; }

    [JsonPropertyName("isFinalized")]
    public bool IsFinalized { get; set; }

    [JsonPropertyName("roomSessions")]
    public int RoomSessions { get; set; }

    [JsonPropertyName("expected")]
    public int Expected { get; set; }

    [JsonPropertyName("found")]
    public int Found { get; set; }

    [JsonPropertyName("missing")]
    public int Missing { get; set; }

    [JsonPropertyName("finalizedRooms")]
    public int FinalizedRooms { get; set; }

    public override string ToString()
    {
        var date = AuditDate == default ? "" : AuditDate.ToString("dd/MM/yyyy");
        return $"{Title}\n{SchoolYear} · {date}\n{Found}/{Expected} βρέθηκαν · {FinalizedRooms}/{RoomSessions} χώροι";
    }
}

public sealed class AuditFolderRoomsResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("rooms")]
    public List<RoomSessionDto> Rooms { get; set; } = new();
}

public sealed class RoomSessionDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("roomSessionId")]
    public int RoomSessionId { get; set; }

    [JsonPropertyName("roomName")]
    public string RoomName { get; set; } = "";

    [JsonPropertyName("expectedItemsCount")]
    public int ExpectedItemsCount { get; set; }

    [JsonPropertyName("foundItemsCount")]
    public int FoundItemsCount { get; set; }

    [JsonPropertyName("missingItemsCount")]
    public int MissingItemsCount { get; set; }

    [JsonPropertyName("wrongRoomItemsCount")]
    public int WrongRoomItemsCount { get; set; }

    [JsonPropertyName("unknownItemsCount")]
    public int UnknownItemsCount { get; set; }

    [JsonPropertyName("isFinalized")]
    public bool IsFinalized { get; set; }

    public override string ToString()
    {
        var issues = WrongRoomItemsCount + UnknownItemsCount;
        var locked = IsFinalized ? " · Κλειδωμένο" : "";
        return $"{RoomName}\n{FoundItemsCount}/{ExpectedItemsCount} βρέθηκαν · {MissingItemsCount} λείπουν · {issues} θέματα{locked}";
    }
}

public sealed class RoomSessionResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("session")]
    public RoomSessionDto? Session { get; set; }

    [JsonPropertyName("expectedItems")]
    public List<ExpectedItemDto> ExpectedItems { get; set; } = new();
}

public sealed class ExpectedItemDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("brandModel")]
    public string? BrandModel { get; set; }

    [JsonPropertyName("categoryName")]
    public string? CategoryName { get; set; }

    [JsonPropertyName("serialNumber")]
    public string? SerialNumber { get; set; }

    [JsonPropertyName("scanned")]
    public bool Scanned { get; set; }

    public override string ToString()
    {
        var scanned = Scanned ? "✓ " : "";
        var detail = string.IsNullOrWhiteSpace(BrandModel) ? CategoryName : $"{BrandModel} · {CategoryName}";
        return $"{scanned}{Name}\n{Code}\n{detail}";
    }
}

public sealed class ScanResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("found")]
    public bool Found { get; set; }

    [JsonPropertyName("alreadyScanned")]
    public bool AlreadyScanned { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "";

    [JsonPropertyName("code")]
    public string Code { get; set; } = "";

    [JsonPropertyName("message")]
    public string Message { get; set; } = "";

    [JsonPropertyName("locked")]
    public bool Locked { get; set; }

    [JsonPropertyName("summary")]
    public ScanSummaryDto? Summary { get; set; }
}

public sealed class ScanSummaryDto
{
    [JsonPropertyName("expectedItemsCount")]
    public int ExpectedItemsCount { get; set; }

    [JsonPropertyName("foundItemsCount")]
    public int FoundItemsCount { get; set; }

    [JsonPropertyName("missingItemsCount")]
    public int MissingItemsCount { get; set; }

    [JsonPropertyName("wrongRoomItemsCount")]
    public int WrongRoomItemsCount { get; set; }

    [JsonPropertyName("unknownItemsCount")]
    public int UnknownItemsCount { get; set; }
}


public sealed class QuickAddOptionsResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("defaultCategoryName")]
    public string? DefaultCategoryName { get; set; }

    [JsonPropertyName("quantityDefault")]
    public int QuantityDefault { get; set; } = 1;

    [JsonPropertyName("quantityLabel")]
    public string? QuantityLabel { get; set; }

    [JsonPropertyName("categories")]
    public List<QuickAddCategoryOptionDto> Categories { get; set; } = new();

    [JsonPropertyName("conditions")]
    public List<QuickAddConditionOptionDto> Conditions { get; set; } = new();

    [JsonPropertyName("guidance")]
    public QuickAddGuidanceDto? Guidance { get; set; }
}

public sealed class QuickAddCategoryOptionDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    public override string ToString()
    {
        return Name;
    }
}

public sealed class QuickAddConditionOptionDto
{
    [JsonPropertyName("value")]
    public int Value { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("label")]
    public string Label { get; set; } = "";

    public override string ToString()
    {
        return Label;
    }
}

public sealed class QuickAddGuidanceDto
{
    [JsonPropertyName("primaryFieldLabel")]
    public string? PrimaryFieldLabel { get; set; }

    [JsonPropertyName("newTypeLabel")]
    public string? NewTypeLabel { get; set; }

    [JsonPropertyName("conditionLabel")]
    public string? ConditionLabel { get; set; }

    [JsonPropertyName("notesLabel")]
    public string? NotesLabel { get; set; }

    [JsonPropertyName("reviewFlagText")]
    public string? ReviewFlagText { get; set; }

    [JsonPropertyName("quantityHelpText")]
    public string? QuantityHelpText { get; set; }
}

public sealed class MobileQuickAddItemRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; }

    [JsonPropertyName("categoryName")]
    public string? CategoryName { get; set; }

    [JsonPropertyName("brand")]
    public string? Brand { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("serialNumber")]
    public string? SerialNumber { get; set; }

    [JsonPropertyName("quantity")]
    public int? Quantity { get; set; }

    [JsonPropertyName("condition")]
    public int? Condition { get; set; }

    [JsonPropertyName("conditionText")]
    public string? ConditionText { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }
}

public sealed class QuickAddItemResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("created")]
    public bool Created { get; set; }

    [JsonPropertyName("locked")]
    public bool Locked { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = "";

    [JsonPropertyName("item")]
    public QuickAddItemDto? Item { get; set; }

    [JsonPropertyName("summary")]
    public ScanSummaryDto? Summary { get; set; }
}

public sealed class QuickAddItemDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("brandModel")]
    public string? BrandModel { get; set; }

    [JsonPropertyName("roomId")]
    public int? RoomId { get; set; }

    [JsonPropertyName("roomName")]
    public string? RoomName { get; set; }

    [JsonPropertyName("categoryName")]
    public string? CategoryName { get; set; }

    [JsonPropertyName("serialNumber")]
    public string? SerialNumber { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("condition")]
    public string? Condition { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("needsReview")]
    public bool NeedsReview { get; set; }

    [JsonPropertyName("createdFromMobileAudit")]
    public bool CreatedFromMobileAudit { get; set; }

    public override string ToString()
    {
        var detail = string.IsNullOrWhiteSpace(BrandModel)
            ? CategoryName
            : $"{BrandModel} · {CategoryName}";

        return $"{Name}\n{Code}\n{detail}";
    }
}

public sealed class CreateRoomRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("sortOrder")]
    public int? SortOrder { get; set; }
}

public sealed class CreateRoomResponse
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("createdRoom")]
    public bool CreatedRoom { get; set; }

    [JsonPropertyName("createdSession")]
    public bool CreatedSession { get; set; }

    [JsonPropertyName("alreadyExists")]
    public bool AlreadyExists { get; set; }

    [JsonPropertyName("locked")]
    public bool Locked { get; set; }

    [JsonPropertyName("isFirstInventory")]
    public bool IsFirstInventory { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = "";

    [JsonPropertyName("room")]
    public CreatedRoomDto? Room { get; set; }

    [JsonPropertyName("session")]
    public RoomSessionDto? Session { get; set; }
}

public sealed class CreatedRoomDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("sortOrder")]
    public int SortOrder { get; set; }

    public override string ToString()
    {
        return Name;
    }
}

