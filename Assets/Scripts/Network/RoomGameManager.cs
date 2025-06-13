// using Photon.Pun;
// using Photon.Realtime;
// using UnityEngine;

// /// <summary>
// /// 在 LobbyScene 中，当房间人数达到最小要求时，
// /// 由房主（Master Client）自动加载游戏场景。
// /// </summary>
// public class RoomGameManager : MonoBehaviourPunCallbacks
// {
//     [Header("最低启动玩家数")]            
//     public int minPlayers = 2;

//     [Header("要加载的场景名")]            
//     public string gameSceneName = "Game";

//     bool hasStarted = false;

//     void Start()
//     {
//         TryStartGame(); // 防止 Master Client 先加入后直接满足条件
//     }

//     public override void OnPlayerEnteredRoom(Player newPlayer)
//     {
//         TryStartGame();
//     }

//     public override void OnPlayerLeftRoom(Player otherPlayer)
//     {
//         // 可选：更新 UI 人数显示，LobbyUIController 已在 HandlePlayerCountChanged 里处理
//     }

//     void TryStartGame()
//     {
//         if (!PhotonNetwork.IsMasterClient || hasStarted)
//             return;

//         int count = PhotonNetwork.CurrentRoom.PlayerCount;
//         if (count >= minPlayers)
//         {
//             hasStarted = true;
//             GameModeManager.CurrentMode = GameMode.MultiPlayer;
//             Debug.Log($"RoomGameManager: 人数 {count} 达标({minPlayers})，Master 正在加载 {gameSceneName}");
//             PhotonNetwork.LoadLevel(gameSceneName);
//         }
//     }
// }

using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// 在 LobbyScene 中，当房间人数达到最小要求时，
/// 由房主（Master Client）自动设置开始时间并加载游戏场景。
/// </summary>
public class RoomGameManager : MonoBehaviourPunCallbacks
{
    [Header("最低启动玩家数")]
    public int minPlayers = 2;

    [Header("要加载的场景名")]
    public string gameSceneName = "Game";

    bool hasStarted = false;

    void Start()
    {
        TryStartGame(); // 房主首次进入时也检查一次
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        TryStartGame();
    }

    void TryStartGame()
    {
        // 仅房主执行一次
        if (!PhotonNetwork.IsMasterClient || hasStarted)
            return;

        if (PhotonNetwork.CurrentRoom.PlayerCount >= minPlayers)
        {
            hasStarted = true;
            // 1) 记录开始时间（服务器同步时钟）
            var props = new ExitGames.Client.Photon.Hashtable {
                { "StartTime", PhotonNetwork.Time }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            Debug.Log($"[RoomGameManager] 设置 StartTime={PhotonNetwork.Time}，开始加载场景 {gameSceneName}");

            // 2) 切到对战场景（会自动同步给所有客户端）
            PhotonNetwork.LoadLevel(gameSceneName);
        }
    }
}
