using System;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField] private Sprite icon = null;
    [SerializeField] private int buildingID = -1;
    [SerializeField] private int price = 100;
    [SerializeField] private GameObject buildingPreview = null;

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;

    private void Start()
    {
        if (IsServer)
        {
            ServerOnBuildingSpawned?.Invoke(this);
        }

        if (IsOwner)
        {
            AuthorityOnBuildingSpawned?.Invoke(this);
        }
    }

    private void OnDestroy()
    {

        if (IsServer)
        {
            ServerOnBuildingDespawned?.Invoke(this);
        }

        if (IsOwner)
        {
            AuthorityOnBuildingDespawned?.Invoke(this);
        }

    }

    public GameObject GetBuildingPreview()
    {
        return buildingPreview;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetID()
    {
        return buildingID;
    }

    public int GetPrice()
    {
        return price;
    }

    /*public int GetPrefabId()
    {
        return prefabId;
    }*/
}
