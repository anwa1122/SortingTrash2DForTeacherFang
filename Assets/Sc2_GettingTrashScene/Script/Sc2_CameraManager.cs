using UnityEngine;

public class Sc2_CameraManager : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector2 offset = new Vector2(0f, 3f); // ← ปรับ Y ตรงนี้เลย

    void LateUpdate()
    {
        Vector3 targetPosition = new Vector3(
            target.position.x + offset.x,
            target.position.y + offset.y, // ← กล้องจะอยู่สูงกว่าตัวละคร
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}