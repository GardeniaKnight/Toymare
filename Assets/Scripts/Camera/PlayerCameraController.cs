// using UnityEngine;
// using Unity.Netcode;

// public class PlayerCameraController : MonoBehaviour
// {
//     public Camera playerCamera;

//     void Start()
//     {
        
//         var netObj = GetComponent<NetworkObject>();
//         if (netObj != null && !netObj.IsOwner)
//         {
//             playerCamera.gameObject.SetActive(false);
//             return;
//         }

//         // 启用本地摄像机
//         playerCamera.gameObject.SetActive(true);
//         playerCamera.tag = "MainCamera";

//         // 确保只有一个 AudioListener 被启用
//         foreach (var listener in FindObjectsOfType<AudioListener>())
//         {
//             if (listener != playerCamera.GetComponent<AudioListener>())
//                 listener.enabled = false;
//         }
//     }
// }

using UnityEngine;
using Photon.Pun;

public class PlayerCameraController : MonoBehaviour
{
    [Header("本地玩家摄像机")]
    public Camera playerCamera;

    void Start()
    {
        // 如果挂载了 PhotonView 并且又不是本地玩家，就禁用这台摄像机
        var pv = GetComponent<PhotonView>();
        if (pv != null && !pv.IsMine)
        {
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(false);
            return;
        }

        // 只有本地玩家才启用该摄像机
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(true);
            playerCamera.tag = "MainCamera";
        }

        // 确保场景中只有一个 AudioListener 生效
        var localListener = playerCamera?.GetComponent<AudioListener>();
        foreach (var listener in FindObjectsOfType<AudioListener>())
        {
            if (listener != localListener)
                listener.enabled = false;
        }
    }
}
