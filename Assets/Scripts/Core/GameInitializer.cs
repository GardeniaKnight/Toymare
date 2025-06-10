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
    // 启用仅单人使用的组件
    if (aiManager != null) aiManager.SetActive(true);
    if (gameOverManager != null) gameOverManager.SetActive(true);
    if (pickupManager != null) pickupManager.SetActive(true);
    if (scoreManager != null) scoreManager.SetActive(true);

        if (singlePlayerPrefab != null)
        {
            GameObject player = Instantiate(singlePlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            // ✅ 只在单人模式中绑定 WaveManager
            WaveManager waveManager = FindObjectOfType<WaveManager>();
            if (waveManager != null)
            {
                var playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    waveManager.playerHealth = playerHealth;
                    Debug.Log("✅ WaveManager 绑定成功");
                }
            }
            // 生成 Player 后……
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            // 绑定给 GameOverManager
            if (gameOverManager != null && ph != null)
                gameOverManager.GetComponent<GameOverManager>().playerHealth = ph;
            
            // 2. 找到场景里的 HUD
            PlayerHUD hud = FindObjectOfType<PlayerHUD>();

            // 3. 把 HUD 推给 PlayerHealth
            ph.healthSliderForeground = hud.foreground;
            ph.healthSliderBackground = hud.background;
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
