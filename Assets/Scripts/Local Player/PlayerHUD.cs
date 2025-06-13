using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩家血条 HUD：
/// 在本地 PlayerHealth 初始化时自动绑定，无需网络判断。
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    [Header("HP Sliders")]
    public Slider foreground;   // 亮条
    public Slider background;   // 暗条
}
