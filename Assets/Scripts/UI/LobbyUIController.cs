// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;
// using System.Collections;

// public class LobbyUIController : MonoBehaviour
// {
//     [Header("UI 绑定")]
//     public InputField roomIdInput;
//     public Button     joinButton;
//     public Button     cancelButton;
//     public Text       statusText;

//     [Header("提示自动清除时间（秒）")]
//     public float statusClearTime = 3f;

//     private IRoomService roomService;
//     private bool         isReady = false;
//     private Coroutine    clearCoroutine;

//     void Awake()
//     {
//         roomService = FindObjectOfType<PhotonRoomService>() as IRoomService;
//         if (roomService == null)
//         {
//             Debug.LogError("找不到 PhotonRoomService，请确认已挂载在场景中");
//             return;
//         }

//         // 初始提示，按钮始终可点
//         ShowStatus("正在连接服务器…");

//         // 订阅 Lobby 就绪事件
//         var prs = roomService as PhotonRoomService;
//         if (prs != null)
//         {
//             prs.OnReady += OnServiceReady;
//         }

//         // 按钮回调
//         joinButton.onClick.AddListener(OnJoinClicked);
//         cancelButton.onClick.AddListener(OnCancelClicked);

//         // 房间事件
//         roomService.OnJoined            += HandleJoined;
//         roomService.OnJoinFailed        += HandleJoinFailed;
//         roomService.OnPlayerCountChanged += HandlePlayerCountChanged;
//     }

//     void OnDestroy()
//     {
//         if (roomService != null)
//         {
//             roomService.OnJoined            -= HandleJoined;
//             roomService.OnJoinFailed        -= HandleJoinFailed;
//             roomService.OnPlayerCountChanged -= HandlePlayerCountChanged;

//             var prs = roomService as PhotonRoomService;
//             if (prs != null)
//                 prs.OnReady -= OnServiceReady;
//         }

//         joinButton.onClick.RemoveListener(OnJoinClicked);
//         cancelButton.onClick.RemoveListener(OnCancelClicked);
//     }

//     private void OnServiceReady()
//     {
//         isReady = true;
//         ShowStatus("服务器已就绪，请输入房间号");
//     }

//     void OnJoinClicked()
//     {
//         if (!isReady)
//         {
//             ShowStatus("服务器初始化中，请稍候…");
//             return;
//         }

//         string roomName = roomIdInput.text.Trim();
//         if (string.IsNullOrEmpty(roomName))
//         {
//             ShowStatus("房间号不能为空");
//             return;
//         }

//         ShowStatus("正在加入或创建房间…");
//         roomService.JoinOrCreate(roomName);
//     }

//     void OnCancelClicked()
//     {
//         roomService.Leave();
//         SceneManager.LoadScene("MainMenu");
//     }

//     void HandleJoined(string roomName)
//     {
//         GameModeManager.CurrentMode = GameMode.MultiPlayer;
//         ShowStatus($"已加入房间：{roomName}，等待其他玩家…");
//     }

//     void HandleJoinFailed(string reason)
//     {
//         ShowStatus($"加入失败：{reason}");
//     }

//     void HandlePlayerCountChanged(int count)
//     {
//         ShowStatus($"房间人数：{count} 人");
//     }

//     private void ShowStatus(string message)
//     {
//         statusText.text = message;
//         if (clearCoroutine != null)
//             StopCoroutine(clearCoroutine);
//         clearCoroutine = StartCoroutine(ClearStatusAfterDelay());
//     }

//     private IEnumerator ClearStatusAfterDelay()
//     {
//         yield return new WaitForSeconds(statusClearTime);
//         statusText.text = string.Empty;
//         clearCoroutine = null;
//     }
// }

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Pun;

public class LobbyUIController : MonoBehaviour
{
    [Header("UI 绑定")]
    public InputField nameInput;      // 新增：玩家昵称输入框
    public InputField roomIdInput;    // 房间号输入框
    public Button     joinButton;     // 加入/创建按钮
    public Button     cancelButton;   // 取消按钮
    public Text       statusText;     // 提示文本

    [Header("提示自动清除时间（秒）")]
    public float statusClearTime = 3f;

    IRoomService roomService;
    bool isReady = false;
    Coroutine clearCoroutine;

    void Awake()
    {
        roomService = FindObjectOfType<PhotonRoomService>() as IRoomService;
        if (roomService == null)
        {
            Debug.LogError("找不到 PhotonRoomService，请确认已挂载在场景中");
            return;
        }

        // 初始化：按钮禁用（直到服务就绪且两个输入框都有内容），提示“正在连接服务器…”
        joinButton.interactable = false;
        ShowStatus("正在连接服务器…");

        // 订阅 Lobby 就绪事件
        var prs = roomService as PhotonRoomService;
        if (prs != null)
            prs.OnReady += OnServiceReady;

        // 当昵称或房号输入变化时，刷新按钮状态
        nameInput.onValueChanged.AddListener(_ => RefreshJoinButton());
        roomIdInput.onValueChanged.AddListener(_ => RefreshJoinButton());

        // 按钮事件
        joinButton.onClick.AddListener(OnJoinClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);

        // 房间事件
        roomService.OnJoined             += HandleJoined;
        roomService.OnJoinFailed         += HandleJoinFailed;
        roomService.OnPlayerCountChanged += HandlePlayerCountChanged;
    }

    void OnDestroy()
    {
        // 解绑
        if (roomService != null)
        {
            roomService.OnJoined             -= HandleJoined;
            roomService.OnJoinFailed         -= HandleJoinFailed;
            roomService.OnPlayerCountChanged -= HandlePlayerCountChanged;

            var prs = roomService as PhotonRoomService;
            if (prs != null)
                prs.OnReady -= OnServiceReady;
        }

        joinButton.onClick.RemoveListener(OnJoinClicked);
        cancelButton.onClick.RemoveListener(OnCancelClicked);
        nameInput.onValueChanged.RemoveAllListeners();
        roomIdInput.onValueChanged.RemoveAllListeners();
    }

    /// <summary>
    /// Lobby 服务就绪时回调
    /// </summary>
    void OnServiceReady()
    {
        isReady = true;
        ShowStatus("服务器已就绪，请输入昵称和房间号");
        RefreshJoinButton();
    }

    /// <summary>
    /// 刷新“Join”按钮的可用性：只有当服务就绪、昵称和房号都非空时才可点
    /// </summary>
    void RefreshJoinButton()
    {
        bool haveName = !string.IsNullOrWhiteSpace(nameInput.text);
        bool haveRoom = !string.IsNullOrWhiteSpace(roomIdInput.text);
        joinButton.interactable = isReady && haveName && haveRoom;
    }

    void OnJoinClicked()
    {
        // 再次校验
        if (!isReady)
        {
            ShowStatus("服务器初始化中，请稍候…");
            return;
        }
        string nick = nameInput.text.Trim();
        string room = roomIdInput.text.Trim();
        if (string.IsNullOrEmpty(nick) || string.IsNullOrEmpty(room))
        {
            ShowStatus("请先输入昵称和房间号");
            return;
        }

        // 设置 Photon 昵称
        PhotonNetwork.NickName = nick;

        ShowStatus("正在加入或创建房间…");
        roomService.JoinOrCreate(room);
    }

    void OnCancelClicked()
    {
        roomService.Leave();
        SceneManager.LoadScene("MainMenu");
    }

    void HandleJoined(string roomName)
    {
        GameModeManager.CurrentMode = GameMode.MultiPlayer;
        ShowStatus($"已加入房间：{roomName}（{PhotonNetwork.NickName}），等待其他玩家…");
    }

    void HandleJoinFailed(string reason)
    {
        ShowStatus($"加入失败：{reason}");
    }

    void HandlePlayerCountChanged(int count)
    {
        ShowStatus($"房间人数：{count} 人");
    }

    void ShowStatus(string message)
    {
        statusText.text = message;
        if (clearCoroutine != null)
            StopCoroutine(clearCoroutine);
        clearCoroutine = StartCoroutine(ClearStatusAfterDelay());
    }

    IEnumerator ClearStatusAfterDelay()
    {
        yield return new WaitForSeconds(statusClearTime);
        statusText.text = string.Empty;
        clearCoroutine = null;
    }
}
