using UnityEngine;
using TMPro;

// Attach this to your floating damage-number PREFAB.
// The prefab should be a "3D Object > Text - TextMeshPro" GameObject (not the UI canvas kind).
public class DamagePopup : MonoBehaviour
{
    public float floatSpeed = 1.5f;
    public float lifetime = 1f;
    public Color normalColor = Color.white;
    public Color heavyColor = new Color(1f, 0.3f, 0.1f); // orange-red, for big hits
    public float heavyThreshold = 15f;

    TextMeshPro textMesh;
    float timer;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void SetDamage(float amount)
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshPro>();
        textMesh.text = Mathf.RoundToInt(amount).ToString();
        textMesh.color = amount >= heavyThreshold ? heavyColor : normalColor;
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Always face the camera so the number is readable from the top-down angle.
        if (Camera.main != null)
            transform.rotation = Camera.main.transform.rotation;

        timer += Time.deltaTime;
        if (textMesh != null)
        {
            Color c = textMesh.color;
            c.a = Mathf.Lerp(1f, 0f, timer / lifetime);
            textMesh.color = c;
        }

        if (timer >= lifetime)
            Destroy(gameObject);
    }
}
