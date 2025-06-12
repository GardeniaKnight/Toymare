// NetScoreManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetScoreManager : MonoBehaviourPun
{
    public static NetScoreManager Instance { get; private set; }

    // actorNumber → score
    private Dictionary<int,int> scores = new Dictionary<int,int>();

    // 当分数更新时，UI 可订阅此事件
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

    /// <summary>
    /// MasterClient 专属：收到上报后累加并广播全网最新分数
    /// </summary>
    [PunRPC]
    public void RPC_AddScore(int actorNumber, int delta)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (!scores.ContainsKey(actorNumber))
            scores[actorNumber] = 0;
        scores[actorNumber] += delta;

        // 同步给所有客户端（缓冲，以便新加入的也能收到）
        var actors = scores.Keys.ToArray();
        var values = actors.Select(a => scores[a]).ToArray();
        photonView.RPC("RPC_SyncScores", RpcTarget.AllBuffered, actors, values);
    }

    /// <summary>
    /// 所有客户端：同步 MasterClient 广播的最新分数
    /// </summary>
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
    /// 返回当前所有玩家的排名列表（按分数降序），不存在的分数默认为 0
    /// </summary>
    public List<(string nick, int score)> GetSortedScores()
    {
        // 遍历所有在线玩家，拿他们的 ActorNumber 查分数，缺省 0
        var list = new List<(string, int)>();
        foreach (var p in PhotonNetwork.PlayerList)
        {
            int sc = 0;
            scores.TryGetValue(p.ActorNumber, out sc);
            list.Add((p.NickName, sc));
        }
        // 按分数降序
        return list
            .OrderByDescending(item => item.Item2)
            .ToList();
    }
}
