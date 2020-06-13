using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInstances
{
    public List<Transform> items = new List<Transform>();

    public void Destroy()
    {
        foreach (Transform item in items)
        {
            GameObject.Destroy(item.gameObject);
        }
    }
}
