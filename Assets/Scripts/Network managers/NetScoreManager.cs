using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetScoreManager : MonoBehaviourPun
{
    public static NetScoreManager Instance { get; private set; }

    // ActorNumber → 分数
    private Dictionary<int,int> scores = new Dictionary<int,int>();

    // 分数更新事件（UI 可订阅）
    public event Action<Dictionary<int,int>> OnScoresUpdated;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    [PunRPC]
    public void RPC_AddScore(int actorNumber, int delta)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (!scores.ContainsKey(actorNumber))
            scores[actorNumber] = 0;
        scores[actorNumber] += delta;

        // 广播最新全表（缓冲）
        var actors = scores.Keys.ToArray();
        var vals   = actors.Select(a => scores[a]).ToArray();
        photonView.RPC("RPC_SyncScores",
                       RpcTarget.AllBuffered,
                       actors, vals);
    }

    [PunRPC]
    public void RPC_SyncScores(int[] actorNumbers, int[] values)
    {
        Debug.Log($"[NetScoreManager] RPC_SyncScores, actors:{actorNumbers.Length}, values:{values.Length}");

        scores.Clear();
        for (int i = 0; i < actorNumbers.Length; i++)
            scores[actorNumbers[i]] = values[i];

        OnScoresUpdated?.Invoke(new Dictionary<int,int>(scores));
    }

    /// <summary>
    /// 返回当前所有在线玩家的昵称 + 分数列表，按分数降序
    /// </summary>
    public List<(string nick, int score)> GetSortedScores()
    {
        var list = new List<(string, int)>();
        foreach (var p in PhotonNetwork.PlayerList)
        {
            int sc = 0;
            scores.TryGetValue(p.ActorNumber, out sc);
            list.Add((p.NickName, sc));
        }
        return list.OrderByDescending(item => item.Item2).ToList();
    }

    /// <summary>
    /// （可选）按 ActorNumber 直接查询当前分数
    /// </summary>
    public int GetScore(int actorNumber)
    {
        if (scores.TryGetValue(actorNumber, out var s)) return s;
        return 0;
    }
}
