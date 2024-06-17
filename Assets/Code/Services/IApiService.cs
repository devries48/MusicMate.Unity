using System;
using UnityEngine;

public interface IApiService
{
    void SignIn(string user, string password);
    void GetInitialReleases(Action<PagedResult<ReleaseResult>> callback);
    void GetRelease(Guid id, Action<ReleaseModel> callback);
 
    void DownloadImage(string url, Action<Sprite> callback);
    void IsFolderImportRunning(Action<bool> callback);
    void FolderImportStart(Action<string> callback);

    void SubscribeToConnectionChanged(ConnectionChangedEventHandler handler);
    void UnsubscribeFromConnectionChanged(ConnectionChangedEventHandler handler);
}

#region EventHandlers & EventArgs
public delegate void ConnectionChangedEventHandler(object sender, ConnectionChangedEventArgs e);

public class ConnectionChangedEventArgs : EventArgs
{
    public ConnectionChangedEventArgs(bool connected, string error = default)
    {
        Connected = connected;
        Error = error;
    }

    public bool Connected { get; private set; }

    public string Error { get; private set; }
}
#endregion