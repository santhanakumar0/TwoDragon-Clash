using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public DragonHealth playerHealth;
    public DragonHealth enemyHealth;
    public GameObject winPanel;
    public GameObject losePanel;
    public float delay = 3f; // your death animation time

    bool gameOver;

    void Start()
    {
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (gameOver) return;

        if (playerHealth != null && playerHealth.IsDead)
        {
            gameOver = true;
            StartCoroutine(ShowLoseAfterDelay());
        }
        else if (enemyHealth != null && enemyHealth.IsDead)
        {
            gameOver = true;
            StartCoroutine(ShowWinAfterDelay());
        }
    }

    IEnumerator ShowWinAfterDelay()
    {
        yield return new WaitForSeconds(delay); // wait 3 sec for death anim
        if (losePanel) losePanel.SetActive(false);
        if (winPanel) winPanel.SetActive(true);
        Time.timeScale = 0f; // pause AFTER animation
    }

    IEnumerator ShowLoseAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}