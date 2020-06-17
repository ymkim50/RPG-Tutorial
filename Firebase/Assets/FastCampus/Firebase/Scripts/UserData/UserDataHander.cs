using FastCampus.InventorySystem.Inventory;
using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;

public class UserDataHander : MonoBehaviour
{
    private DatabaseReference databaseRef;
    private string UserDataPath => "users";
    private string StatsDataPath => "stats";
    private string EquipmentDataPath => "equipment";
    private string InventoryDataPath => "inventory";

    public StatsObject playerStats;
    public InventoryObject playerEquipment;
    public InventoryObject playerInventory;

    // Start is called before the first frame update
    void Start()
    {
        databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void OnClickSave()
    {
        var userId = FirebaseAuthManager.Instance.UserId;
        if (userId == string.Empty)
        {
            return;
        }

        string statsJson = playerStats.ToJson();
        databaseRef.Child(UserDataPath).Child(userId).Child(StatsDataPath).SetRawJsonValueAsync(statsJson).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Save user data was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Save user data encountered an error: " + task.Exception);
                return;
            }

            Debug.LogFormat("Save user data in successfully: {0} ({1})", userId, statsJson);
        });

        string equipmentJson = playerEquipment.ToJson();
        databaseRef.Child(UserDataPath).Child(userId).Child(EquipmentDataPath).SetRawJsonValueAsync(equipmentJson).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Save equipment data was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Save equipment data encountered an error: " + task.Exception);
                return;
            }

            Debug.LogFormat("Save equipment data in successfully: {0} ({1})", userId, equipmentJson);

        });

        string inventoryJson = playerInventory.ToJson();
        databaseRef.Child(UserDataPath).Child(userId).Child(InventoryDataPath).SetRawJsonValueAsync(inventoryJson).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Save inventory data was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Save inventory data encountered an error: " + task.Exception);
                return;
            }

            Debug.LogFormat("Save inventory data in successfully: {0} ({1})", userId, inventoryJson);

        });
    }

    public void OnClickLoad()
    {
        var userId = FirebaseAuthManager.Instance.UserId;
        if (userId == string.Empty)
        {
            return;
        }

        databaseRef.Child(UserDataPath).Child(userId).Child(StatsDataPath).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Load user data was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Load user data encountered an error: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            playerStats.FromJson(snapshot.GetRawJsonValue());
            Debug.LogFormat("Load user data in successfully: {0} ({1})", "user_name", snapshot.GetRawJsonValue());
        });

        databaseRef.Child(UserDataPath).Child(userId).Child(EquipmentDataPath).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Load user data was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Load user data encountered an error: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            playerEquipment.FromJson(snapshot.GetRawJsonValue());
            Debug.LogFormat("Load user data in successfully: {0} ({1})", "user_name", snapshot.GetRawJsonValue());
        });

        databaseRef.Child(UserDataPath).Child(userId).Child(InventoryDataPath).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Load user data was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("Load user data encountered an error: " + task.Exception);
                return;
            }

            DataSnapshot snapshot = task.Result;
            Debug.LogFormat("Load user data in successfully: {0} ({1})", "user_name", snapshot.GetRawJsonValue());
        });
    }
}
