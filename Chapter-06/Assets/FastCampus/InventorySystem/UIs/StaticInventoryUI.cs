using FastCampus.InventorySystem.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FastCampus.InventorySystem.UIs
{
    public class StaticInventoryUI : InventoryUI
    {
        public GameObject[] staticSlots = null;

        public override void CreateSlots()
        {
            slotUIs = new Dictionary<GameObject, Inventory.InventorySlot>();
            for (int i = 0; i < inventoryObject.Slots.Length; i++)
            {
                GameObject slotGO = staticSlots[i];

                AddEvent(slotGO, EventTriggerType.PointerEnter, delegate { OnEnter(slotGO); });
                AddEvent(slotGO, EventTriggerType.PointerExit, delegate { OnExit(slotGO); });
                AddEvent(slotGO, EventTriggerType.BeginDrag, delegate { OnStartDrag(slotGO); });
                AddEvent(slotGO, EventTriggerType.EndDrag, delegate { OnEndDrag(slotGO); });
                AddEvent(slotGO, EventTriggerType.Drag, delegate { OnDrag(slotGO); });

                inventoryObject.Slots[i].slotUI = slotGO;
                slotUIs.Add(slotGO, inventoryObject.Slots[i]);
            }
        }
    }
}
