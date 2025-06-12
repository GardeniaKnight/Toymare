// // // using System;
// // // using Photon.Pun;
// // // using Photon.Realtime;
// // // using UnityEngine;

// // // public class PhotonRoomService : MonoBehaviourPunCallbacks, IRoomService
// // // {
// // //     // IRoomService 事件
// // //     public event Action<string> OnJoined;
// // //     public event Action<string> OnJoinFailed;
// // //     public event Action<int>    OnPlayerCountChanged;

// // //     // 缓存用户想加入的房间号
// // //     private string pendingRoomName;

// // //     // 是否已经连接并准备好加入房间
// // //     private bool isReady = false;

// // //     /// <summary>加入或创建房间（不限制最大人数）</summary>
// // //     // public void JoinOrCreate(string roomName)
// // //     // {
// // //     //     if (string.IsNullOrWhiteSpace(roomName))
// // //     //         return;

// // //     //     // 如果还没准备好，就先缓存，等回调再调用
// // //     //     if (!PhotonNetwork.IsConnectedAndReady)
// // //     //     {
// // //     //         pendingRoomName = roomName;
// // //     //         Debug.Log($"[PhotonRoomService] 尚未准备就绪，缓存房间号：{roomName}");
// // //     //         return;
// // //     //     }

// // //     //     Debug.Log($"[PhotonRoomService] 直接发起加入/创建房间：{roomName}");
// // //     //     var options = new RoomOptions { MaxPlayers = 0 };
// // //     //     PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
// // //     // }

// // //     public void JoinOrCreate(string roomName)
// // //     {
// // //         if (string.IsNullOrWhiteSpace(roomName))
// // //         {
// // //             Debug.LogWarning("[PhotonRoomService] 房间号为空，不发起请求");
// // //             return;
// // //         }

// // //         // 缓存房号，等 ready 再去真正 JoinOrCreate
// // //         pendingRoomName = roomName.Trim();
// // //         if (isReady)
// // //         {
// // //             TryJoinOrCreate();
// // //         }
// // //         else
// // //         {
// // //             Debug.Log($"[PhotonRoomService] 尚未就绪，缓存房间号：{pendingRoomName}");
// // //         }
// // //     }

// // //     /// <summary>
// // //     /// 真正调用 PhotonNetwork.JoinOrCreateRoom 的方法，保证只在已加入 Lobby 之后执行
// // //     /// </summary>
// // //     private void TryJoinOrCreate()
// // //     {
// // //         Debug.Log($"[PhotonRoomService] 发起 JoinOrCreateRoom：{pendingRoomName}");
// // //         PhotonNetwork.JoinOrCreateRoom(
// // //             pendingRoomName,
// // //             new RoomOptions { MaxPlayers = 0 },
// // //             TypedLobby.Default
// // //         );
// // //     }

// // //     /// <summary>离开当前房间</summary>
// // //     public void Leave()
// // //     {
// // //         if (PhotonNetwork.InRoom)
// // //             PhotonNetwork.LeaveRoom();
// // //     }

// // //     #region Photon 回调
// // //     // public override void OnConnectedToMaster()
// // //     // {
// // //     //     isReady = true;
// // //     //     Debug.Log("[PhotonRoomService] 已连接 Master Server，准备就绪");

// // //     //     // 如果之前用户点击过加入，就立即执行
// // //     //     if (!string.IsNullOrEmpty(pendingRoomName))
// // //     //     {
// // //     //         string roomToJoin = pendingRoomName;
// // //     //         pendingRoomName = null;
// // //     //         JoinOrCreate(roomToJoin);
// // //     //     }
// // //     // }

// // //     public override void OnConnectedToMaster()
// // //     {
// // //         Debug.Log("[PhotonRoomService] 已连接 Master Server，开始加入 Lobby…");
// // //         PhotonNetwork.JoinLobby();  // 一定要先 JoinLobby
// // //     }

// // //     // 一定要重写这个方法，它在真正加入 Lobby 之后被调用
// // //     public override void OnJoinedLobby()
// // //     {
// // //         isReady = true;
// // //         Debug.Log("[PhotonRoomService] 已加入默认 Lobby，准备就绪");
// // //         OnReady?.Invoke();

// // //         // Lobby 准备好后，如果之前有缓存的房间号，就在这里发起 JoinOrCreate
// // //         if (!string.IsNullOrEmpty(pendingRoomName))
// // //         {
// // //             TryJoinOrCreate();
// // //             pendingRoomName = null;
// // //         }
// // //     }

// // //     public override void OnJoinedRoom()
// // //     {
// // //         Debug.Log($"[PhotonRoomService] 已加入房间：{PhotonNetwork.CurrentRoom.Name}");
// // //         OnJoined?.Invoke(PhotonNetwork.CurrentRoom.Name);
// // //         OnPlayerCountChanged?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
// // //     }

// // //     public override void OnJoinRoomFailed(short returnCode, string message)
// // //     {
// // //         Debug.LogError($"[PhotonRoomService] 加入房间失败：{message}");
// // //         OnJoinFailed?.Invoke(message);
// // //     }

// // //     public override void OnPlayerEnteredRoom(Player newPlayer)
// // //     {
// // //         OnPlayerCountChanged?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
// // //     }

// // //     public override void OnPlayerLeftRoom(Player otherPlayer)
// // //     {
// // //         OnPlayerCountChanged?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
// // //     }
// // //     #endregion
// // // }

// // using System;
// // using Photon.Pun;
// // using Photon.Realtime;
// // using UnityEngine;

// // public class PhotonRoomService : MonoBehaviourPunCallbacks, IRoomService
// // {
// //     // IRoomService 事件
// //     public event Action<string> OnJoined;
// //     public event Action<string> OnJoinFailed;
// //     public event Action<int>    OnPlayerCountChanged;
// //     public event Action        OnReady;            // ← 新增：Lobby 就绪事件

// //     // 缓存用户想加入的房间号
// //     private string pendingRoomName;

// //     // 是否已经连接并准备好加入房间
// //     private bool isReady = false;

// //     /// <summary>加入或创建房间（不限制最大人数）</summary>
// //     public void JoinOrCreate(string roomName)
// //     {
// //         if (string.IsNullOrWhiteSpace(roomName))
// //         {
// //             Debug.LogWarning("[PhotonRoomService] 房间号为空，不发起请求");
// //             return;
// //         }

// //         // 缓存房号，等 ready 再去真正 JoinOrCreate
// //         pendingRoomName = roomName.Trim();
// //         if (isReady)
// //         {
// //             TryJoinOrCreate();
// //             pendingRoomName = null;
// //         }
// //         else
// //         {
// //             Debug.Log($"[PhotonRoomService] 尚未就绪，缓存房间号：{pendingRoomName}");
// //         }
// //     }

// //     /// <summary>
// //     /// 真正调用 PhotonNetwork.JoinOrCreateRoom 的方法，保证只在已加入 Lobby 之后执行
// //     /// </summary>
// //     private void TryJoinOrCreate()
// //     {
// //         Debug.Log($"[PhotonRoomService] 发起 JoinOrCreateRoom：{pendingRoomName}");
// //         PhotonNetwork.JoinOrCreateRoom(
// //             pendingRoomName,
// //             new RoomOptions { MaxPlayers = 0 },
// //             TypedLobby.Default
// //         );
// //     }

// //     /// <summary>离开当前房间</summary>
// //     public void Leave()
// //     {
// //         if (PhotonNetwork.InRoom)
// //             PhotonNetwork.LeaveRoom();
// //     }

// //     #region Photon 回调

// //     public override void OnConnectedToMaster()
// //     {
// //         Debug.Log("[PhotonRoomService] 已连接 Master Server，开始加入 Lobby…");
// //         PhotonNetwork.JoinLobby();  // 必须先加入 Lobby
// //     }

// //     public override void OnJoinedLobby()
// //     {
// //         isReady = true;
// //         Debug.Log("[PhotonRoomService] 已加入默认 Lobby，准备就绪");
// //         OnReady?.Invoke();          // 通知 UI 可以启用 JOIN 按钮

// //         // 如果之前有缓存的房间号，就现在发起
// //         if (!string.IsNullOrEmpty(pendingRoomName))
// //         {
// //             TryJoinOrCreate();
// //             pendingRoomName = null;
// //         }
// //     }

// //     public override void OnJoinedRoom()
// //     {
// //         Debug.Log($"[PhotonRoomService] 已加入房间：{PhotonNetwork.CurrentRoom.Name}");
// //         OnJoined?.Invoke(PhotonNetwork.CurrentRoom.Name);
// //         OnPlayerCountChanged?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
// //     }

// //     public override void OnJoinRoomFailed(short returnCode, string message)
// //     {
// //         Debug.LogError($"[PhotonRoomService] 加入房间失败：{message}");
// //         OnJoinFailed?.Invoke(message);
// //     }

// //     public override void OnPlayerEnteredRoom(Player newPlayer)
// //     {
// //         OnPlayerCountChanged?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
// //     }

// //     public override void OnPlayerLeftRoom(Player otherPlayer)
// //     {
// //         OnPlayerCountChanged?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
// //     }

// //     #endregion
// // }

// using System;
// using Photon.Pun;
// using Photon.Realtime;
// using UnityEngine;

// public class PhotonRoomService : MonoBehaviourPunCallbacks, IRoomService
// {
//     // IRoomService 事件
//     public event Action<string> OnJoined;
//     public event Action<string> OnJoinFailed;
//     public event Action<int>    OnPlayerCountChanged;
//     public event Action        OnReady;            // Lobby 就绪事件

//     // 缓存用户想加入的房间号
//     private string pendingRoomName;
//     // 是否已经加入 Lobby 并准备好匹配
//     private bool isReady = false;

//     void Awake()
//     {
//         // 保证场景切换时不被销毁
//         DontDestroyOnLoad(gameObject);
//     }

//     /// <summary>请求加入或创建房间（等待 OnJoinedLobby 后再真正发起）</summary>
//     public void JoinOrCreate(string roomName)
//     {
//         if (string.IsNullOrWhiteSpace(roomName))
//         {
//             Debug.LogWarning("[PhotonRoomService] 房间号为空，不发起请求");
//             return;
//         }

//         // 缓存房号
//         pendingRoomName = roomName.Trim();
//         if (isReady)
//         {
//             TryJoinOrCreate();
//             pendingRoomName = null;
//         }
//         else
//         {
//             Debug.Log($"[PhotonRoomService] 尚未就绪，缓存房间号：{pendingRoomName}");
//         }
//     }

//     /// <summary>真正调用 PUN 加入/创建房间的地方</summary>
//     private void TryJoinOrCreate()
//     {
//         Debug.Log($"[PhotonRoomService] 发起 JoinOrCreateRoom：{pendingRoomName}");
//         PhotonNetwork.JoinOrCreateRoom(
//             pendingRoomName,
//             new RoomOptions { MaxPlayers = 0 },
//             TypedLobby.Default
//         );
//     }

//     /// <summary>离开当前房间</summary>
//     public void Leave()
//     {
//         if (PhotonNetwork.InRoom)
//             PhotonNetwork.LeaveRoom();
//     }

//     #region Photon 回调

//     public override void OnConnectedToMaster()
//     {
//         Debug.Log("[PhotonRoomService] 已连接 Master Server，开始加入 Lobby…");
//         PhotonNetwork.JoinLobby();  // 一定要先加入 Lobby
//     }

//     public override void OnJoinedLobby()
//     {
//         isReady = true;
//         Debug.Log("[PhotonRoomService] 已加入默认 Lobby，准备就绪");
//         OnReady?.Invoke();

//         // 如果此前用户已点“加入”，就在此刻发起
//         if (!string.IsNullOrEmpty(pendingRoomName))
//         {
//             TryJoinOrCreate();
//             pendingRoomName = null;
//         }
//     }

//     public override void OnJoinedRoom()
//     {
//         Debug.Log($"[PhotonRoomService] 已加入房间：{PhotonNetwork.CurrentRoom.Name}");
//         OnJoined?.Invoke(PhotonNetwork.CurrentRoom.Name);
//         OnPlayerCountChanged?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
//     }

//     public override void OnJoinRoomFailed(short returnCode, string message)
//     {
//         Debug.LogError($"[PhotonRoomService] 加入房间失败：{message}");
//         OnJoinFailed?.Invoke(message);
//     }

//     public override void OnPlayerEnteredRoom(Player newPlayer)
//     {
//         OnPlayerCountChanged?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
//     }

//     public override void OnPlayerLeftRoom(Player otherPlayer)
//     {
//         OnPlayerCountChanged?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
//     }

//     #endregion
// }

// PhotonRoomService.cs
using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonRoomService : MonoBehaviourPunCallbacks, IRoomService
{
    // IRoomService 事件
    public event Action<string> OnJoined;
    public event Action<string> OnJoinFailed;
    public event Action<int>    OnPlayerCountChanged;
    public event Action        OnReady;           // Lobby 就绪事件

    // 缓存待加入的房间号
    private string pendingRoomName;
    // 标记是否已加入 Lobby 并准备好匹配
    private bool isReady = false;

    void Awake()
    {
        // 切场景时不销毁
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 请求加入或创建房间（若未就绪则缓存，等 OnJoinedLobby 再真正执行）
    /// </summary>
    public void JoinOrCreate(string roomName)
    {
        if (string.IsNullOrWhiteSpace(roomName))
        {
            Debug.LogWarning("[PhotonRoomService] 房间号为空，不发起请求");
            return;
        }

        pendingRoomName = roomName.Trim();
        if (isReady)
        {
            TryJoinOrCreate();
            pendingRoomName = null;
        }
        else
        {
            Debug.Log($"[PhotonRoomService] 尚未就绪，缓存房间号：{pendingRoomName}");
        }
    }

    /// <summary>
    /// 真正调用 PUN 加入/创建房间的方法
    /// </summary>
    private void TryJoinOrCreate()
    {
        Debug.Log($"[PhotonRoomService] 发起 JoinOrCreateRoom：{pendingRoomName}");
        PhotonNetwork.JoinOrCreateRoom(
            pendingRoomName,
            new RoomOptions { MaxPlayers = 0 },
            TypedLobby.Default
        );
    }

    /// <summary>离开当前房间</summary>
    public void Leave()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
    }

    #region Photon 回调

    public override void OnConnectedToMaster()
    {
        Debug.Log("[PhotonRoomService] 已连接 Master Server");
        // 仅在尚未加入 Lobby 时才请求 JoinLobby
        if (!PhotonNetwork.InLobby)
        {
            Debug.Log("[PhotonRoomService] 发起 JoinLobby");
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        isReady = true;
        Debug.Log("[PhotonRoomService] 已加入默认 Lobby，准备就绪");
        OnReady?.Invoke();

        if (!string.IsNullOrEmpty(pendingRoomName))
        {
            TryJoinOrCreate();
            pendingRoomName = null;
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"[PhotonRoomService] 已加入房间：{PhotonNetwork.CurrentRoom.Name}");
        OnJoined?.Invoke(PhotonNetwork.CurrentRoom.Name);
        OnPlayerCountChanged?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"[PhotonRoomService] 加入房间失败：{message}");
        OnJoinFailed?.Invoke(message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnPlayerCountChanged?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnPlayerCountChanged?.Invoke(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    #endregion
}

