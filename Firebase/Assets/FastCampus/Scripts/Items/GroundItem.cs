using FastCampus.InventorySystem.Items;
using UnityEngine;

public class GroundItem : MonoBehaviour
{
    public ItemObject itemObject;

    private void OnValidate()
    {
#if UNITY_EDITOR
        GetComponent<SpriteRenderer>().sprite = itemObject.icon;
#endif  // UNITY_EDITOR
    }
}
