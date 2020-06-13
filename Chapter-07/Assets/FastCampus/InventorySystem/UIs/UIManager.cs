using FastCampus.InventorySystem.Inventory;
using FastCampus.InventorySystem.Items;
using FastCampus.InventorySystem.UIs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public ItemDatabaseObject itemDatabase;

    public StaticInventoryUI equipmentUI;
    public DynamicInventoryUI inventoryUI;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown("i"))
        {
            inventoryUI.gameObject.SetActive(!inventoryUI.gameObject.activeSelf);
        }

        if (Input.GetKeyDown("e"))
        {
            equipmentUI.gameObject.SetActive(!equipmentUI.gameObject.activeSelf);
        }
    }

    public void AddNewItem()
    {
        if (itemDatabase.itemObjects.Length > 0)
        {
            ItemObject newItemObject = itemDatabase.itemObjects[Random.Range(0, itemDatabase.itemObjects.Length - 1)];
            //ItemObject newItemObject = itemDatabase.itemObjects[itemDatabase.itemObjects.Length - 1];
            Item newItem = new Item(newItemObject);

            inventoryUI.inventoryObject.AddItem(newItem, 1);
        }
    }

    public void ClearInventory()
    {
        inventoryUI.inventoryObject.Clear();
        equipmentUI.inventoryObject.Clear();
    }
}
