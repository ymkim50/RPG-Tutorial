using UnityEngine;
using System.Collections;
using FastCampus.Core;
using FastCampus.InventorySystem.Items;
using FastCampus.Characters;

public class PickupItem : MonoBehaviour, IInteractable
{
    public float distance = 3.0f;
    public ItemObject itemObject;

    public float Distance => distance;

    public void Interact(GameObject other)
    {
        float calcDistance = Vector3.Distance(transform.position, other.transform.position);
        if (calcDistance > distance)
        {
            return;
        }

        PlayerCharacter playerCharacter = other.GetComponent<PlayerCharacter>();
        if (playerCharacter?.PickupItem(itemObject) ?? false)
        {
            Destroy(gameObject);            
        }
    }

    public void StopInteract(GameObject other)
    {
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        GetComponent<SpriteRenderer>().sprite = itemObject.icon;
#endif  // UNITY_EDITOR
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distance);
    }
}
