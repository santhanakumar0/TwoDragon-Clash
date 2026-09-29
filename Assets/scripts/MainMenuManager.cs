using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Tooltip("Exact name of your gameplay scene, as it appears in Build Settings.")]
    public string gameplaySceneName = "Game";

    void Start()
    {
        Time.timeScale = 1f; // FIX for sound bug
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMenuMusic();
        }
    }

    public void PlayGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClick();
            AudioManager.Instance.PlayGameMusic(); // or StopMusic if you want
        }

        SceneManager.LoadScene(gameplaySceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit pressed");
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}