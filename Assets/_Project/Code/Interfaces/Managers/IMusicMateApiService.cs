using System;
using UnityEngine;

public interface IMusicMateApiService
{
    void SignIn(string url, string user, string password);
    void GetInitialReleases(Action<PagedResultOld<ReleaseResult>> callback);
    void GetRelease(Guid id, Action<ReleaseModel> callback);
    void GetArtist(Guid id, Action<ArtistModel> callback);
 
    void DownloadImage(string url, Action<Sprite> callback);
    void GetFolderImport(int pageNumber, int pageSize, Action<PagedResult<ImportReleaseResult>> callback);

    void SubscribeToConnectionChanged(ConnectionChangedEventHandler handler);
    void UnsubscribeFromConnectionChanged(ConnectionChangedEventHandler handler);
    void SubscribeToApiError(ApiErrorEventHandler handler);
    void UnsubscribeFromApiError(ApiErrorEventHandler handler);
}

#region EventHandlers & EventArgs
public delegate void ConnectionChangedEventHandler(ConnectionChangedEventArgs e);
public delegate void ApiErrorEventHandler(ErrorEventArgs e);

public class ConnectionChangedEventArgs
{
    public ConnectionChangedEventArgs(bool connected, string error = default)
    {
        Connected = connected;
        Error = error;
    }

    public bool Connected { get; private set; }

    public string Error { get; private set; }
}

public class ErrorEventArgs
{
    public ErrorEventArgs(ErrorType error, string message, string description = "")
    {
        Error = error;
        Message = message;
        Description = description;
    }

    public ErrorType Error { get; private set; }
    public string Message { get; private set; }
    public string Description { get; private set; }
}
#endregion