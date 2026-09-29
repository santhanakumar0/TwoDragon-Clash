using UnityEngine;

// Attach this script to your Main Camera.
// After attaching, drag your player object into the "Target" slot
// that appears in the Inspector.
public class TopDownCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // Your player (e.g. "dragon")

    [Header("Camera Angle Settings")]
    // X = left/right offset, Y = height above player, Z = distance behind player
    // Increase Y and Z for a more zoomed-out, Dota 2-style angle.
    public Vector3 offset = new Vector3(0f, 12f, -8f);
    public float followSpeed = 5f; // How smoothly the camera catches up to the player

    void LateUpdate()
    {
        if (target == null) return;

        // Follow the player at a fixed offset
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // Always look down at the player, keeping the angled top-down view
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
