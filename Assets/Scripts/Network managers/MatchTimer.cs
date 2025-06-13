// MatchTimer.cs
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MatchTimer : MonoBehaviourPunCallbacks
{
    [Header("UI 文本（倒计时）")]
    public Text timerText;
    [Header("结束后保留面板时间（秒）")]
    public float endDelay = 5f;

    private double startTime;
    private double duration;
    private bool   running = false;

    void Start()
    {
        TryInitTimer();
    }

    public override void OnRoomPropertiesUpdate(Hashtable propsThatChanged)
    {
        if (propsThatChanged.ContainsKey("StartTime"))
        {
            double st = (double)PhotonNetwork.CurrentRoom.CustomProperties["StartTime"];
            SetupTimer(st, 600.0);
        }
    }

    void TryInitTimer()
    {
        if (running) return;
        if (PhotonNetwork.CurrentRoom != null &&
            PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("StartTime", out var stObj))
        {
            double st = (double)stObj;
            SetupTimer(st, 600.0);
        }
    }

    void SetupTimer(double st, double dur)
    {
        startTime = st;
        duration  = dur;
        running   = true;
        Debug.Log($"[MatchTimer] Timer started at {startTime}, duration {duration}");
    }

    void Update()
    {
        if (!running) return;

        double elapsed   = PhotonNetwork.Time - startTime;
        double remaining = duration - elapsed;

        if (remaining > 0)
        {
            int sec = Mathf.CeilToInt((float)remaining);
            int m   = sec / 60;
            int s   = sec % 60;
            if (timerText != null)
                timerText.text = $"{m:00}:{s:00}";
        }
        else
        {
            running = false;
            if (timerText != null) timerText.text = "00:00";
            OnMatchEnd();
        }
    }

    void OnMatchEnd()
    {
        Debug.Log("[MatchTimer] 比赛结束，显示最终排行榜");
        var sb = FindObjectOfType<ScoreboardUI>();
        if (sb != null) sb.ForceShow();

        if (PhotonNetwork.IsMasterClient)
            Invoke(nameof(ReturnToLobby), endDelay);
    }

    void ReturnToLobby()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// 外部（如 GameInitializer）可调用此方法手动启动计时
    /// </summary>
    public void StartTimer(double st, double dur)
    {
        SetupTimer(st, dur);
    }
}
