using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class SimpleScoreboardUI : MonoBehaviour
{
    [Header("主 Panel（带背景 Image）")]
    public GameObject panel;         // 关联：ScoreboardPanel
    [Header("列表容器（带 VerticalLayoutGroup）")]
    public Transform contentParent;  // 关联：Content
    [Header("行预制体：一个带 Text 的 Prefab")]
    public GameObject entryPrefab;   // 关联：ScoreEntry Prefab

    // actorNumber → Text 行
    private Dictionary<int, Text> entries = new Dictionary<int, Text>();

    void Awake()
    {
        // 一开始隐藏
        if (panel != null) panel.SetActive(false);
    }

    void Update()
    {
        // 按下 Tab：刷新并显示
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Refresh();
            panel.SetActive(true);
        }
        // 松开 Tab：隐藏
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            panel.SetActive(false);
        }
    }

    void Refresh()
    {
        if (contentParent == null || entryPrefab == null) return;

        // 清掉旧的不再在线的行
        var toRemove = new List<int>();
        foreach (var kv in entries)
        {
            bool stillOnline = false;
            foreach (var p in PhotonNetwork.PlayerList)
                if (p.ActorNumber == kv.Key) { stillOnline = true; break; }
            if (!stillOnline) toRemove.Add(kv.Key);
        }
        foreach (var a in toRemove)
        {
            Destroy(entries[a].gameObject);
            entries.Remove(a);
        }

        // 遍历所有在线玩家，生成或更新行
        foreach (var p in PhotonNetwork.PlayerList)
        {
            int actor = p.ActorNumber;
            Text txt;
            if (!entries.TryGetValue(actor, out txt))
            {
                // 新玩家，实例化一行
                var go = Instantiate(entryPrefab, contentParent);
                txt = go.GetComponent<Text>();
                entries[actor] = txt;
            }
            // 拿分数；假设 NetScoreManager 有一个 GetScore(actorNumber) 方法
            int score = 0;
            if (NetScoreManager.Instance != null)
                score = NetScoreManager.Instance.GetSortedScores()
                            .Find(tuple => tuple.nick == p.NickName).score;
            txt.text = $"{p.NickName}: {score}";
        }
    }
}
