using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    public Image fadeImage;                  // 用于黑屏淡入的 Image（全屏黑色）
    public float fadeDuration = 1.5f;        // 淡入时长
    public GameObject menuUI;                // 包含按钮的 UI 容器（Canvas 下）

    void Start()
    {
        menuUI.SetActive(false); // 先隐藏按钮
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
        menuUI.SetActive(true); // 淡入后显示按钮
    }

    // 下面是按钮事件函数
    public void OnSinglePlayerClicked()
    {
        SceneManager.LoadScene("SingleLevel 01"); // 替换为你的单人场景名
    }

    public void OnMultiplayerClicked()
    {
        SceneManager.LoadScene("MultiplayerScene"); // 替换为你的多人场景名
    }

    public void OnQuitClicked()
    {
        Application.Quit(); // 退出游戏
        Debug.Log("Quit Game"); // 编辑器中看不到退出，打印日志方便测试
    }
}
