using System.Timers;
using Microsoft.Extensions.Logging;
using VSI.Enums;

namespace VSI.Services
{
    public class ToastService : IDisposable
    {
        private readonly ILogger<ToastService> _logger;
        private System.Timers.Timer? Countdown;

        public ToastService(ILogger<ToastService> logger)
        {
            _logger = logger;
        }

        public event Action<string, ToastLevel, string>? OnShow;
        public event Action? OnHide;

        public void ShowToast(string message, ToastLevel level)
        {
            ShowToast(message, level, true, string.Empty);
        }

        public void ShowToast(string message, ToastLevel level, bool autoClose)
        {
            ShowToast(message, level, autoClose, string.Empty);
        }

        public void ShowToast(string message, ToastLevel level, string alternateHeading)
        {
            ShowToast(message, level, true, alternateHeading);
        }

        public void ShowToast(string message, ToastLevel level, bool autoClose, string alternateHeading)
        {
            _logger.LogInformation("ShowToast: {Message} ({Level})", message, level);
            OnShow?.Invoke(message, level, alternateHeading);

            if (autoClose)
            {
                StartCountdown();
            }
        }

        private void StartCountdown()
        {
            SetCountdown();

            if (Countdown!.Enabled)
            {
                Countdown.Stop();
                Countdown.Start();
            }
            else
            {
                Countdown!.Start();
            }
        }

        private void SetCountdown()
        {
            if (Countdown != null) return;

            Countdown = new System.Timers.Timer(4000);
            Countdown.Elapsed += HideToast;
            Countdown.AutoReset = false;
        }

        private void HideToast(object? source, ElapsedEventArgs args)
        {
            try
            {
                _logger.LogDebug("HideToast triggered");
                OnHide?.Invoke();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ToastService] Fout bij verbergen van toast");
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
