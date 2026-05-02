using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform player;      // Drag player here in Inspector
public float smoothTime = 0.3f; // Delay before camera catches up
public Vector3 offset = new Vector3(0, 0, -10); // Maintain distance on Z-axis

private Vector3 velocity = Vector3.zero;
// add bounds values here.

// add camera bounds here.
void LateUpdate()
{
    if (player != null)
    {
        // Target position based on player + initial offset
        Vector3 targetPosition = player.position + offset;

        // Smoothly move the camera toward that target
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
}
