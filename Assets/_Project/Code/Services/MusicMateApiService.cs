using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
    #region Fields & Properties
    string _apiUrl;
    string _accessToken;

    readonly JsonSerializerSettings _jsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };
    
    public bool IsConnected => _accessToken != null;

    public IMusicMateApiService GetClient() => this;
    #endregion
    
    #region  Events
    event ConnectionChangedEventHandler ConnectionChanged;
    event ApiErrorEventHandler ErrorOccurred;
    
    public void SubscribeToConnectionChanged(ConnectionChangedEventHandler handler) => ConnectionChanged += handler;

    public void UnsubscribeFromConnectionChanged(ConnectionChangedEventHandler handler) => ConnectionChanged -= handler;

    public void SubscribeToApiError(ApiErrorEventHandler handler) => ErrorOccurred += handler;

    public void UnsubscribeFromApiError(ApiErrorEventHandler handler) => ErrorOccurred -= handler;

    #endregion
    
    public void SignIn(string url, string user, string password) => StartCoroutine(PostSignInRequest(url, user, password));

    public void GetInitialReleases(Action<PagedResultOld<ReleaseResult>> callback) => StartCoroutine(GetReleases(callback));

    public void GetRelease(Guid id, Action<ReleaseModel> callback) => StartCoroutine(GetReleaseCore(id, callback));

    public void GetArtist(Guid id, Action<ArtistModel> callback) => StartCoroutine(GetArtistCore(id, callback));
    
    public void GetSuggestions(LookupSuggestion lookup, string query, Action<IReadOnlyList<SuggestionResult>> callback)
    {
        var endpoint = $"lookup/{lookup}?query={Uri.EscapeDataString(query)}";
        
        StopCoroutine(GetSuggestionsCore(endpoint, callback));
        StartCoroutine(GetSuggestionsCore(endpoint, callback));
    }

    public void DownloadImage(string url, Action<Sprite> callback) => StartCoroutine(GetImage(url, callback));
    
    public void GetFolderImport(int pageNumber, int pageSize, Action<PagedResult<ImportReleaseResult>> callback)
    {
        var endpoint = $"import/disk/find?pageNumber={pageNumber}&pageSize={pageSize}";
        StartCoroutine(GetFolderImportCore(endpoint, callback));
    }

    IEnumerator PostSignInRequest(string url, string user, string password)
    {
        _apiUrl = url;

        var model = JsonUtility.ToJson(new LoginModel { Username = user, Password = password });
        using UnityWebRequest wr = UnityWebRequest.Post(UriCombine("auth/token"), model, "application/json");
        wr.certificateHandler = new CertificateBypassHandler();

        yield return wr.SendWebRequest();

        if (wr.result != UnityWebRequest.Result.Success)
        {
            InvokeConnectionChanged(false, wr.error);
        }
        else
        {
            var auth = JsonConvert.DeserializeObject<AuthModel>(wr.downloadHandler.text, _jsonSettings);
            _accessToken = auth.Token;

            InvokeConnectionChanged(true);
        }
    }

    IEnumerator GetReleases(Action<PagedResultOld<ReleaseResult>> callback)
    {
        var q = CreateQueryString("releases", 50);
        using var wr = CreateGetRequest(q);
        yield return wr.SendWebRequest();

        if (wr.result != UnityWebRequest.Result.Success)
            InvokeErrorOccurred(q, wr);
        else
        {
            try
            {
                Debug.Log(wr.downloadHandler.text);
                var result = JsonConvert.DeserializeObject<PagedResultOld<ReleaseResult>>(
                    wr.downloadHandler.text,
                    _jsonSettings);

                callback.Invoke(result);
            }
            catch (Exception ex)
            {
                InvokeErrorOccurred(q, ex);
            }
        }
    }

    IEnumerator GetReleaseCore(Guid id, Action<ReleaseModel> callback)
    {
        var q = $"releases/{id}";
        using UnityWebRequest wr = CreateGetRequest(q);
        yield return wr.SendWebRequest();

        if (wr.result != UnityWebRequest.Result.Success)
        {
            InvokeErrorOccurred(q, wr);
            callback.Invoke(null);
        }
        else
        {
            try
            {
                Debug.Log("RELEASE: " + wr.downloadHandler.text);
                var result = JsonConvert.DeserializeObject<SingleResult<ReleaseModel>>(wr.downloadHandler.text, _jsonSettings);
                callback.Invoke(result.Data);
            }
            catch (Exception ex)
            {
                InvokeErrorOccurred(q, ex);
                callback.Invoke(null);
            }
        }
    }

    IEnumerator GetArtistCore(Guid id, Action<ArtistModel> callback)
    {
        using UnityWebRequest wr = CreateGetRequest($"artists/{id}");
        yield return wr.SendWebRequest();

        if (wr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Web request error getting artist.");
        }
        else
        {
            try
            {
                //print(wr.downloadHandler.text);
                var result = JsonConvert.DeserializeObject<SingleResult<ArtistModel>>(wr.downloadHandler.text, _jsonSettings);
                callback.Invoke(result.Data);
            }
            catch (Exception ex)
            {
                print(ex.ToString());
            }
        }
    }
    
    public async Task<List<SuggestionResult>> GetSuggestionsAsync(LookupSuggestion lookup, string query)
    {
        var url = $"https://yourserver/api/lookup/{lookup}?query={Uri.EscapeDataString(query)}";

        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<SuggestionResult>>(json);
    }

    IEnumerator GetSuggestionsCore(string endpoint, Action<IReadOnlyList<SuggestionResult>> callback)
    {
        yield return GetRequest(endpoint, callback);
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
            var tex = ((DownloadHandlerTexture)wrt.downloadHandler).texture;
            var sprite = Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(tex.width / 2, tex.height / 2));
            callback.Invoke(sprite);
        }
    }
    
    private IEnumerator GetFolderImportCore(string endpoint, Action<PagedResult<ImportReleaseResult>> callback)
    {
        yield return GetRequest(endpoint, callback);
    }

    void InvokeConnectionChanged(bool connected, string error = default)
    {
        var args = new ConnectionChangedEventArgs(connected, error);
        ConnectionChanged?.Invoke(args);
    }

    void InvokeErrorOccurred(string q, UnityWebRequest wr)
    {
        var args = new ErrorEventArgs(ErrorType.Api, GetMessage(q), GetDescription(wr));
        ErrorOccurred?.Invoke(args);
    }

    void InvokeErrorOccurred(UnityWebRequest wr)
    {
        var args = new ErrorEventArgs(ErrorType.Api, GetDescription(wr));
        ErrorOccurred?.Invoke(args);
    }
    
    void InvokeErrorOccurred(UnityWebRequest wr, Exception ex)
    {
        var args = new ErrorEventArgs(ErrorType.Api, GetDescription(wr),ex.Message);
        ErrorOccurred?.Invoke(args);
    }
    
    void InvokeErrorOccurred(string q, Exception ex)
    {
        var args = new ErrorEventArgs(ErrorType.Api, GetMessage(q), ex.Message);
        ErrorOccurred?.Invoke(args);
    }

    string GetMessage(string q)
    {
        var parts = q.Split('?');
        var endpoint = parts[0];
        var parameters = parts.Length > 1 ? parts[1] : "";
        var prefix = "Failed to retrieve";
        if (!string.IsNullOrEmpty(parameters))
        {
            parameters = parameters.Replace("&", ", ").Replace("=", ": ");
            return $"{prefix} {endpoint} ({parameters})"; // Example: "releases (limit: 50, sort: desc)"
        }

        return $"{prefix} {endpoint}";
    }

    string GetDescription(UnityWebRequest wr)
    {
        return wr.result switch
        {
            UnityWebRequest.Result.ConnectionError => "Network error: Unable to reach the server. Please check your internet connection.",
            UnityWebRequest.Result.ProtocolError => wr.responseCode switch
            {
                400 => "Bad request: The server could not process the request. Please verify your input.",
                401 => "Unauthorized: Access denied. Please check your credentials.",
                403 => "Forbidden: You do not have permission to access this resource.",
                404 => "Not found: The requested resource could not be found on the server.",
                405 => "Not Allowed: You do not have permission to access this resource.",
                >= 500 => "Server error: The server encountered an issue. Please try again later.",
                _ => "Protocol error: An unexpected response was received from the server."
            },
            UnityWebRequest.Result.DataProcessingError => "Data processing error: The received data could not be processed.",
            _ => "An unknown error occurred. Please try again.",
        };
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
    
    private IEnumerator GetRequest<T>(string endpoint, Action<T> callback)
    {
        using var wr = CreateGetRequest(endpoint);
        yield return wr.SendWebRequest();

        if (wr.result != UnityWebRequest.Result.Success)
        {
            InvokeErrorOccurred(endpoint, wr);
            callback?.Invoke(default);
        }
        else
        {
            try
            {
                var result = JsonConvert.DeserializeObject<T>(wr.downloadHandler.text, _jsonSettings);
                callback?.Invoke(result);
            }
            catch (Exception ex)
            {
                InvokeErrorOccurred(endpoint, ex);
                callback?.Invoke(default);
            }
        }
    }
}

public class CertificateBypassHandler : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData) => true;
}
