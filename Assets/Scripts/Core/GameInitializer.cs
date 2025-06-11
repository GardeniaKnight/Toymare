using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameInitializer : MonoBehaviour
{
    [Header("仅单人模式启用的管理器")]
    public GameObject aiManager;
    public GameObject gameOverManager;
    public GameObject pickupManager;
    public GameObject scoreManager;

    [Header("仅单人模式使用的 Player 预制体")]
    public GameObject singlePlayerPrefab;

    [Header("仅多人模式使用的 Player 预制体")]
    public GameObject multiplayerPlayerPrefab;
    [Tooltip("在场景中预先布置的出生点")]
    public Transform[] spawnPoints;

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
            Debug.LogWarning("未知游戏模式，返回主菜单");
            SceneManager.LoadScene("MainMenu");
        }
    }

    void InitSinglePlayer()
    {
        // 启用单人模块
        aiManager?.SetActive(true);
        gameOverManager?.SetActive(true);
        pickupManager?.SetActive(true);
        scoreManager?.SetActive(true);

        if (singlePlayerPrefab != null)
        {
            // 在原点生成单人玩家
            GameObject player = Instantiate(
                singlePlayerPrefab,
                Vector3.zero,
                Quaternion.identity
            );

            // 绑定 WaveManager
            WaveManager waveManager = FindObjectOfType<WaveManager>();
            if (waveManager != null)
            {
                var ph = player.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    waveManager.playerHealth = ph;
                }
            }

            // 绑定 GameOverManager
            var playerHealth = player.GetComponent<PlayerHealth>();
            if (gameOverManager != null && playerHealth != null)
            {
                gameOverManager
                    .GetComponent<GameOverManager>()
                    .playerHealth = playerHealth;
            }

            // 绑定 HUD
            PlayerHUD hud = FindObjectOfType<PlayerHUD>();
            if (hud != null && playerHealth != null)
            {
                playerHealth.healthSliderForeground = hud.foreground;
                playerHealth.healthSliderBackground = hud.background;
            }
        }
    }

    void InitMultiplayer()
    {
        // 禁用所有单人专属模块
        aiManager?.SetActive(false);
        gameOverManager?.SetActive(false);
        pickupManager?.SetActive(false);
        scoreManager?.SetActive(false);

        // 仅在已加入 Photon 房间时生成玩家
        if (PhotonNetwork.InRoom 
            && multiplayerPlayerPrefab != null 
            && spawnPoints != null 
            && spawnPoints.Length > 0)
        {
            // 按 ActorNumber 分配出生点
            int index = (PhotonNetwork.LocalPlayer.ActorNumber - 1)
                        % spawnPoints.Length;
            Transform sp = spawnPoints[index];

            PhotonNetwork.Instantiate(
                multiplayerPlayerPrefab.name,
                sp.position,
                sp.rotation
            );
        }
        else
        {
            Debug.LogWarning("多人模式：尚未加入房间或未配置预制体/出生点");
        }
    }
}
