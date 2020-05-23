using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum ItemType : int
{
    Head,
    Upper,
    Shoulder,
    Arm,
    Lower,
    Leg,
    LeftWeapon,
    RightWeapon,
}

[Serializable]
public struct ItemData
{
    public uint id;
    public ItemType itemType;
    public string name;
    public string description;
    public GameObject itemPrefab;
    public Sprite icon;
}

[CreateAssetMenu]
public class ItemDatas : ScriptableObject
{
    public List<ItemData> itemDatas = new List<ItemData>();
}