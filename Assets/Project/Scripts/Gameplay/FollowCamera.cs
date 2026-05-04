using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FollowCamera : MonoBehaviour
{
    public Transform player;      // Drag player here in Inspector
//public float smoothTime = 0.3f; // Delay before camera catches up
//public Vector3 offset = new Vector3(0, 0, -10); // Maintain distance on Z-axis

    //private Vector3 velocity = Vector3.zero;
    // add bounds values here.
    [SerializeField] Collider2D map;
    private Vector3 bottomLimit;
    private Vector3 topLimit;

    private float halfHeight;
    private float halfWidth;

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;


    // add camera bounds here.

    void Awake()
    {
        player = FindAnyObjectByType<PlayerController>().transform;
        minX = map.bounds.min.x;
        minY = map.bounds.min.y;
        maxX = map.bounds.max.x;
        maxY = map.bounds.max.y;
    }
    private void Start()
    {

        // if this do not work, get the camera
        bottomLimit = map.bounds.min;
        topLimit = map.bounds.max;
        // offsets
        halfHeight = Camera.main.orthographicSize;
        halfWidth = Camera.main.aspect * halfHeight;

    }
    void LateUpdate()
{
    if (player != null)
    {
            // Target position based on player + initial offset
            //Vector3 targetPosition = player.position + offset;

            //// Smoothly move the camera toward that target
            //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            transform.position = new Vector3(Mathf.Clamp(player.transform.position.x, bottomLimit.x, topLimit.x),
                   Mathf.Clamp(player.transform.position.y, bottomLimit.y, topLimit.y),
                   transform.position.z);
        }
}

}
