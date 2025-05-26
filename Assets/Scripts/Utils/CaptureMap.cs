using UnityEngine;

public class CaptureTopDownMap : MonoBehaviour
{
    public Camera topDownCamera;  // 用于渲染俯视图的摄像机

    void Start()
    {
        // 确保摄像机已设置
        if (topDownCamera != null)
        {
            // 截取并保存截图
            string filePath = "Assets/TopDownMap.png";
            ScreenCapture.CaptureScreenshot(filePath);
            Debug.Log("Screenshot saved at: " + filePath);
        }
        else
        {
            Debug.LogError("Top-down camera is not assigned!");
        }
    }
}
