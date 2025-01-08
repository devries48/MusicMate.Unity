using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Provides an implementation of the <see cref="IMusicMateApiService"/> interface, 
/// managing communication with the MusicMate API. 
/// This service handles user authentication, data retrieval, image downloads, 
/// and folder import operations.
/// 
/// Features include:
/// - API connection management with an event-driven connection status system.
/// - Retrieval of releases and specific album details.
/// - Support for downloading images as Unity Sprites.
/// - Folder import process management, including checks for running imports.
/// 
/// **Design Patterns:**
/// - **Singleton Pattern:** Inherits from <see cref="SceneSingleton{T}"/> to ensure only 
///   one instance of the service exists in the scene.
/// - **Observer Pattern:** Implements a connection status event mechanism to notify 
///   subscribers of changes in API connectivity.
///
/// **Best Practices:**
/// - **Encapsulation of API Logic:** The class centralizes API communication logic, 
///   making it easier to maintain and test.
/// - **Async Operations with Coroutines:** Uses Unity coroutines for asynchronous 
///   operations, ensuring compatibility with Unity's main thread.
/// - **Error Handling:** Implements basic error handling for network requests, 
///   logging errors and invoking callbacks with appropriate responses.
/// - **Separation of Concerns:** Keeps HTTP request creation and API-specific logic 
///   (e.g., parsing JSON) distinct from UI logic.
/// 
/// **Extensibility:**
/// - Supports dynamic extension through interfaces, enabling flexible testing and 
///   future service replacements.
/// - Leverages event-driven architecture for modular integration with other systems.
/// 
/// **Usage:**
/// This service is designed to be a central hub for API interactions, providing methods 
/// to sign in, retrieve album data, and manage media-related operations. It can be used 
/// as a dependency in other components requiring API access.
/// </summary>
public class MusicMateApiService : SceneSingleton<MusicMateApiService>, IMusicMateApiService
{
    string _apiUrl;
    string _accessToken;

    readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    event ConnectionChangedEventHandler ConnectionChanged;

    public bool IsConnected => _accessToken != null;

    public IMusicMateApiService GetClient() => this;

    public void SubscribeToConnectionChanged(ConnectionChangedEventHandler handler) => ConnectionChanged += handler;

    public void UnsubscribeFromConnectionChanged(ConnectionChangedEventHandler handler) => ConnectionChanged -= handler;

    public void SignIn(string url, string user, string password) => StartCoroutine(PostSignInRequest(url,user, password));

    public void GetInitialReleases(Action<PagedResult<ReleaseResult>> callback) => StartCoroutine(GetReleases(callback));

    public void GetRelease(Guid id, Action<ReleaseModel> callback) => StartCoroutine(GetReleaseCore(id, callback));

    public void DownloadImage(string url, Action<Sprite> callback) => StartCoroutine(GetImage(url, callback));

    public void IsFolderImportRunning(Action<bool> callback) => StartCoroutine(FolderImportRunning(callback));

    public void FolderImportStart(Action<string> callback)
    {
        IsFolderImportRunning(running =>
        {
            if (running)
                callback.Invoke("Folder import process is already running!");
            else
                StartCoroutine(GetFolderImport(callback));
        });
    }

    IEnumerator PostSignInRequest(string url, string user, string password)
    {
        _apiUrl = url;

        var model = JsonUtility.ToJson(new LoginModel { Username = user, Password = password });
        using UnityWebRequest wr = UnityWebRequest.Post(UriCombine("auth/token"), model, "application/json");
        wr.certificateHandler = new CertificateWhore();

        yield return wr.SendWebRequest();

        if (wr.result != UnityWebRequest.Result.Success)
        {
            OnConnectionChanged(false, wr.error);
        }
        else
        {
            var auth = JsonConvert.DeserializeObject<AuthModel>(wr.downloadHandler.text, _jsonSettings);
            _accessToken = auth.Token;

            OnConnectionChanged(true);
        }
    }

    IEnumerator GetReleases(Action<PagedResult<ReleaseResult>> callback)
    {
        var q = CreateQueryString("releases", 50);
        using UnityWebRequest wr = CreateGetRequest(q);
        yield return wr.SendWebRequest();

        if (wr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(wr.error);
        }
        else
        {
            //Debug.Log(wr.downloadHandler.text);
            var result = JsonConvert.DeserializeObject<PagedResult<ReleaseResult>>(
                wr.downloadHandler.text,
                _jsonSettings);

            callback.Invoke(result);
        }
    }

    IEnumerator GetReleaseCore(Guid id, Action<ReleaseModel> callback)
    {
        using UnityWebRequest wr = CreateGetRequest($"releases/{id}");
        yield return wr.SendWebRequest();

        if (wr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Import process started successfully.");
        }
        else
        {
            Debug.Log(wr.downloadHandler.text);
            try
            {
                var result = JsonConvert.DeserializeObject<SingleResult<ReleaseModel>>(wr.downloadHandler.text, _jsonSettings);
                callback.Invoke(result.Data);
            }
            catch (Exception ex)
            {
               print(ex.ToString());
            }

        }
    }

    //TODO Cache images
    IEnumerator GetImage(string url, Action<Sprite> callback)
    {
        var wrt = UnityWebRequestTexture.GetTexture(url);
        yield return wrt.SendWebRequest();

        if (wrt.result != UnityWebRequest.Result.Success)
            Debug.Log(wrt.error);
        else
        {
            Texture2D tex = ((DownloadHandlerTexture)wrt.downloadHandler).texture;
            Sprite sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(tex.width / 2, tex.height / 2));
            callback.Invoke(sprite);
        }
    }

    IEnumerator FolderImportRunning(Action<bool> callback)
    {
        using UnityWebRequest wr = CreateGetRequest("import/scan/status");
        yield return wr.SendWebRequest();

        if (wr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(wr.error);
            callback.Invoke(false);
        }
        else
        {
            var status = JsonUtility.FromJson<ScanStatusResponse>(wr.downloadHandler.text);
            callback.Invoke(status.IsScanRunning);
        }
    }

    IEnumerator GetFolderImport(Action<string> callback)
    {
        using UnityWebRequest www = CreateGetRequest("import/scan/inbound");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Import process started successfully.");
        }
        else
        {
            Debug.LogError("Failed to start the import process.");
        }
    }

    void OnConnectionChanged(bool connected, string error = default)
    {
        var args = new ConnectionChangedEventArgs(connected, error);
        ConnectionChanged?.Invoke(this, args);
    }

    UnityWebRequest CreateGetRequest(string queryString)
    {
        var request = UnityWebRequest.Get(UriCombine(queryString));
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + _accessToken);

        return request;
    }

    string CreateQueryString(string controller, int limit)
    {
        var queryParams = new Dictionary<string, string> {
            {
                "Limit", limit.ToString()
            }
        };

        return controller + "?" + string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={Uri.EscapeDataString(kvp.Value)}"));
    }

    string UriCombine(string uri)
    {
        var uri1 = _apiUrl.TrimEnd('/');
        var uri2 = uri.TrimStart('/');
        return $"{uri1}/{uri2}";
    }

    class ScanStatusResponse
    {
        public bool IsScanRunning = false;
    }

}

public class CertificateWhore : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData) => true;
}