using UnityEngine;
using Unity.Netcode;

public class PlayerCameraController : MonoBehaviour
{
    public Camera playerCamera;

    void Start()
    {
        var netObj = GetComponent<NetworkObject>();
        if (netObj != null && !netObj.IsOwner)
        {
            playerCamera.gameObject.SetActive(false);
            return;
        }

        // 启用本地摄像机
        playerCamera.gameObject.SetActive(true);
        playerCamera.tag = "MainCamera";

        // 确保只有一个 AudioListener 被启用
        foreach (var listener in FindObjectsOfType<AudioListener>())
        {
            if (listener != playerCamera.GetComponent<AudioListener>())
                listener.enabled = false;
        }
    }
}
