using UnityEngine;
using Photon.Pun;

public class PhotonInit : MonoBehaviour
{
    void Awake()
    {
        // —— 网络层：在任何 LoadLevel 之前开启自动场景同步
        PhotonNetwork.AutomaticallySyncScene = true;
        // （可选）立即连接 Photon
        PhotonNetwork.ConnectUsingSettings();
        DontDestroyOnLoad(gameObject);
    }
}
