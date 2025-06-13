using UnityEngine;

public class TPSCameraOrbit : MonoBehaviour
{
    public Transform target;
    public float distance = 4.0f;
    public float height = 2.0f;

    public float rotationSpeed = 3.0f;
    public float verticalSpeed = 2.0f;

    private float currentX = 0f;
    private float currentY = 20f;

    public float minY = -20f;
    public float maxY = 80f;

    void LateUpdate()
    {
        if (target == null) return;

        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentY -= Input.GetAxis("Mouse Y") * verticalSpeed;
        currentY = Mathf.Clamp(currentY, minY, maxY);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, height, -distance);

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
