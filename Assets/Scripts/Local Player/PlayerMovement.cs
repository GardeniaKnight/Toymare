// using UnityEngine;

// public class PlayerMovement : MonoBehaviour
// {
//     public float speed = 6f;

//     Vector3 movement;
//     Animator anim;
//     Rigidbody playerRigidbody;
//     int floorMask;
//     float camRayLength = 100f;

//     Camera mainCamera;
//     void Start()
//     {
//         mainCamera = Camera.main;
//     }
//     void Awake()
//     {
//         floorMask = LayerMask.GetMask("Floor");
//         anim = GetComponent<Animator>();
//         playerRigidbody = GetComponent<Rigidbody>();
//         mainCamera = Camera.main;
//     }

//     void FixedUpdate()
//     {
//         if (mainCamera == null)
//         {
//             mainCamera = Camera.main;
//             if (mainCamera == null) return; // 等待下一帧
//         }
//         float h = Input.GetAxisRaw("Horizontal");
//         float v = Input.GetAxisRaw("Vertical");

//         Move(h, v);
//         Turning();
//         Animating(h, v);
//     }

//     void Move(float h, float v)
//     {
//         // 获取摄像机正前方向（去掉 Y 分量）
//         Vector3 camForward = mainCamera.transform.forward;
//         camForward.y = 0f;
//         camForward.Normalize();

//         // 获取摄像机右方向（横向）
//         Vector3 camRight = mainCamera.transform.right;
//         camRight.y = 0f;
//         camRight.Normalize();

//         // 基于摄像机方向构建移动向量
//         Vector3 desiredMove = (camForward * v + camRight * h).normalized;

//         movement = desiredMove * speed * Time.deltaTime;

//         playerRigidbody.MovePosition(transform.position + movement);
//     }

//     void Turning()
//     {
//         Vector3 forward = mainCamera.transform.forward;
//         forward.y = 0f;
//         forward.Normalize();

//         if (forward.sqrMagnitude > 0.01f)
//         {
//             Quaternion targetRotation = Quaternion.LookRotation(forward);
//             playerRigidbody.MoveRotation(targetRotation);
//         }
//     }


//     void Animating(float h, float v)
//     {
//         bool walking = h != 0f || v != 0f;
//         anim.SetBool("IsWalking", walking);
//     }
// }

using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    [Header("移动速度")]
    public float speed = 6f;

    // 私有字段
    Vector3 movement;
    Animator anim;
    Rigidbody playerRigidbody;
    int floorMask;
    Camera mainCamera;

    void Awake()
    {

        // 只要挂了 PhotonView 且又不是本地，就立刻禁用组件
        if (photonView != null && !photonView.IsMine)
        {
            enabled = false;
            return;
        }
        // 初始化组件引用（单/多人都需要）
        anim            = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        floorMask       = LayerMask.GetMask("Floor");
        mainCamera      = Camera.main;

        
    }

    void FixedUpdate()
    {
        // 只有启用脚本后才响应输入
        if (!enabled) return;

        // 在多人模式下，这里一定是本地玩家；在单人模式也可以直接执行
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Move(h, v);
        Turning();
        Animating(h, v);
    }

    void Move(float h, float v)
    {
        // 确保有摄像机引用
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (mainCamera == null)
            return;

        // 计算基于摄像机朝向的移动
        Vector3 camForward = mainCamera.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = mainCamera.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 desiredMove = (camForward * v + camRight * h).normalized;
        movement = desiredMove * speed * Time.deltaTime;

        // 实际移动
        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Turning()
    {
        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(forward);
        playerRigidbody.MoveRotation(targetRotation);
    }

    void Animating(float h, float v)
    {
        bool walking = (h != 0f || v != 0f);
        anim.SetBool("IsWalking", walking);
    }
}

