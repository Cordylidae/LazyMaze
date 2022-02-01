using UnityEngine;

public class CameraLerpToTransform : MonoBehaviour
{
    public Transform target;
    public float speed;
    public float cameraDepth = -10f;
    public float minX, minY, maxX, maxY;

    void FixedUpdate()
    {
        var newPosition = Vector2.Lerp(transform.position, target.position, speed * Time.deltaTime);
        var camPosition = new Vector3(newPosition.x, newPosition.y, cameraDepth);
        var v3 = camPosition;
        var newX = Mathf.Clamp(v3.x, minX, maxX);
        var newY = Mathf.Clamp(v3.y, minY, maxY);
        transform.position = new Vector3(newX, newY, cameraDepth);

    }
}
