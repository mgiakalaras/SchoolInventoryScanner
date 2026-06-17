using Android.App;
using Android.OS;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using SchoolInventoryScanner.Models;
using SchoolInventoryScanner.Services;

namespace SchoolInventoryScanner;

[Activity(
    Label = "School Inventory Scanner",
    Theme = "@style/AppTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ScreenOrientation = ScreenOrientation.Portrait,
    ConfigurationChanges =
        ConfigChanges.ScreenSize |
        ConfigChanges.Orientation |
        ConfigChanges.UiMode |
        ConfigChanges.ScreenLayout |
        ConfigChanges.SmallestScreenSize |
        ConfigChanges.Density)]
public sealed class MainActivity : Activity
{
    private static readonly Color Background = Color.ParseColor("#06111F");
    private static readonly Color Surface = Color.ParseColor("#0B1728");
    private static readonly Color Surface2 = Color.ParseColor("#101C2E");
    private static readonly Color Surface3 = Color.ParseColor("#16243A");
    private static readonly Color Line = Color.ParseColor("#26364E");
    private static readonly Color Text = Color.ParseColor("#F8FAFC");
    private static readonly Color Muted = Color.ParseColor("#94A3B8");
    private static readonly Color Cyan = Color.ParseColor("#22D3EE");
    private static readonly Color Blue = Color.ParseColor("#4F46E5");
    private static readonly Color Green = Color.ParseColor("#34D399");
    private static readonly Color Amber = Color.ParseColor("#F59E0B");
    private static readonly Color Red = Color.ParseColor("#F87171");
    private static readonly Color Purple = Color.ParseColor("#A78BFA");
    private static readonly Color DarkText = Color.ParseColor("#06121F");

    private ScannerSettings _settings = null!;
    private ScannerApiClient _api = null!;

    private string _screen = "home";
    private AuditFolderDto? _selectedFolder;
    private RoomSessionDto? _selectedRoom;

    private EditText? _serverUrlInput;
    private TextView? _connectionStatus;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        Window?.SetStatusBarColor(Color.ParseColor("#020817"));
        Window?.SetNavigationBarColor(Color.ParseColor("#020817"));

        _settings = new ScannerSettings(this);
        _api = new ScannerApiClient(_settings);

        ShowHome();
    }

    public override void OnBackPressed()
    {
        if (_screen == "createroom")
        {
            ShowRooms();
            return;
        }

        if (_screen == "quickadd")
        {
            if (_selectedRoom != null)
            {
                _ = ShowRoomSessionAsync(_selectedRoom);
                return;
            }

            ShowRooms();
            return;
        }

        if (_screen == "session")
        {
            _selectedRoom = null;
            ShowRooms();
            return;
        }

        if (_screen == "rooms")
        {
            _selectedFolder = null;
            ShowFolders();
            return;
        }

        if (_screen == "folders" || _screen == "settings" || _screen == "help")
        {
            ShowHome();
            return;
        }

        base.OnBackPressed();
    }

    private void ShowHome()
    {
        _screen = "home";
        _selectedFolder = null;
        _selectedRoom = null;

        var root = CreateLinearRoot();

        root.AddView(CreateAppHero(
            "School Inventory\nScanner",
            "Γρήγορη και καθαρή απογραφή με QR.",
            "Σύνδεση: " + _settings.ServerUrl,
            "▦",
            Cyan));

        var grid = CreateTileGrid(
            CreateTile("Φάκελοι", "Άνοιγμα απογραφών", "▤", Blue, async () => await ShowFoldersAsync()),
            CreateTile("Νέα σάρωση", "Επιλογή φακέλου/χώρου", "⌗", Green, async () => await ShowFoldersAsync()),
            CreateTile("Ρυθμίσεις", "URL server", "⚙", Purple, () => ShowSettings()),
            CreateTile("Βοήθεια", "Ροή χρήσης", "?", Amber, () => ShowHelp()));

        root.AddView(grid);

        var test = CreatePrimaryButton("Έλεγχος σύνδεσης");
        var status = CreateBodyText("Πάτα για γρήγορο health check στο web app.");
        test.Click += async (_, _) => await TestConnectionAsync(status);

        root.AddView(CreateCard(Stack(
            CreateSectionHeader("Κατάσταση server", "Έλεγχος σύνδεσης με το School Inventory Manager."),
            test,
            status)));

        root.AddView(CreateBottomNav("home"));
        SetContentView(WrapInScrollView(root));
    }

    private void ShowSettings()
    {
        _screen = "settings";

        _serverUrlInput = CreateEditText("http://172.26.0.1:5148");
        _serverUrlInput.Text = _settings.ServerUrl;

        _connectionStatus = CreateBodyText("Δεν έχει γίνει ακόμα δοκιμή σύνδεσης.");

        var save = CreatePrimaryButton("Αποθήκευση και δοκιμή");
        save.Click += async (_, _) => await SaveAndTestConnectionAsync();

        var folders = CreateSecondaryButton("Άνοιγμα φακέλων");
        folders.Click += async (_, _) =>
        {
            SaveServerUrl();
            await ShowFoldersAsync();
        };

        var root = CreateLinearRoot();
        root.AddView(CreateScreenHeader("Ρυθμίσεις", "Σύνδεση με server", "⚙", Purple));
        root.AddView(CreateCard(Stack(
            CreateSectionHeader("URL Server", "Η διεύθυνση του web app που τρέχει στο σχολικό δίκτυο ή στο Zima/VPN."),
            _serverUrlInput,
            save,
            folders)));

        root.AddView(CreateStatusCard("Κατάσταση", _connectionStatus));
        root.AddView(CreateInfoStrip("Παράδειγμα", "Για το δικό σου setup συνήθως: http://172.26.0.1:5148"));
        root.AddView(CreateBottomNav("settings"));

        SetContentView(WrapInScrollView(root));
    }

    private async Task TestConnectionAsync(TextView status)
    {
        status.Text = "Δοκιμή σύνδεσης...";

        try
        {
            var health = await _api.GetHealthAsync();
            status.Text = health?.Ok == true
                ? $"Σύνδεση OK · {health.App} · έκδοση {health.AppVersion}"
                : "Ο server απάντησε, αλλά το health response δεν ήταν έγκυρο.";
            status.SetTextColor(health?.Ok == true ? Green : Amber);
        }
        catch (Exception ex)
        {
            status.Text = $"Αποτυχία σύνδεσης: {ex.Message}";
            status.SetTextColor(Red);
        }
    }

    private async Task SaveAndTestConnectionAsync()
    {
        SaveServerUrl();

        if (_connectionStatus == null)
        {
            return;
        }

        await TestConnectionAsync(_connectionStatus);
    }

    private async Task ShowFoldersAsync()
    {
        _screen = "folders";
        ShowLoading("Φάκελοι", "Φόρτωση φακέλων απογραφής...");

        try
        {
            var response = await _api.GetAuditFoldersAsync();
            var folders = response?.Folders ?? new List<AuditFolderDto>();
            RunOnUiThread(() => ShowFolders(folders));
        }
        catch (Exception ex)
        {
            ShowError("Φάκελοι", $"Σφάλμα φόρτωσης: {ex.Message}", ShowHome);
        }
    }

    private void ShowFolders()
    {
        _ = ShowFoldersAsync();
    }

    private void ShowFolders(List<AuditFolderDto> folders)
    {
        _screen = "folders";
        _selectedFolder = null;
        _selectedRoom = null;

        var root = CreateLinearRoot();
        root.AddView(CreateScreenHeader("Φάκελοι απογραφής", "Επίλεξε φάκελο για να δεις χώρους.", "▤", Blue));

        var refresh = CreateSecondaryButton("Ανανέωση");
        refresh.Click += async (_, _) => await ShowFoldersAsync();
        root.AddView(refresh);

        if (folders.Count == 0)
        {
            root.AddView(CreateEmptyState("Δεν υπάρχουν φάκελοι", "Δημιούργησε πρώτα έναν φάκελο απογραφής από το web app."));
        }
        else
        {
            foreach (var folder in folders)
            {
                root.AddView(CreateFolderCard(folder));
            }
        }

        root.AddView(CreateBottomNav("folders"));
        SetContentView(WrapInScrollView(root));
    }

    private View CreateFolderCard(AuditFolderDto folder)
    {
        var title = CreateTitleText(folder.Title, 19);
        var meta = CreateBodyText($"{folder.SchoolName ?? "Σχολική μονάδα"} · {folder.SchoolYear} · {folder.AuditDate:dd/MM/yyyy}");

        var progress = CreateProgressBar(folder.Found, Math.Max(1, folder.Expected), Green);

        var stats = CreateStatsRow(new[]
        {
            ("Χώροι", folder.RoomSessions.ToString(), Blue),
            ("Βρέθηκαν", $"{folder.Found}/{folder.Expected}", Green),
            ("Λείπουν", folder.Missing.ToString(), folder.Missing > 0 ? Amber : Muted)
        });

        var open = CreatePrimaryButton("Άνοιγμα φακέλου");
        open.Click += async (_, _) =>
        {
            _selectedFolder = folder;
            await ShowRoomsAsync(folder);
        };

        return CreateAccentCard(Stack(title, meta, progress, stats, open), Blue);
    }

    private async Task ShowRoomsAsync(AuditFolderDto folder)
    {
        _screen = "rooms";
        ShowLoading(folder.Title, "Φόρτωση χώρων...");

        try
        {
            var response = await _api.GetRoomsAsync(folder.Id);
            var rooms = response?.Rooms ?? new List<RoomSessionDto>();
            RunOnUiThread(() => ShowRooms(folder, rooms));
        }
        catch (Exception ex)
        {
            ShowError(folder.Title, $"Σφάλμα φόρτωσης χώρων: {ex.Message}", ShowFolders);
        }
    }

    private void ShowRooms()
    {
        if (_selectedFolder != null)
        {
            _ = ShowRoomsAsync(_selectedFolder);
        }
    }


    private void ShowRooms(AuditFolderDto folder, List<RoomSessionDto> rooms)
    {
        _screen = "rooms";
        _selectedFolder = folder;
        _selectedRoom = null;

        var root = CreateLinearRoot();
        root.AddView(CreateScreenHeader(folder.Title, $"{rooms.Count} χώροι διαθέσιμοι", "⌂", Cyan));

        var back = CreateSecondaryButton("Πίσω στους φακέλους");
        back.Click += async (_, _) => await ShowFoldersAsync();

        var createRoom = CreatePrimaryButton("+ Νέος χώρος");
        createRoom.Click += (_, _) => ShowCreateRoom(folder);

        root.AddView(Stack(back, createRoom));

        if (rooms.Count == 0)
        {
            root.AddView(CreateEmptyState(
                "Δεν υπάρχουν χώροι",
                "Αν αυτός είναι φάκελος πρώτης απογραφής, ξεκίνα με το + Νέος χώρος και μετά πρόσθεσε αντικείμενα μέσα στον χώρο."));
        }
        else
        {
            root.AddView(CreateInfoStrip(
                "Χώροι φακέλου",
                "Άνοιξε έναν χώρο για σάρωση ή πάτα + Νέος χώρος αν βρίσκεσαι σε πρώτη απογραφή και πρέπει να δημιουργήσεις νέα αίθουσα/χώρο."));

            foreach (var room in rooms)
            {
                root.AddView(CreateRoomCard(room));
            }
        }

        root.AddView(CreateBottomNav("folders"));
        SetContentView(WrapInScrollView(root));
    }


    private View CreateRoomCard(RoomSessionDto room)
    {
        var status = room.IsFinalized
            ? CreatePill("ΟΡΙΣΤΙΚΟ", Purple)
            : CreatePill("ΕΝΕΡΓΟ", Green);

        var title = CreateTitleText(room.RoomName, 19);
        var top = CreateRow(title, status);

        var progress = CreateProgressBar(room.FoundItemsCount, Math.Max(1, room.ExpectedItemsCount), Green);

        var stats = CreateStatsRow(new[]
        {
            ("Βρέθηκαν", $"{room.FoundItemsCount}/{room.ExpectedItemsCount}", Green),
            ("Λείπουν", room.MissingItemsCount.ToString(), room.MissingItemsCount > 0 ? Amber : Muted),
            ("Θέματα", (room.WrongRoomItemsCount + room.UnknownItemsCount).ToString(), room.WrongRoomItemsCount + room.UnknownItemsCount > 0 ? Red : Muted)
        });

        var open = CreatePrimaryButton("Άνοιγμα χώρου");
        open.Click += async (_, _) =>
        {
            _selectedRoom = room;
            await ShowRoomSessionAsync(room);
        };

        return CreateAccentCard(Stack(top, progress, stats, open), room.IsFinalized ? Purple : Cyan);
    }


    private void ShowCreateRoom(AuditFolderDto folder)
    {
        _screen = "createroom";
        _selectedFolder = folder;
        _selectedRoom = null;

        var root = CreateLinearRoot();

        root.AddView(CreateScreenHeader(
            "Νέος χώρος",
            folder.Title,
            "+",
            Green));

        var back = CreateSecondaryButton("Πίσω στους χώρους");
        back.Click += (_, _) => ShowRooms();
        root.AddView(back);

        var nameInput = CreateEditText("Όνομα χώρου *");
        var resultText = CreateBodyText("Δώσε καθαρό όνομα χώρου, π.χ. Αίθουσα Α1, Εργαστήριο Πληροφορικής, Γραφείο Διευθυντή.");
        resultText.SetTextColor(Muted);

        var submit = CreatePrimaryButton("Δημιουργία χώρου");
        submit.Click += async (_, _) => await SubmitCreateRoomAsync(folder, nameInput, resultText, submit);

        root.AddView(CreateAccentCard(Stack(
            CreateSectionHeader("Δημιουργία χώρου", "Για πρώτη απογραφή μπορείς να στήσεις τους χώρους απευθείας από το κινητό/tablet."),
            CreateFieldLabel("Όνομα χώρου *"),
            nameInput,
            CreateSmallText("Χρησιμοποίησε ονόματα όπως θα θέλεις να φαίνονται μετά στις ετικέτες και στις αναφορές."),
            submit,
            resultText), Green));

        root.AddView(CreateInfoStrip(
            "Μετά τη δημιουργία",
            "Ο χώρος θα εμφανιστεί στη λίστα του φακέλου. Άνοιξέ τον και πρόσθεσε τα αντικείμενα που βρίσκεις στον χώρο."));

        root.AddView(CreateBottomNav("folders"));
        SetContentView(WrapInScrollView(root));
    }

    private async Task SubmitCreateRoomAsync(
        AuditFolderDto folder,
        EditText nameInput,
        TextView resultText,
        Button submitButton)
    {
        var name = nameInput.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
        {
            resultText.Text = "Συμπλήρωσε όνομα χώρου.";
            resultText.SetTextColor(Amber);
            return;
        }

        HideKeyboard(nameInput);

        submitButton.Enabled = false;
        resultText.Text = "Δημιουργία χώρου...";
        resultText.SetTextColor(Muted);

        try
        {
            var response = await _api.PostCreateRoomAsync(folder.Id, new CreateRoomRequest
            {
                Name = name
            });

            if (response?.Ok == true)
            {
                resultText.Text = response.Message;
                resultText.SetTextColor(Green);

                await Task.Delay(650);
                await ShowRoomsAsync(folder);
                return;
            }

            resultText.Text = response?.Message ?? "Δεν ελήφθη απάντηση από τον server.";
            resultText.SetTextColor(response?.Locked == true ? Amber : Red);
            submitButton.Enabled = true;
        }
        catch (Exception ex)
        {
            resultText.Text = $"Σφάλμα: {ex.Message}";
            resultText.SetTextColor(Red);
            submitButton.Enabled = true;
        }
    }

    private async Task ShowRoomSessionAsync(RoomSessionDto room)
    {
        _screen = "session";
        ShowLoading(room.RoomName, "Φόρτωση συνεδρίας χώρου...");

        try
        {
            var response = await _api.GetRoomSessionAsync(room.Id);
            RunOnUiThread(() => ShowRoomSession(room, response));
        }
        catch (Exception ex)
        {
            ShowError(room.RoomName, $"Σφάλμα φόρτωσης απογραφής: {ex.Message}", ShowRooms);
        }
    }

    private void ShowRoomSession(RoomSessionDto room, RoomSessionResponse? response)
    {
        _screen = "session";
        _selectedRoom = room;

        var session = response?.Session ?? room;
        var items = response?.ExpectedItems ?? new List<ExpectedItemDto>();

        var root = CreateLinearRoot();

        root.AddView(CreateScreenHeader(
            session.RoomName,
            $"{session.FoundItemsCount}/{session.ExpectedItemsCount} βρέθηκαν · {session.MissingItemsCount} λείπουν",
            "⌗",
            Green));

        var back = CreateSecondaryButton("Πίσω στους χώρους");
        back.Click += (_, _) => ShowRooms();
        root.AddView(back);

        root.AddView(CreateAccentCard(Stack(
            CreateSectionHeader("Πρόοδος", "Σύνοψη τρέχουσας απογραφής χώρου."),
            CreateProgressBar(session.FoundItemsCount, Math.Max(1, session.ExpectedItemsCount), Green),
            CreateStatsRow(new[]
            {
                ("Σύνολο", session.ExpectedItemsCount.ToString(), Blue),
                ("Βρέθηκαν", session.FoundItemsCount.ToString(), Green),
                ("Λείπουν", session.MissingItemsCount.ToString(), session.MissingItemsCount > 0 ? Amber : Muted),
                ("Θέματα", (session.WrongRoomItemsCount + session.UnknownItemsCount).ToString(), session.WrongRoomItemsCount + session.UnknownItemsCount > 0 ? Red : Muted)
            })), Green));

        var codeInput = CreateEditText("SIM-2026-000123 ή full QR URL");
        var resultText = CreateBodyText("Πάτα σάρωση ή γράψε χειροκίνητα κωδικό QR.");

        var cameraButton = CreatePrimaryButton("Σάρωση QR με κάμερα");
        cameraButton.Click += async (_, _) => await StartGoogleCodeScanAsync(room, resultText, items);

        var manualButton = CreateSecondaryButton("Χειροκίνητη καταγραφή");
        manualButton.Click += async (_, _) =>
        {
            var code = codeInput.Text?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(code))
            {
                resultText.Text = "Γράψε κωδικό πρώτα.";
                resultText.SetTextColor(Amber);
                return;
            }

            HideKeyboard(codeInput);

            var alreadyScannedItem = FindAlreadyScannedExpectedItem(items, code);
            if (alreadyScannedItem != null)
            {
                ShowAlreadyScannedDialog(alreadyScannedItem);
                resultText.Text = $"Το αντικείμενο έχει ήδη σαρωθεί: {alreadyScannedItem.Name}";
                resultText.SetTextColor(Amber);
                codeInput.Text = string.Empty;
                return;
            }

            resultText.Text = "Αποστολή scan...";
            resultText.SetTextColor(Muted);

            try
            {
                var scanResponse = await _api.PostScanAsync(room.Id, code);
                ApplyScanResultMessage(scanResponse, resultText);
                codeInput.Text = string.Empty;

                await Task.Delay(scanResponse?.AlreadyScanned == true ? 1600 : 650);
                await ShowRoomSessionAsync(room);
            }
            catch (Exception ex)
            {
                resultText.Text = $"Σφάλμα: {ex.Message}";
                resultText.SetTextColor(Red);
            }
        };

        var quickAddButton = CreateSecondaryButton("+ Νέο αντικείμενο στον χώρο");
        quickAddButton.Click += async (_, _) => await ShowQuickAddItemAsync(room);

        root.AddView(CreateAccentCard(Stack(
            CreateSectionHeader("Σάρωση αντικειμένων", "Η κάμερα είναι η βασική ροή. Το χειροκίνητο πεδίο μένει ως fallback."),
            cameraButton,
            CreateDivider(),
            codeInput,
            manualButton,
            resultText,
            CreateDivider(),
            quickAddButton,
            CreateSmallText("Αν βρεις αντικείμενο που δεν υπάρχει στα αναμενόμενα, πρόσθεσέ το προσωρινά ως νέο εύρημα.")), Purple));

        root.AddView(CreateSectionTitle("Αναμενόμενα αντικείμενα"));

        if (items.Count == 0)
        {
            root.AddView(CreateEmptyState("Κενή λίστα", "Δεν βρέθηκαν αναμενόμενα αντικείμενα για αυτόν τον χώρο."));
        }
        else
        {
            foreach (var item in items.Take(150))
            {
                root.AddView(CreateItemCard(item));
            }

            if (items.Count > 150)
            {
                root.AddView(CreateInfoStrip("Σημείωση", $"Εμφανίζονται τα πρώτα 150 από {items.Count} αντικείμενα."));
            }
        }

        root.AddView(CreateBottomNav("scan"));
        SetContentView(WrapInScrollView(root));
    }



    private async Task ShowQuickAddItemAsync(RoomSessionDto room)
    {
        _screen = "quickadd";
        _selectedRoom = room;

        ShowLoading("Φόρτωση", "Φόρτωση επιλογών γρήγορης καταχώρησης...");

        QuickAddOptionsResponse options;

        try
        {
            options = await _api.GetQuickAddOptionsAsync() ?? CreateFallbackQuickAddOptions();
        }
        catch
        {
            options = CreateFallbackQuickAddOptions();
        }

        ShowQuickAddItem(room, options);
    }

    private void ShowQuickAddItem(RoomSessionDto room, QuickAddOptionsResponse options)
    {
        _screen = "quickadd";
        _selectedRoom = room;

        var root = CreateLinearRoot();

        root.AddView(CreateScreenHeader(
            "Νέο αντικείμενο",
            room.RoomName,
            "+",
            Green));

        var back = CreateSecondaryButton("Πίσω στον χώρο");
        back.Click += async (_, _) => await ShowRoomSessionAsync(room);
        root.AddView(back);

        var guidance = options.Guidance ?? new QuickAddGuidanceDto();

        var primaryFieldLabel = string.IsNullOrWhiteSpace(guidance.PrimaryFieldLabel)
            ? "Τύπος αντικειμένου"
            : guidance.PrimaryFieldLabel.Trim();

        var newTypeLabel = string.IsNullOrWhiteSpace(guidance.NewTypeLabel)
            ? "Νέος τύπος αντικειμένου"
            : guidance.NewTypeLabel.Trim();

        var conditionLabel = string.IsNullOrWhiteSpace(guidance.ConditionLabel)
            ? "Κατάσταση"
            : guidance.ConditionLabel.Trim();

        var notesLabel = string.IsNullOrWhiteSpace(guidance.NotesLabel)
            ? "Σημείωση"
            : guidance.NotesLabel.Trim();

        var reviewFlagText = string.IsNullOrWhiteSpace(guidance.ReviewFlagText)
            ? "Προς έλεγχο από web app"
            : guidance.ReviewFlagText.Trim();

        var quantityLabel = string.IsNullOrWhiteSpace(options.QuantityLabel)
            ? "Ποσότητα (συνήθως 1)"
            : options.QuantityLabel.Trim();

        var quantityHelpText = string.IsNullOrWhiteSpace(guidance.QuantityHelpText)
            ? "Για κανονικό εξοπλισμό άφησέ το 1."
            : guidance.QuantityHelpText.Trim();

        var defaultTypeName = string.IsNullOrWhiteSpace(options.DefaultCategoryName)
            ? "Άλλο"
            : options.DefaultCategoryName.Trim();

        var typeNames = options.Categories
            .Select(x => x.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct()
            .ToList();

        if (typeNames.Count == 0)
        {
            typeNames.Add(defaultTypeName);
        }

        var typeSpinner = CreateSpinner(typeNames, FindDefaultTypeIndex(typeNames, defaultTypeName));
        var newTypeInput = CreateEditText(newTypeLabel);

        var conditionOptions = options.Conditions.Count > 0
            ? options.Conditions
            : CreateFallbackQuickAddOptions().Conditions;

        var conditionSpinner = CreateSpinner(
            conditionOptions.Select(x => x.Label).ToList(),
            FindDefaultConditionIndex(conditionOptions));

        var brandInput = CreateEditText("Μάρκα");
        var modelInput = CreateEditText("Μοντέλο");
        var serialInput = CreateEditText("Serial Number");

        var quantityInput = CreateEditText(quantityLabel);
        quantityInput.Text = Math.Max(1, options.QuantityDefault).ToString();

        var notesInput = CreateEditText(notesLabel);

        var resultText = CreateBodyText("Το αντικείμενο θα προστεθεί στον τρέχοντα χώρο ως νέο εύρημα και θα μείνει για έλεγχο από το web app.");
        resultText.SetTextColor(Muted);

        var submit = CreatePrimaryButton("Προσθήκη στον χώρο");
        submit.Click += async (_, _) => await SubmitQuickAddItemAsync(
            room,
            typeSpinner,
            newTypeInput,
            conditionSpinner,
            conditionOptions,
            brandInput,
            modelInput,
            serialInput,
            quantityInput,
            notesInput,
            resultText,
            submit);

        root.AddView(CreateAccentCard(Stack(
            CreateSectionHeader("1. Τι βρέθηκε;", "Διάλεξε τύπο αντικειμένου από τη λίστα ή γράψε νέο τύπο αν δεν υπάρχει."),
            CreateFieldLabel(primaryFieldLabel + " *"),
            typeSpinner,
            CreateSmallText("Αν ο τύπος υπάρχει στη λίστα, διάλεξέ τον. Αν όχι, συμπλήρωσε το επόμενο πεδίο."),
            newTypeInput,
            CreateDivider(),
            CreateSectionHeader("2. Κατάσταση", "Η λειτουργική κατάσταση είναι ξεχωριστή από το ότι το αντικείμενο είναι προς έλεγχο."),
            CreateFieldLabel(conditionLabel + " *"),
            conditionSpinner,
            CreateSmallText(reviewFlagText),
            CreateDivider(),
            CreateSectionHeader("3. Βασικά στοιχεία", "Συμπλήρωσε όσα ξέρεις τώρα. Τα υπόλοιπα διορθώνονται μετά από το web app."),
            brandInput,
            modelInput,
            serialInput,
            CreateFieldLabel(quantityLabel),
            quantityInput,
            CreateSmallText(quantityHelpText),
            CreateDivider(),
            CreateSectionHeader("4. Σημείωση", "Προαιρετικά γράψε πού/πώς βρέθηκε ή τι χρειάζεται έλεγχο."),
            notesInput,
            submit,
            resultText), Green));

        root.AddView(CreateInfoStrip(
            "Μετά την καταχώρηση",
            "Το νέο αντικείμενο θα εμφανιστεί στο web app στα Νέα ευρήματα, ώστε να γίνει έλεγχος στοιχείων και εκτύπωση QR."));

        root.AddView(CreateBottomNav("scan"));
        SetContentView(WrapInScrollView(root));
    }

    private async Task SubmitQuickAddItemAsync(
        RoomSessionDto room,
        Spinner typeSpinner,
        EditText newTypeInput,
        Spinner conditionSpinner,
        List<QuickAddConditionOptionDto> conditionOptions,
        EditText brandInput,
        EditText modelInput,
        EditText serialInput,
        EditText quantityInput,
        EditText notesInput,
        TextView resultText,
        Button submitButton)
    {
        var selectedType = typeSpinner.SelectedItem?.ToString()?.Trim() ?? string.Empty;
        var newType = newTypeInput.Text?.Trim() ?? string.Empty;
        var itemType = string.IsNullOrWhiteSpace(newType) ? selectedType : newType;

        if (string.IsNullOrWhiteSpace(itemType))
        {
            resultText.Text = "Διάλεξε ή γράψε τύπο αντικειμένου.";
            resultText.SetTextColor(Amber);
            return;
        }

        var quantity = 1;
        if (!string.IsNullOrWhiteSpace(quantityInput.Text) &&
            (!int.TryParse(quantityInput.Text.Trim(), out quantity) || quantity <= 0))
        {
            resultText.Text = "Η ποσότητα πρέπει να είναι θετικός αριθμός.";
            resultText.SetTextColor(Amber);
            return;
        }

        var conditionIndex = Math.Max(0, conditionSpinner.SelectedItemPosition);
        var conditionValue = conditionOptions.Count > conditionIndex
            ? conditionOptions[conditionIndex].Value
            : 2;

        HideKeyboard(newTypeInput);
        HideKeyboard(brandInput);
        HideKeyboard(modelInput);
        HideKeyboard(serialInput);
        HideKeyboard(quantityInput);
        HideKeyboard(notesInput);

        submitButton.Enabled = false;
        resultText.Text = "Αποστολή νέου αντικειμένου...";
        resultText.SetTextColor(Muted);

        try
        {
            var request = new MobileQuickAddItemRequest
            {
                Name = itemType,
                CategoryName = itemType,
                Brand = CleanOptionalText(brandInput.Text),
                Model = CleanOptionalText(modelInput.Text),
                SerialNumber = CleanOptionalText(serialInput.Text),
                Quantity = quantity,
                Condition = conditionValue,
                Notes = CleanOptionalText(notesInput.Text)
            };

            var response = await _api.PostQuickAddItemAsync(room.Id, request);

            if (response?.Ok == true)
            {
                resultText.Text = response.Message;
                resultText.SetTextColor(Green);

                await Task.Delay(650);
                await ShowRoomSessionAsync(room);
                return;
            }

            resultText.Text = response?.Message ?? "Δεν ελήφθη απάντηση από τον server.";
            resultText.SetTextColor(response?.Locked == true ? Amber : Red);
            submitButton.Enabled = true;
        }
        catch (Exception ex)
        {
            resultText.Text = $"Σφάλμα: {ex.Message}";
            resultText.SetTextColor(Red);
            submitButton.Enabled = true;
        }
    }

    private static string? CleanOptionalText(string? value, string? fallback = null)
    {
        var cleaned = value?.Trim();

        if (string.IsNullOrWhiteSpace(cleaned))
        {
            return fallback;
        }

        return cleaned;
    }

    private QuickAddOptionsResponse CreateFallbackQuickAddOptions()
    {
        return new QuickAddOptionsResponse
        {
            Ok = false,
            DefaultCategoryName = "Προς έλεγχο",
            QuantityDefault = 1,
            QuantityLabel = "Ποσότητα (συνήθως 1)",
            Categories = new List<QuickAddCategoryOptionDto>
            {
                new() { Id = 0, Name = "Προς έλεγχο" }
            },
            Conditions = new List<QuickAddConditionOptionDto>
            {
                new() { Value = 0, Name = "Working", Label = "Λειτουργικό" },
                new() { Value = 1, Name = "NotWorking", Label = "Μη λειτουργικό" },
                new() { Value = 2, Name = "NeedsCheck", Label = "Άγνωστο / Προς έλεγχο" }
            },
            Guidance = new QuickAddGuidanceDto()
        };
    }

    private static int FindDefaultTypeIndex(List<string> typeNames, string defaultName)
    {
        var index = typeNames.FindIndex(x => string.Equals(x, defaultName, StringComparison.OrdinalIgnoreCase));

        return index >= 0 ? index : 0;
    }

    private static int FindDefaultConditionIndex(List<QuickAddConditionOptionDto> conditions)
    {
        var index = conditions.FindIndex(x =>
            x.Name.Contains("Needs", StringComparison.OrdinalIgnoreCase) ||
            x.Name.Contains("Check", StringComparison.OrdinalIgnoreCase) ||
            x.Label.Contains("Άγνω", StringComparison.OrdinalIgnoreCase) ||
            x.Label.Contains("έλεγχο", StringComparison.OrdinalIgnoreCase));

        return index >= 0 ? index : Math.Min(conditions.Count - 1, 0);
    }

    private ExpectedItemDto? FindAlreadyScannedExpectedItem(List<ExpectedItemDto> expectedItems, string scannedValue)
    {
        if (expectedItems.Count == 0 || string.IsNullOrWhiteSpace(scannedValue))
        {
            return null;
        }

        return expectedItems.FirstOrDefault(item =>
            item.Scanned &&
            QrValueMatchesItemCode(scannedValue, item.Code));
    }

    private static bool QrValueMatchesItemCode(string scannedValue, string itemCode)
    {
        if (string.IsNullOrWhiteSpace(scannedValue) || string.IsNullOrWhiteSpace(itemCode))
        {
            return false;
        }

        var normalizedScanned = NormalizeQrCodeCandidate(scannedValue);
        var normalizedCode = NormalizeQrCodeCandidate(itemCode);

        return normalizedScanned == normalizedCode ||
               scannedValue.Contains(itemCode, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeQrCodeCandidate(string value)
    {
        var candidate = (value ?? string.Empty).Trim();

        if (Uri.TryCreate(candidate, UriKind.Absolute, out var uri))
        {
            candidate = uri.Segments.LastOrDefault()?.Trim('/') ?? candidate;
        }

        var questionMarkIndex = candidate.IndexOf('?');
        if (questionMarkIndex >= 0)
        {
            candidate = candidate[..questionMarkIndex];
        }

        var hashIndex = candidate.IndexOf('#');
        if (hashIndex >= 0)
        {
            candidate = candidate[..hashIndex];
        }

        return candidate.Trim().Trim('/').ToUpperInvariant();
    }

    private void ShowAlreadyScannedDialog(ExpectedItemDto item)
    {
        var message = string.IsNullOrWhiteSpace(item.Name)
            ? "Το αντικείμενο έχει ήδη σαρωθεί σε αυτόν τον χώρο."
            : $"Το αντικείμενο έχει ήδη σαρωθεί σε αυτόν τον χώρο.\n\n{item.Name}\n{item.Code}";

        ShowAlreadyScannedDialog(message);
    }

    private void ShowAlreadyScannedDialog(string message)
    {
        RunOnUiThread(() =>
        {
            using var dialog = new AlertDialog.Builder(this)
                .SetTitle("Ήδη σκαναρισμένο")
                .SetMessage(string.IsNullOrWhiteSpace(message)
                    ? "Το αντικείμενο έχει ήδη σαρωθεί σε αυτόν τον χώρο."
                    : message)
                .SetPositiveButton("OK", (_, _) => { })
                .Create();

            dialog.Show();
        });
    }

    private void ApplyScanResultMessage(ScanResponse? scanResponse, TextView resultText)
    {
        if (scanResponse == null)
        {
            resultText.Text = "Δεν ελήφθη απάντηση από τον server.";
            resultText.SetTextColor(Amber);
            return;
        }

        resultText.Text = scanResponse.Message;

        if (scanResponse.AlreadyScanned)
        {
            resultText.SetTextColor(Amber);
            ShowAlreadyScannedDialog(scanResponse.Message);
            return;
        }

        resultText.SetTextColor(scanResponse.Ok ? Green : Red);
    }

    private async Task StartGoogleCodeScanAsync(RoomSessionDto room, TextView resultText, List<ExpectedItemDto> expectedItems)
    {
        resultText.Text = "Άνοιγμα κάμερας QR scanner...";
        resultText.SetTextColor(Muted);

        try
        {
            using var scanner = new GoogleCodeScanner(this);
            var rawValue = await scanner.ScanAsync();

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                resultText.Text = "Η σάρωση ακυρώθηκε.";
                resultText.SetTextColor(Amber);
                return;
            }

            var alreadyScannedItem = FindAlreadyScannedExpectedItem(expectedItems, rawValue);
            if (alreadyScannedItem != null)
            {
                ShowAlreadyScannedDialog(alreadyScannedItem);
                resultText.Text = $"Το αντικείμενο έχει ήδη σαρωθεί: {alreadyScannedItem.Name}";
                resultText.SetTextColor(Amber);
                return;
            }

            resultText.Text = $"Σαρώθηκε: {rawValue}\nΑποστολή στον server...";

            var scanResponse = await _api.PostScanAsync(room.Id, rawValue);
            ApplyScanResultMessage(scanResponse, resultText);

            await Task.Delay(scanResponse?.AlreadyScanned == true ? 1600 : 650);
            await ShowRoomSessionAsync(room);
        }
        catch (Exception ex)
        {
            resultText.Text =
                "Δεν ολοκληρώθηκε η σάρωση. Αν είναι η πρώτη φορά, περίμενε λίγο και ξαναδοκίμασε.\n\n" +
                ex.Message;
            resultText.SetTextColor(Red);
        }
    }

    private void ShowHelp()
    {
        _screen = "help";

        var root = CreateLinearRoot();
        root.AddView(CreateScreenHeader("Βοήθεια", "Πώς δουλεύει η εφαρμογή scanner.", "?", Amber));

        root.AddView(CreateCard(Stack(
            CreateSectionHeader("Ροή χρήσης", "Τρία απλά βήματα για απογραφή."),
            CreateStep("1", "Επιλέγεις φάκελο απογραφής."),
            CreateStep("2", "Επιλέγεις χώρο."),
            CreateStep("3", "Σκανάρεις QR ή περνάς κωδικό χειροκίνητα."))));

        root.AddView(CreateInfoStrip("Σημαντικό", "Η δημιουργία φακέλων, QR labels και οι αναφορές γίνονται από το web app."));
        root.AddView(CreateBottomNav("help"));

        SetContentView(WrapInScrollView(root));
    }

    private void ShowLoading(string title, string message)
    {
        RunOnUiThread(() =>
        {
            var root = CreateLinearRoot();
            root.AddView(CreateScreenHeader(title, message, "…", Cyan));
            var progress = new ProgressBar(this) { Indeterminate = true };
            progress.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, Dp(52));
            root.AddView(CreateCard(Stack(CreateBodyText("Παρακαλώ περίμενε..."), progress)));
            SetContentView(WrapInScrollView(root));
        });
    }

    private void ShowError(string title, string message, Action backAction)
    {
        RunOnUiThread(() =>
        {
            var root = CreateLinearRoot();
            root.AddView(CreateScreenHeader(title, "Κάτι δεν πήγε καλά.", "!", Red));

            var back = CreatePrimaryButton("Πίσω");
            back.Click += (_, _) => backAction();

            root.AddView(CreateAccentCard(Stack(
                CreateTitleText("Σφάλμα", 22),
                CreateBodyText(message),
                back), Red));

            SetContentView(WrapInScrollView(root));
        });
    }

    private void SaveServerUrl()
    {
        if (_serverUrlInput != null)
        {
            _settings.ServerUrl = _serverUrlInput.Text ?? string.Empty;
        }
    }

    private LinearLayout CreateLinearRoot()
    {
        var root = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical
        };

        root.SetBackgroundColor(Background);
        root.LayoutParameters = MatchWrap();
        root.SetPadding(Dp(16), Dp(16), Dp(16), Dp(20));
        return root;
    }

    private ScrollView WrapInScrollView(View content)
    {
        var scroll = new ScrollView(this)
        {
            FillViewport = true
        };

        scroll.SetBackgroundColor(Background);
        scroll.AddView(content);
        return scroll;
    }

    private View CreateAppHero(string title, string subtitle, string server, string icon, Color accent)
    {
        var iconView = CreateIconBox(icon, accent, 74, 30);
        var titleView = CreateTitleText(title, 28);
        titleView.Gravity = GravityFlags.Center;
        var subtitleView = CreateBodyText(subtitle);
        subtitleView.Gravity = GravityFlags.Center;

        var status = CreatePill(server, Green);

        return CreateAccentCard(StackCentered(iconView, titleView, subtitleView, status), accent);
    }

    private View CreateScreenHeader(string title, string subtitle, string icon, Color accent)
    {
        var iconView = CreateIconBox(icon, accent, 54, 23);
        var texts = Stack(CreateTitleText(title, 24), CreateBodyText(subtitle));
        var row = CreateRow(iconView, texts, 70);
        return CreateAccentCard(row, accent);
    }

    private View CreateTile(string title, string subtitle, string icon, Color accent, Action action)
    {
        var layout = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical,
            Clickable = true
        };

        layout.SetPadding(Dp(14), Dp(14), Dp(14), Dp(14));
        layout.SetBackgroundDrawable(MakeStroke(Color.ParseColor("#0B1424"), accent, Dp(22), 1));

        layout.AddView(CreateIconBox(icon, accent, 44, 20));
        layout.AddView(CreateTitleText(title, 16));
        layout.AddView(CreateSmallText(subtitle));

        layout.Click += (_, _) => action();

        return layout;
    }

    private LinearLayout CreateTileGrid(params View[] tiles)
    {
        var container = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical
        };

        for (var i = 0; i < tiles.Length; i += 2)
        {
            var row = new LinearLayout(this)
            {
                Orientation = Orientation.Horizontal
            };

            row.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            {
                BottomMargin = Dp(10)
            };

            for (var j = i; j < i + 2 && j < tiles.Length; j++)
            {
                tiles[j].LayoutParameters = new LinearLayout.LayoutParams(0, Dp(142), 1)
                {
                    RightMargin = Dp(j % 2 == 0 ? 8 : 0),
                    LeftMargin = Dp(j % 2 == 1 ? 8 : 0)
                };
                row.AddView(tiles[j]);
            }

            container.AddView(row);
        }

        return container;
    }

    private View CreateFolderCardContent(string title, string subtitle)
    {
        return Stack(CreateTitleText(title, 18), CreateBodyText(subtitle));
    }

    private LinearLayout Stack(params View[] views)
    {
        var layout = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical
        };

        foreach (var view in views)
        {
            layout.AddView(view);
        }

        return layout;
    }

    private LinearLayout StackCentered(params View[] views)
    {
        var layout = Stack(views);
        layout.SetGravity(GravityFlags.CenterHorizontal);
        return layout;
    }

    private View CreateStatusCard(string title, TextView content)
    {
        return CreateCard(Stack(CreateSectionHeader(title, "Η εφαρμογή δείχνει εδώ το αποτέλεσμα."), content));
    }

    private View CreateSectionHeader(string title, string subtitle)
    {
        return Stack(CreateTitleText(title, 20), CreateBodyText(subtitle));
    }

    private TextView CreateSectionTitle(string text)
    {
        return CreateTitleText(text, 20).WithMargins(2, 8, 2, 10);
    }

    private View CreateEmptyState(string title, string message)
    {
        return CreateAccentCard(Stack(CreatePill("EMPTY", Amber), CreateTitleText(title, 21), CreateBodyText(message)), Amber);
    }

    private View CreateInfoStrip(string title, string message)
    {
        return CreateAccentCard(Stack(CreateSmallAccentText(title, Cyan), CreateBodyText(message)), Cyan);
    }

    private View CreateStep(string number, string text)
    {
        var num = CreateIconBox(number, Cyan, 38, 17);
        var body = CreateBodyText(text);
        return CreateRow(num, body, 52).WithMargins(0, 0, 0, 8);
    }

    private View CreateItemCard(ExpectedItemDto item)
    {
        var title = CreateTitleText($"{(item.Scanned ? "✓ " : "")}{item.Name}", 16);
        title.SetTextColor(item.Scanned ? Green : Text);

        var code = CreateSmallAccentText(item.Code, Cyan);

        var details = CreateBodyText(string.Join(" · ", new[]
        {
            item.BrandModel,
            item.CategoryName,
            item.SerialNumber
        }.Where(x => !string.IsNullOrWhiteSpace(x))));

        return CreateAccentCard(Stack(title, code, details), item.Scanned ? Green : Line, item.Scanned ? Color.ParseColor("#0D2A24") : Surface2);
    }

    private View CreateStatsRow((string Label, string Value, Color Color)[] stats)
    {
        var row = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal
        };

        row.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
        {
            BottomMargin = Dp(10)
        };

        foreach (var stat in stats)
        {
            var box = Stack(CreateSmallAccentText(stat.Value, stat.Color), CreateTinyText(stat.Label));
            box.SetPadding(Dp(9), Dp(8), Dp(9), Dp(8));
            box.SetBackgroundDrawable(MakeSolid(Color.ParseColor("#0B1424"), Dp(15)));
            box.LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1)
            {
                RightMargin = Dp(6)
            };

            row.AddView(box);
        }

        return row;
    }

    private View CreateProgressBar(int value, int total, Color accent)
    {
        var pct = total <= 0 ? 0 : Math.Clamp((int)Math.Round(value * 100d / total), 0, 100);
        var container = Stack();

        var label = CreateSmallAccentText($"{pct}% πρόοδος", accent);
        container.AddView(label);

        var outer = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal
        };

        outer.SetPadding(Dp(2), Dp(2), Dp(2), Dp(2));
        outer.SetBackgroundDrawable(MakeSolid(Color.ParseColor("#0B1424"), Dp(999)));
        outer.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, Dp(14))
        {
            BottomMargin = Dp(10)
        };

        var inner = new TextView(this);
        inner.SetBackgroundDrawable(MakeSolid(accent, Dp(999)));
        inner.LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.MatchParent, Math.Max(1, pct));

        var spacer = new TextView(this);
        spacer.LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.MatchParent, Math.Max(1, 100 - pct));

        outer.AddView(inner);
        outer.AddView(spacer);

        container.AddView(outer);
        return container;
    }

    private TextView CreatePill(string text, Color color)
    {
        var pill = new TextView(this)
        {
            Text = text,
            TextSize = 10,
            Typeface = Typeface.DefaultBold,
            Gravity = GravityFlags.Center
        };

        pill.SetTextColor(color);
        pill.SetPadding(Dp(10), Dp(5), Dp(10), Dp(5));
        pill.SetBackgroundDrawable(MakeStroke(Color.ParseColor("#0B1424"), color, Dp(999), 1));
        pill.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
        {
            BottomMargin = Dp(8)
        };

        return pill;
    }

    private TextView CreateIconBox(string icon, Color accent, int sizeDp, int textSize)
    {
        var box = new TextView(this)
        {
            Text = icon,
            TextSize = textSize,
            Typeface = Typeface.DefaultBold,
            Gravity = GravityFlags.Center
        };

        box.SetTextColor(accent);
        box.SetBackgroundDrawable(MakeStroke(Color.ParseColor("#111B31"), accent, Dp(18), 1));
        box.LayoutParameters = new LinearLayout.LayoutParams(Dp(sizeDp), Dp(sizeDp))
        {
            BottomMargin = Dp(10)
        };

        return box;
    }

    private LinearLayout CreateBottomNav(string active)
    {
        var nav = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal
        };

        nav.SetPadding(Dp(6), Dp(8), Dp(6), Dp(8));
        nav.SetBackgroundDrawable(MakeStroke(Color.ParseColor("#08111F"), Line, Dp(24), 1));
        nav.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
        {
            TopMargin = Dp(8),
            BottomMargin = Dp(4)
        };

        nav.AddView(CreateNavButton("Αρχική", "home", active, () => ShowHome()));
        nav.AddView(CreateNavButton("Φάκελοι", "folders", active, async () => await ShowFoldersAsync()));
        nav.AddView(CreateNavButton("Σάρωση", "scan", active, async () => await ShowFoldersAsync()));
        nav.AddView(CreateNavButton("Ρυθμίσεις", "settings", active, () => ShowSettings()));

        return nav;
    }

    private TextView CreateNavButton(string text, string key, string active, Action action)
    {
        var item = new TextView(this)
        {
            Text = text,
            TextSize = 12,
            Gravity = GravityFlags.Center,
            Typeface = key == active ? Typeface.DefaultBold : Typeface.Default
        };

        item.SetTextColor(key == active ? Cyan : Muted);
        item.SetPadding(Dp(4), Dp(10), Dp(4), Dp(10));
        item.Clickable = true;
        item.Click += (_, _) => action();
        item.LayoutParameters = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1);

        return item;
    }

    private LinearLayout CreateRow(View left, View right, int leftWidthDp = 0)
    {
        var row = new LinearLayout(this)
        {
            Orientation = Orientation.Horizontal
        };

        row.SetGravity(GravityFlags.CenterVertical);

        left.LayoutParameters = leftWidthDp > 0
            ? new LinearLayout.LayoutParams(Dp(leftWidthDp), ViewGroup.LayoutParams.WrapContent)
            : new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1);

        right.LayoutParameters = leftWidthDp > 0
            ? new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1)
            : new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

        row.AddView(left);
        row.AddView(right);
        return row;
    }

    private View CreateDivider()
    {
        var divider = new View(this);
        divider.SetBackgroundColor(Line);
        divider.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, Dp(1))
        {
            TopMargin = Dp(8),
            BottomMargin = Dp(12)
        };
        return divider;
    }

    private LinearLayout CreateCard(View content)
    {
        return CreateAccentCard(content, Line, Surface2);
    }

    private LinearLayout CreateAccentCard(View content, Color accent)
    {
        return CreateAccentCard(content, accent, Surface2);
    }

    private LinearLayout CreateAccentCard(View content, Color accent, Color fill)
    {
        var card = new LinearLayout(this)
        {
            Orientation = Orientation.Vertical
        };

        card.SetPadding(Dp(16), Dp(16), Dp(16), Dp(16));
        card.SetBackgroundDrawable(MakeStroke(fill, accent, Dp(24), 1));
        card.LayoutParameters = CardLayout();
        card.AddView(content);
        return card;
    }

    private TextView CreateTitleText(string text, int sp)
    {
        return new TextView(this)
        {
            Text = text,
            TextSize = sp,
            Typeface = Typeface.DefaultBold
        }.WithTextColor(Text).WithMargins(0, 0, 0, 8);
    }

    private TextView CreateBodyText(string text)
    {
        return new TextView(this)
        {
            Text = text,
            TextSize = 15
        }.WithTextColor(Muted).WithMargins(0, 0, 0, 10);
    }

    private TextView CreateSmallText(string text)
    {
        return new TextView(this)
        {
            Text = text,
            TextSize = 12
        }.WithTextColor(Muted).WithMargins(0, 0, 0, 6);
    }

    private TextView CreateTinyText(string text)
    {
        return new TextView(this)
        {
            Text = text,
            TextSize = 10
        }.WithTextColor(Muted).WithMargins(0, 0, 0, 0);
    }

    private TextView CreateSmallAccentText(string text, Color color)
    {
        return new TextView(this)
        {
            Text = text,
            TextSize = 13,
            Typeface = Typeface.DefaultBold
        }.WithTextColor(color).WithMargins(0, 0, 0, 6);
    }


    private TextView CreateFieldLabel(string text)
    {
        return new TextView(this)
        {
            Text = text,
            TextSize = 13,
            Typeface = Typeface.DefaultBold
        }.WithTextColor(Text).WithMargins(0, 4, 0, 6);
    }

    private Spinner CreateSpinner(List<string> values, int selectedIndex = 0)
    {
        var adapter = new ArrayAdapter<string>(
            this,
            Android.Resource.Layout.SimpleSpinnerItem,
            values);

        adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

        var spinner = new Spinner(this)
        {
            Adapter = adapter
        };

        spinner.SetBackgroundDrawable(MakeStroke(Surface3, Line, Dp(18), 1));
        spinner.SetPadding(Dp(12), 0, Dp(12), 0);
        spinner.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, Dp(54))
        {
            BottomMargin = Dp(10)
        };

        if (values.Count > 0)
        {
            spinner.SetSelection(Math.Clamp(selectedIndex, 0, values.Count - 1));
        }

        return spinner;
    }

    private EditText CreateEditText(string hint)
    {
        var input = new EditText(this)
        {
            Hint = hint
        };

        input.SetSingleLine(true);
        input.SetTextColor(Text);
        input.SetHintTextColor(Muted);
        input.SetBackgroundDrawable(MakeStroke(Surface3, Line, Dp(18), 1));
        input.SetPadding(Dp(12), 0, Dp(12), 0);
        input.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, Dp(54))
        {
            BottomMargin = Dp(10)
        };

        return input;
    }

    private Button CreatePrimaryButton(string text)
    {
        var button = new Button(this)
        {
            Text = text
        };

        button.SetAllCaps(false);
        button.SetTextColor(DarkText);
        button.SetTypeface(Typeface.DefaultBold, TypefaceStyle.Bold);
        button.SetBackgroundDrawable(MakeSolid(Cyan, Dp(18)));
        button.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, Dp(54))
        {
            BottomMargin = Dp(10)
        };

        return button;
    }

    private Button CreateSecondaryButton(string text)
    {
        var button = new Button(this)
        {
            Text = text
        };

        button.SetAllCaps(false);
        button.SetTextColor(Text);
        button.SetBackgroundDrawable(MakeStroke(Surface3, Line, Dp(18), 1));
        button.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, Dp(54))
        {
            BottomMargin = Dp(10)
        };

        return button;
    }

    private GradientDrawable MakeSolid(Color color, int radius)
    {
        var drawable = new GradientDrawable();
        drawable.SetShape(ShapeType.Rectangle);
        drawable.SetColor(color);
        drawable.SetCornerRadius(radius);
        return drawable;
    }

    private GradientDrawable MakeStroke(Color fill, Color stroke, int radius, int strokeDp)
    {
        var drawable = MakeSolid(fill, radius);
        drawable.SetStroke(Dp(strokeDp), stroke);
        return drawable;
    }

    private LinearLayout.LayoutParams MatchWrap()
    {
        return new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
    }

    private LinearLayout.LayoutParams CardLayout()
    {
        return new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
        {
            BottomMargin = Dp(12)
        };
    }

    private int Dp(int value)
    {
        return (int)(value * Resources.DisplayMetrics!.Density + 0.5f);
    }

    private void HideKeyboard(View view)
    {
        var imm = (InputMethodManager?)GetSystemService(InputMethodService);
        imm?.HideSoftInputFromWindow(view.WindowToken, 0);
    }
}

internal static class ViewExtensions
{
    public static T WithTextColor<T>(this T view, Color color) where T : TextView
    {
        view.SetTextColor(color);
        return view;
    }

    public static T WithMargins<T>(this T view, int left, int top, int right, int bottom) where T : View
    {
        var density = view.Resources?.DisplayMetrics?.Density ?? 1;
        int Dp(int value) => (int)(value * density + 0.5f);

        view.LayoutParameters = new LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.MatchParent,
            ViewGroup.LayoutParams.WrapContent)
        {
            LeftMargin = Dp(left),
            TopMargin = Dp(top),
            RightMargin = Dp(right),
            BottomMargin = Dp(bottom)
        };

        return view;
    }
}
