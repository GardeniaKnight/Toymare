
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.AI;
// using Photon.Pun;

// public class GameInitializer : MonoBehaviour
// {
//     [Header("仅单人模式启用的管理器")]
//     public GameObject aiManager;
//     public GameObject gameOverManager;
//     public GameObject pickupManager;
//     public GameObject scoreManager;

//     [Header("仅单人模式使用的 Player 预制体")]
//     public GameObject singlePlayerPrefab;

//     [Header("仅多人模式使用的 Player 预制体")]
//     public GameObject multiplayerPlayerPrefab;
//     [Tooltip("在场景中预先布置的出生点")]
//     public Transform[] spawnPoints;

//     // NavMesh 贴点参数
//     [Header("NavMesh 贴地参数")]
//     [Tooltip("在原始 spawnPoints 半径内采样")]
//     public float navSampleRadius = 5f;

//     void Start()
//     {
//         if (GameModeManager.CurrentMode == GameMode.SinglePlayer)
//         {
//             InitSinglePlayer();
//         }
//         else if (GameModeManager.CurrentMode == GameMode.MultiPlayer)
//         {
//             InitMultiplayer();
//         }
//         else
//         {
//             Debug.LogWarning("未知游戏模式，返回主菜单");
//             SceneManager.LoadScene("MainMenu");
//         }
//     }

//     void InitSinglePlayer()
//     {
//         // 启用单人模块
//         aiManager?.SetActive(true);
//         gameOverManager?.SetActive(true);
//         pickupManager?.SetActive(true);
//         scoreManager?.SetActive(true);

//         if (singlePlayerPrefab != null)
//         {
//             // 在原点生成单人玩家
//             GameObject player = Instantiate(
//                 singlePlayerPrefab,
//                 Vector3.zero,
//                 Quaternion.identity
//             );

//             // 绑定 WaveManager
//             WaveManager waveManager = FindObjectOfType<WaveManager>();
//             if (waveManager != null)
//             {
//                 var ph = player.GetComponent<PlayerHealth>();
//                 if (ph != null)
//                     waveManager.playerHealth = ph;
//             }

//             // 绑定 GameOverManager
//             var playerHealth = player.GetComponent<PlayerHealth>();
//             if (gameOverManager != null && playerHealth != null)
//             {
//                 gameOverManager
//                     .GetComponent<GameOverManager>()
//                     .playerHealth = playerHealth;
//             }

//             // 绑定 HUD
//             PlayerHUD hud = FindObjectOfType<PlayerHUD>();
//             if (hud != null && playerHealth != null)
//             {
//                 playerHealth.healthSliderForeground = hud.foreground;
//                 playerHealth.healthSliderBackground = hud.background;
//             }
//         }
//     }

//     void InitMultiplayer()
//     {
//         // 确保模式已标记
//         GameModeManager.CurrentMode = GameMode.MultiPlayer;

//         // 禁用单人模块
//         aiManager?.SetActive(false);
//         gameOverManager?.SetActive(false);
//         pickupManager?.SetActive(false);
//         scoreManager?.SetActive(false);
        

//         if (!PhotonNetwork.InRoom ||
//             multiplayerPlayerPrefab == null ||
//             spawnPoints == null ||
//             spawnPoints.Length == 0)
//         {
//             Debug.LogWarning("多人模式：尚未加入房间或未配置预制体/出生点");
//             return;
//         }

//         // 随机选择一个 spawn point
//         int idx = Random.Range(0, spawnPoints.Length);
//         Vector3 rawPos = spawnPoints[idx].position;
//         Quaternion rawRot = spawnPoints[idx].rotation;

//         // 在 NavMesh 上寻找最近的可行走位置
//         NavMeshHit hit;
//         Vector3 spawnPos;
//         if (NavMesh.SamplePosition(rawPos, out hit, navSampleRadius, NavMesh.AllAreas))
//         {
//             spawnPos = hit.position;
//         }
//         else
//         {
//             spawnPos = rawPos;
//             Debug.LogWarning($"SpawnPoint '{spawnPoints[idx].name}' 未能采样 NavMesh，使用原始位置");
//         }

//         // 网络实例化玩家
//         PhotonNetwork.Instantiate(
//             multiplayerPlayerPrefab.name,
//             spawnPos,
//             rawRot
//         );
//     }
// }

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Photon.Pun;

public class GameInitializer : MonoBehaviour
{
    [Header("仅单人模式启用的管理器")]
    public GameObject aiManager;
    public GameObject gameOverManager;
    public GameObject pickupManager;
    public GameObject scoreManager;

    [Header("仅多人模式启用的管理器")]
    [Tooltip("挂载 NetScoreManager 的 GameObject")]
    public GameObject netScoreManager;
    [Tooltip("排行榜所用 Canvas")]
    public GameObject scoreBoardCanvas;
    [Tooltip("承载 ScoreboardUI 的 GameObject")]
    public GameObject scoreUIManager;

    [Header("仅单人模式使用的 Player 预制体")]
    public GameObject singlePlayerPrefab;

    [Header("仅多人模式使用的 Player 预制体")]
    public GameObject multiplayerPlayerPrefab;
    [Tooltip("在场景中预先布置的出生点")]
    public Transform[] spawnPoints;

    [Header("NavMesh 贴地参数")]
    [Tooltip("在原始 spawnPoints 半径内采样")]
    public float navSampleRadius = 5f;

    void Start()
    {
        // 如果已经在房间里，就走多人逻辑；否则走单人逻辑
        if (PhotonNetwork.InRoom)
        {
            InitMultiplayer();
        }
        else
        {
            InitSinglePlayer();
        }
    }

    void InitSinglePlayer()
    {
        // 单人模式：开启单人相关，关闭多人相关
        aiManager?.SetActive(true);
        gameOverManager?.SetActive(true);
        pickupManager?.SetActive(true);
        scoreManager?.SetActive(true);

        //netScoreManager?.SetActive(false);
        //scoreBoardCanvas?.SetActive(false);
        //scoreUIManager?.SetActive(false);

        // 生成单人玩家
        if (singlePlayerPrefab != null)
        {
            var player = Instantiate(
                singlePlayerPrefab,
                Vector3.zero,
                Quaternion.identity
            );

            // 绑定 WaveManager
            var waveManager = FindObjectOfType<WaveManager>();
            if (waveManager != null)
            {
                var ph = player.GetComponent<PlayerHealth>();
                if (ph != null)
                    waveManager.playerHealth = ph;
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
            var hud = FindObjectOfType<PlayerHUD>();
            if (hud != null && playerHealth != null)
            {
                playerHealth.healthSliderForeground = hud.foreground;
                playerHealth.healthSliderBackground = hud.background;
            }
        }
    }

    void InitMultiplayer()
    {
        // 多人模式：开启多人相关，关闭单人相关
        aiManager?.SetActive(false);
        gameOverManager?.SetActive(false);
        pickupManager?.SetActive(false);
        scoreManager?.SetActive(false);

        netScoreManager?.SetActive(true);
        scoreBoardCanvas?.SetActive(true);
        scoreUIManager?.SetActive(true);

        // 只有在真正加入房间后才生成
        if (!PhotonNetwork.InRoom
            || multiplayerPlayerPrefab == null
            || spawnPoints == null
            || spawnPoints.Length == 0)
        {
            Debug.LogWarning("多人模式：尚未加入房间或未配置预制体/出生点");
            return;
        }

        // 随机选一个 spawn 点，并在 NavMesh 上贴地
        int idx = Random.Range(0, spawnPoints.Length);
        var rawTF = spawnPoints[idx];
        Vector3 rawPos = rawTF.position;
        Quaternion rawRot = rawTF.rotation;

        if (NavMesh.SamplePosition(rawPos, out NavMeshHit hit, navSampleRadius, NavMesh.AllAreas))
        {
            rawPos = hit.position;
        }
        else
        {
            Debug.LogWarning($"SpawnPoint '{rawTF.name}' 未能采样 NavMesh，使用原始位置");
        }

        // 网络实例化玩家
        PhotonNetwork.Instantiate(
            multiplayerPlayerPrefab.name,
            rawPos,
            rawRot
        );
    }
}

