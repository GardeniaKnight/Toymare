using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1.5f;
    public GameObject menuUI;

    void Start()
    {
        menuUI.SetActive(false);
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float timer = 0f;
        Color color = fadeImage.color;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
        menuUI.SetActive(true);
    }

    // ✅ 单人按钮：设置为 SinglePlayer
    public void OnSinglePlayerClicked()
    {
        GameModeManager.CurrentMode = GameMode.SinglePlayer;
        SceneManager.LoadScene("Game");
    }

    // ✅ 多人按钮：设置为 MultiPlayer
    public void OnMultiplayerClicked()
    {
        GameModeManager.CurrentMode = GameMode.MultiPlayer;
        SceneManager.LoadScene("Game");
    }

    public void OnQuitClicked()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
