using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Item
{
    private readonly ItemType itemType;

    Item(ItemType itemType)
    {
        this.itemType = itemType;
    }
}
