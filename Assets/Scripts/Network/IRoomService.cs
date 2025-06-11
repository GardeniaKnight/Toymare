using System;

public interface IRoomService
{
    /// <summary>请求加入或创建指定房间</summary>
    void JoinOrCreate(string roomName);

    /// <summary>离开当前房间</summary>
    void Leave();

    /// <summary>成功加入房间时触发，参数是房间名</summary>
    event Action<string> OnJoined;

    /// <summary>加入失败时触发，参数是失败原因</summary>
    event Action<string> OnJoinFailed;

    /// <summary>房间里玩家数变化时触发，参数是当前人数</summary>
    event Action<int> OnPlayerCountChanged;
}
