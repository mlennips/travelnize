using LIT.Travelnize.Interfaces;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace LIT.Travelnize.Services
{
    public class NotificationService(ISnackbar snackbar, IStringLocalizer<App> localizer) : INotificationService
    {
        public void Success(string message, params object[] args)
            => AddNotification(message, args, Severity.Success);

        public void Error(string message, params object[] args)
            => AddNotification(message, args, Severity.Error);

        public void Info(string message, params object[] args)
            => AddNotification(message, args, Severity.Info);

        private void AddNotification(string message, object[] args, Severity severity)
        {
            var localizedArgs = LocalizeArgs(args);
            snackbar.Add(
                localizedArgs.Length > 0
                    ? localizer[message, localizedArgs]
                    : localizer[message],
                severity, config =>
                {
                    config.ShowCloseIcon = true;
                    config.RequireInteraction = severity == Severity.Error;
                    config.VisibleStateDuration = severity == Severity.Error ? 5000 : 3000;
                    config.BackgroundBlurred = true;
                    config.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
                    config.HideTransitionDuration = 400;
                }
            );
        }

        private object[] LocalizeArgs(object[]? args)
        {
            if (args is null || args.Length == 0) return Array.Empty<object>();
            var localizedArgs = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                localizedArgs[i] = args[i] is string s ? localizer[s] : args[i];
            }
            return localizedArgs;
        }
    }
}
