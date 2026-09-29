using UnityEngine;

// Put this on an empty GameObject in your scene (create one, name it "DamagePopupManager").
// Only one of these should exist in the scene.
public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance;

    [Tooltip("Drag your DamagePopup prefab here")]
    public DamagePopup damagePopupPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void SpawnPopup(Vector3 worldPosition, float damage)
    {
        if (damagePopupPrefab == null) return;

        DamagePopup popup = Instantiate(damagePopupPrefab, worldPosition, Quaternion.identity);
        popup.SetDamage(damage);
    }
}
