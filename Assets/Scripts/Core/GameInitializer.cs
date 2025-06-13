// GameInitializer.cs
using System.Collections;
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
    public GameObject netScoreManager;
    public GameObject scoreBoardCanvas;
    public GameObject scoreUIManager;

    [Header("仅单人模式使用的 Player 预制体")]
    public GameObject singlePlayerPrefab;

    [Header("仅多人模式使用的 Player 预制体")]
    public GameObject multiplayerPlayerPrefab;
    public Transform[] spawnPoints;

    [Header("NavMesh 贴地参数")]
    public float navSampleRadius = 5f;

    void Start()
    {
        if (PhotonNetwork.InRoom)
            InitMultiplayer();
        else
            InitSinglePlayer();
    }

    void InitSinglePlayer()
    {
        aiManager?.SetActive(true);
        gameOverManager?.SetActive(true);
        pickupManager?.SetActive(true);
        scoreManager?.SetActive(true);

        netScoreManager?.SetActive(false);
        scoreBoardCanvas?.SetActive(false);
        scoreUIManager?.SetActive(false);

        if (singlePlayerPrefab == null) return;
        var player = Instantiate(singlePlayerPrefab, Vector3.zero, Quaternion.identity);

        var waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
            waveManager.playerHealth = player.GetComponent<PlayerHealth>();

        if (gameOverManager != null)
            gameOverManager.GetComponent<GameOverManager>().playerHealth = player.GetComponent<PlayerHealth>();

        var hud = FindObjectOfType<PlayerHUD>();
        if (hud != null)
        {
            var ph = player.GetComponent<PlayerHealth>();
            ph.healthSliderForeground = hud.foreground;
            ph.healthSliderBackground = hud.background;
        }
    }

    void InitMultiplayer()
    {
        aiManager?.SetActive(false);
        gameOverManager?.SetActive(false);
        pickupManager?.SetActive(false);
        scoreManager?.SetActive(false);

        netScoreManager?.SetActive(true);
        scoreBoardCanvas?.SetActive(true);
        scoreUIManager?.SetActive(true);

        if (!PhotonNetwork.InRoom ||
            multiplayerPlayerPrefab == null ||
            spawnPoints == null ||
            spawnPoints.Length == 0)
        {
            Debug.LogWarning("多人模式：配置不完整");
            return;
        }

        int idx = Random.Range(0, spawnPoints.Length);
        var raw = spawnPoints[idx];
        Vector3 pos = raw.position;
        if (NavMesh.SamplePosition(pos, out NavMeshHit hit, navSampleRadius, NavMesh.AllAreas))
            pos = hit.position;
        else
            Debug.LogWarning($"SpawnPoint '{raw.name}' 贴地失败");

        PhotonNetwork.Instantiate(multiplayerPlayerPrefab.name, pos, raw.rotation);

        // 延迟两帧再启动比赛计时
        StartCoroutine(DelayedStartTimer());
    }

    IEnumerator DelayedStartTimer()
    {
        yield return null;
        yield return null;

        if (PhotonNetwork.CurrentRoom != null &&
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("StartTime", out var st))
        {
            double startTime    = (double)st;
            double matchDuration = 600.0;
            var timer = FindObjectOfType<MatchTimer>();
            if (timer != null)
                timer.StartTimer(startTime, matchDuration);
            else
                Debug.LogError("GameInitializer: 找不到 MatchTimer");
        }
        else
        {
            Debug.LogError("GameInitializer: 延迟读取 StartTime 失败");
        }
    }
}
