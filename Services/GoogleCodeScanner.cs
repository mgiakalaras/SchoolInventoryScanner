using Android.Content;
using Android.Runtime;
using Gms = Android.Gms.Tasks;
using Xamarin.Google.MLKit.Vision.Barcode.Common;
using Xamarin.Google.MLKit.Vision.CodeScanner;

namespace SchoolInventoryScanner.Services;

public sealed class GoogleCodeScanner : IDisposable
{
    private readonly IGmsBarcodeScanner _barcodeScanner;

    public GoogleCodeScanner(Context context)
    {
        var options = new GmsBarcodeScannerOptions.Builder()
            .AllowManualInput()
            .EnableAutoZoom()
            .SetBarcodeFormats(Barcode.FormatAllFormats)
            .Build();

        // Important:
        // Use the current Activity context, not Application.Context.
        // The scanner opens a Google Play services UI, so it needs an Activity-backed context.
        _barcodeScanner = GmsBarcodeScanning.GetClient(context, options);
    }

    public async Task<string?> ScanAsync()
    {
        var completion = new TaskCompletionSource<string?>();

        using var listener = new BarcodeCompleteListener(completion);
        using var task = _barcodeScanner.StartScan().AddOnCompleteListener(listener);

        return await completion.Task;
    }

    public void Dispose()
    {
        _barcodeScanner.Dispose();
    }

    private sealed class BarcodeCompleteListener : Java.Lang.Object, Gms.IOnCompleteListener
    {
        private readonly TaskCompletionSource<string?> _completion;

        public BarcodeCompleteListener(TaskCompletionSource<string?> completion)
        {
            _completion = completion;
        }

        public void OnComplete(Gms.Task task)
        {
            if (task.IsSuccessful)
            {
                var barcode = task.Result?.JavaCast<Barcode>();
                _completion.TrySetResult(barcode?.RawValue ?? barcode?.DisplayValue);
                return;
            }

            if (task.IsCanceled)
            {
                _completion.TrySetResult(null);
                return;
            }

            var message = task.Exception?.Message ?? "Η σάρωση απέτυχε.";
            _completion.TrySetException(new InvalidOperationException(message));
        }
    }
}
