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
using Photon.Pun;  // 新增

public class PlayerMovement : MonoBehaviourPun
{
    public float speed = 6f;

    Vector3 movement;
    Animator anim;
    Rigidbody playerRigidbody;
    int floorMask;
    float camRayLength = 100f;

    Camera mainCamera;

    void Awake()
    {
        // 如果这不是本地玩家，禁用脚本
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }

        floorMask = LayerMask.GetMask("Floor");
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    void Start()
    {
        // 再次保证主摄像机引用
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        // 只在本地玩家实例上执行
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Move(h, v);
        Turning();
        Animating(h, v);
    }

    void Move(float h, float v)
    {
        Vector3 camForward = mainCamera.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = mainCamera.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 desiredMove = (camForward * v + camRight * h).normalized;
        movement = desiredMove * speed * Time.deltaTime;
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
