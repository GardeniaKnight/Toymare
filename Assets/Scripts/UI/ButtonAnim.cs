using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ButtonClickEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    public float scaleFactor = 0.9f;           // 缩放倍率
    public float bounceDuration = 0.1f;        // 弹回动画时间
    public AudioClip clickSound;               // 点击音效
    private AudioSource audioSource;

    void Start()
    {
        originalScale = transform.localScale;

        // 自动获取或添加 AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale = originalScale * scaleFactor;

        // 播放点击音效
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(BounceBack());
    }

    IEnumerator BounceBack()
    {
        float time = 0f;
        while (time < bounceDuration)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, time / bounceDuration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;
    }
}
