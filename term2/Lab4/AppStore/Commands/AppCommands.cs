using System;
using AppStore.Models;
using AppStore.Services;

namespace AppStore.Commands
{
    /// <summary>
    /// Toggles install/uninstall state of an app and persists the change.
    /// </summary>
    public class DownloadAppCommand : RelayCommand<AppItem>
    {
        public DownloadAppCommand(IAppService service, Action<AppItem?>? afterExecute = null)
            : base(app =>
            {
                if (app == null)
                    return;
                if (app.IsDownloaded)
                    service.Uninstall(app.Id);
                else
                    service.Download(app.Id);
                afterExecute?.Invoke(app);
            }) { }
    }

    /// <summary>
    /// Permanently removes an app by Id.
    /// </summary>
    public class DeleteAppCommand : RelayCommand<Guid>
    {
        public DeleteAppCommand(IAppService service, Action? afterExecute = null)
            : base(id =>
            {
                if (id == Guid.Empty)
                    return;
                service.Delete(id);
                afterExecute?.Invoke();
            }) { }
    }
}
