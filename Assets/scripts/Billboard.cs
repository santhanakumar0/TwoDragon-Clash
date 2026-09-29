using UnityEngine;

// Attach this to the World Space Canvas (child of the dragon).
// Keeps the health bar always facing the camera, no matter which way the dragon turns.
public class Billboard : MonoBehaviour
{
    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (cam != null)
            transform.rotation = cam.transform.rotation;
    }
}
