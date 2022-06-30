using System;
using UnityEngine;

/// <summary>
/// The base component for all buildings. Holds information for the UI sprite, Id, price and name of the building.
/// </summary>
public class Building : NetworkBehaviour
{
    [SerializeField] private Sprite icon = null;
    [SerializeField] private int price = 100;
    [SerializeField] private GameObject buildingPreview = null;
    [SerializeField] private string buildingName = "";

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

    public string GetBuildingName()
    {
        return buildingName;
    }

    public GameObject GetBuildingPreview()
    {
        return buildingPreview;
    }

    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetPrice()
    {
        return price;
    }
}
