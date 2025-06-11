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

    // 缓存用户想加入的房间号
    private string pendingRoomName;

    // 是否已经连接并准备好加入房间
    private bool isReady = false;

    /// <summary>加入或创建房间（不限制最大人数）</summary>
    public void JoinOrCreate(string roomName)
    {
        if (string.IsNullOrWhiteSpace(roomName))
            return;

        // 如果还没准备好，就先缓存，等回调再调用
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            pendingRoomName = roomName;
            Debug.Log($"[PhotonRoomService] 尚未准备就绪，缓存房间号：{roomName}");
            return;
        }

        Debug.Log($"[PhotonRoomService] 直接发起加入/创建房间：{roomName}");
        var options = new RoomOptions { MaxPlayers = 0 };
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
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
        isReady = true;
        Debug.Log("[PhotonRoomService] 已连接 Master Server，准备就绪");

        // 如果之前用户点击过加入，就立即执行
        if (!string.IsNullOrEmpty(pendingRoomName))
        {
            string roomToJoin = pendingRoomName;
            pendingRoomName = null;
            JoinOrCreate(roomToJoin);
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
