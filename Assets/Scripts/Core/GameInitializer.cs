using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameInitializer : MonoBehaviour
{
    [Header("仅单人模式启用的管理器")]
    public GameObject aiManager;
    public GameObject gameOverManager;
    public GameObject pickupManager;
    public GameObject scoreManager;

    [Header("仅单人模式使用的 Player Prefab")]
    public GameObject singlePlayerPrefab;

    [Header("仅多人模式启用的 NetworkManager 预制体")]
    public GameObject networkManagerPrefab;

    void Start()
    {
        if (GameModeManager.CurrentMode == GameMode.SinglePlayer)
        {
            InitSinglePlayer();
        }
        else if (GameModeManager.CurrentMode == GameMode.MultiPlayer)
        {
            InitMultiplayer();
        }
        else
        {
            Debug.LogWarning("未知游戏模式，回到主菜单");
            SceneManager.LoadScene("MainMenu");
        }
    }

    void InitSinglePlayer()
    {
        if (aiManager != null) aiManager.SetActive(true);
        if (gameOverManager != null) gameOverManager.SetActive(true);
        if (pickupManager != null) pickupManager.SetActive(true);
        if (scoreManager != null) scoreManager.SetActive(true);

        if (singlePlayerPrefab != null)
        {
            Instantiate(singlePlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("SinglePlayerPrefab 未指定！");
        }
    }

    void InitMultiplayer()
    {
        if (aiManager != null) aiManager.SetActive(false);
        if (gameOverManager != null) gameOverManager.SetActive(false);
        if (pickupManager != null) pickupManager.SetActive(false);
        if (scoreManager != null) scoreManager.SetActive(false);

        if (NetworkManager.Singleton == null && networkManagerPrefab != null)
        {
            Instantiate(networkManagerPrefab);
        }

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartHost(); // 或 StartClient()
        }
        else
        {
            Debug.LogError("NetworkManager 启动失败，请检查配置");
        }
    }
}
