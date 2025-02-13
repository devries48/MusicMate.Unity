using UnityEngine;

/// <summary>
/// A singleton utility class for running coroutines globally, even if the calling object is inactive.
/// This ensures coroutines can always be started, preventing "Coroutine couldn't be started" errors.
/// </summary>
public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    /// <summary>
    /// Singleton instance of CoroutineRunner.
    /// </summary>
    public static CoroutineRunner Instance
    {
        get
        {
            // If instance is null, create a new GameObject to host the CoroutineRunner
            if (!_instance)
            {
                GameObject runner = new("CoroutineRunner");
                _instance = runner.AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(runner); // Ensure it persists across scenes
            }
            return _instance;
        }
    }
}
