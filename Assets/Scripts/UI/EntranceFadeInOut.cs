using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeInOut : MonoBehaviour
{
    public Image blackOverlayImage;  // 黑色遮罩图层（应铺满全屏）
    public Image logoImage;          // logo 图片（始终不透明）
    public float fadeDuration = 2f;  // 渐变时间
    public float displayTime = 2f;   // logo 停留时间
    public string nextSceneName = "MainScene"; // 要跳转的场景名

    void Start()
    {
        StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        // 初始设置：黑幕不透明，logo 不透明（由黑幕遮住）
        SetAlpha(blackOverlayImage, 1f);
        SetAlpha(logoImage, 1f);  // logo 始终显示，由遮罩控制可见性

        // 黑幕淡出，露出 logo
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            SetAlpha(blackOverlayImage, Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        // 停留 logo
        yield return new WaitForSeconds(displayTime);

        // 黑幕淡入，覆盖全屏
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            SetAlpha(blackOverlayImage, Mathf.Lerp(0f, 1f, t));
            yield return null;
        }

        // 切换场景
        SceneManager.LoadScene(nextSceneName);
    }

    void SetAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }
}
