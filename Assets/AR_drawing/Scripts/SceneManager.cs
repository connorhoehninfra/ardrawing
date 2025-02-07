using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ARDrawingSceneManager : MonoBehaviour
{
    public static ARDrawingSceneManager Instance { get; private set; }

    [Tooltip("Reference to the OVRPassthroughLayer.")]
    public OVRPassthroughLayer ovrPassthroughLayer;

    [Tooltip("Reference to the UI canvas game object.")]
    public GameObject uiCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Changes the scene based on the provided index.
    /// </summary>
    /// <param name="sceneIndex">Index of the scene to load.</param>
    public void ChangeScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// Sets the opacity of the passthrough layer.
    /// </summary>
    /// <param name="value">The new opacity value.</param>
    public void SetPassthroughOpacity(float value)
    {
        if (ovrPassthroughLayer != null)
        {
            float clampedValue = Mathf.Clamp(value, 0, 0.2f);
            ovrPassthroughLayer.DOKill(); // Kill any existing tweens on the layer
            DOVirtual.DelayedCall(0.2f, () => {
                DOTween.To(() => ovrPassthroughLayer.textureOpacity, x => ovrPassthroughLayer.textureOpacity = x, clampedValue, 1.0f);
            });
        }
        else
        {
            Debug.LogError("SceneManager: OVRPassthroughLayer is not assigned.");
        }
    }

    /// <summary>
    /// Handles the event when a room is loaded.
    /// </summary>
    public void OnRoomLoaded()
    {
        SetPassthroughOpacity(0.05f);
        if (uiCanvas != null)
        {
            uiCanvas.SetActive(true);
        }
        else
        {
            Debug.LogError("SceneManager: UICanvas is not assigned.");
        }
    }

    /// <summary>
    /// Loads the tutorial scene.
    /// </summary>
    public static void LoadTutorialScene()
    {
        if (Instance != null && Instance.ovrPassthroughLayer != null)
        {
            Instance.SetPassthroughOpacity(0.05f);
            DOVirtual.DelayedCall(0.5f, () => {
                SceneManager.LoadScene(2);
            });
        }
        else
        {
            SceneManager.LoadScene(2);
        }
    }

    /// <summary>
    /// Logs when an object is selected.
    /// </summary>
    public void WhenSelected()
    {
        Debug.Log("ARDrawingSceneManager: Object selected.");
    }

    /// <summary>
    /// Logs when an object is unselected.
    /// </summary>
    public void WhenUnselected()
    {
        Debug.Log("ARDrawingSceneManager: Object unselected.");
    }

    /// <summary>
    /// Logs a custom message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public void LogMessage(string message)
    {
        Debug.Log($"ARDrawingSceneManager: {message}");
    }
}