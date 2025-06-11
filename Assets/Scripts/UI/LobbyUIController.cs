using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LobbyUIController : MonoBehaviour
{
    [Header("UI 绑定")]
    public InputField roomIdInput;
    public Button     joinButton;
    public Button     cancelButton;
    public Text       statusText;

    [Header("提示自动清除时间 (秒)")]
    public float statusClearTime = 3f;

    private IRoomService roomService;
    private Coroutine clearCoroutine;

    void Awake()
    {
        // 获取房间服务实现
        roomService = FindObjectOfType<PhotonRoomService>() as IRoomService;
        if (roomService == null)
        {
            Debug.LogError("找不到 PhotonRoomService，请确认已挂载在 PhotonBootstrap 上");
        }

        // 按钮事件绑定
        joinButton.onClick.AddListener(OnJoinClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);

        // 订阅房间服务事件
        roomService.OnJoined            += HandleJoined;
        roomService.OnJoinFailed        += HandleJoinFailed;
        roomService.OnPlayerCountChanged += HandlePlayerCountChanged;

        // 初始提示
        ShowStatus("请输入房间号");
    }

    void OnDestroy()
    {
        // 解绑，防止内存泄漏
        joinButton.onClick.RemoveListener(OnJoinClicked);
        cancelButton.onClick.RemoveListener(OnCancelClicked);

        roomService.OnJoined            -= HandleJoined;
        roomService.OnJoinFailed        -= HandleJoinFailed;
        roomService.OnPlayerCountChanged -= HandlePlayerCountChanged;
    }

    void OnJoinClicked()
    {
        string roomName = roomIdInput.text.Trim();
        if (string.IsNullOrEmpty(roomName))
        {
            ShowStatus("房间号不能为空");
            return;
        }
        ShowStatus("正在加入或创建房间...");
        roomService.JoinOrCreate(roomName);
    }

    void OnCancelClicked()
    {
        roomService.Leave();
        SceneManager.LoadScene("MainMenu");
    }

    void HandleJoined(string roomName)
    {
        ShowStatus($"已加入房间：{roomName}，等待其他玩家...");
    }

    void HandleJoinFailed(string reason)
    {
        ShowStatus($"加入失败：{reason}");
    }

    void HandlePlayerCountChanged(int count)
    {
        ShowStatus($"房间人数：{count} 人");
    }

    /// <summary>
    /// 显示状态信息并在延迟后自动清除
    /// </summary>
    private void ShowStatus(string message)
    {
        statusText.text = message;
        if (clearCoroutine != null)
        {
            StopCoroutine(clearCoroutine);
        }
        clearCoroutine = StartCoroutine(ClearStatusAfterDelay());
    }

    /// <summary>
    /// 清除状态信息的协程
    /// </summary>
    private IEnumerator ClearStatusAfterDelay()
    {
        yield return new WaitForSeconds(statusClearTime);
        statusText.text = string.Empty;
        clearCoroutine = null;
    }
}
