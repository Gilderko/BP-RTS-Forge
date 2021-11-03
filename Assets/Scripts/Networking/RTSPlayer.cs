using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Unity;
using Forge.Networking.Unity.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField]
    private LayerMask buildingBlockCollisionLayer = new LayerMask();

    [SerializeField]
    private LayerMask buildingKeepDistanceLayer = new LayerMask();

    [SerializeField]
    private Building[] buildings = new Building[0];

    [SerializeField]
    private float buildingRangeLimit = 10f;

    [SerializeField]
    private float buildingFromEnemyLimit = 5f;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private Vector2 cameraStartOffset = new Vector2(-5, -5);

    private INetPlayer forgePlayer;

    private MessagePool<UpdateResourcesMessage> resourcesPool = new MessagePool<UpdateResourcesMessage>();
    private MessagePool<SpawnEntityMessage> spawnPool = new MessagePool<SpawnEntityMessage>();

    // Synced values

    private int resources = 0;    

    private bool isPartyOwner = false;

    private string playerName = "";

    private Vector3 cameraStartPosition = new Vector3(0, 0, 21);

    // Synced values

    public event Action<int> ClientOnResourcesUpdated;
    public event Action<Color> ClientOnColorUpdated;

    public static event Action ClientOnInfoUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerChanged;

    private Color teamColor = new Color();

    [SerializeField]
    private List<Unit> myUnits = new List<Unit>();

    [SerializeField]
    private List<Building> myBuildings = new List<Building>();

    public void Start()
    {
        RTSNetworkManager.Instance.Players.Add(this);        

        if (IsServer)
        {
            OnStartServer();
        }

        if (IsClient)
        {
            OnStartAuthority();
        }
    }

    public void OnDestroy()
    {

        if (IsServer)
        {
            OnStopServer();
        }

        if (IsClient)
        {
            OnStopClient();
        }
    }


    #region Server

    public void OnStartServer()
    {
        forgePlayer = RTSNetworkManager.Instance.Facade.NetworkMediator.PlayerRepository.GetPlayer(OwnerSignatureId);

        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
    }

    public void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;

        RTSNetworkManager.Instance.Players.Remove(this);
    }

    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.OwnerClientId != OwnerClientId) { return; }

        myBuildings.Remove(building);
    }

    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.OwnerClientId != OwnerClientId) { return; }

        myBuildings.Add(building);
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        // Check if the same person who owns this player also owns this unit
        if (unit.OwnerClientId != OwnerClientId)
        {
            return;
        }

        myUnits.Add(unit);
    }    

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.OwnerClientId != OwnerClientId)
        {
            return;
        }

        myUnits.Remove(unit);
    }

    public void ServerAddResources(int resourcesToAdd)
    {
        resources += resourcesToAdd;

        var resourceMessage = resourcesPool.Get();
        resourceMessage.PlayerId = OwnerClientId;
        resourceMessage.ResourcesValue = resources;        

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendMessage(resourceMessage, forgePlayer);
    }

    public void ServerSetPlayerName(string displayName)
    {
        playerName = displayName;

        var message = new ChangePlayerNameMessage();
        message.NewPlayerName = playerName;
        message.PlayerId = OwnerClientId;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(message);
    }

    public void ServerSetTeamColor(Color newColor)
    {
        teamColor = newColor;

        var message = new SetTeamColorMessage();
        message.PlayerId = OwnerClientId;
        message.ColorR = teamColor.r;
        message.ColorG = teamColor.g;
        message.ColorB = teamColor.b;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(message);
    }

    public void ServerSetTeamOwner(bool state)
    {
        isPartyOwner = state;

        var message = new ChangePlayerSessionOwnershipMessage();
        message.IsOwner = isPartyOwner;
        message.PlayerId = OwnerClientId;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(message);
    }

    public void ServerSetStartingCameraPosition(Vector3 pos)
    {
        cameraStartPosition = pos;

        var message = new SetPlayerCameraStartPositionMessage();
        message.PosX = pos.x;
        message.PosY = pos.y;
        message.PosZ = pos.z;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(message, forgePlayer);
    }

    public void CmdStartGameServerRpc()
    {
        if (!isPartyOwner)
        {
            return;
        }

        RTSNetworkManager.Instance.StartGame();
    }


    public void CmdTryPlaceBuildingServerRpc(int buildingID, Vector3 positionToSpawn)
    {
        Building buildingToPlace = buildings.First(build => build.GetComponent<NetworkEntity>().PrefabId == buildingID);

        if (buildingToPlace == null)
        {
            return;
        }

        if (resources < buildingToPlace.GetPrice())
        {
            return;
        }

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

        if (!CanPlaceBuilding(buildingCollider, positionToSpawn))
        {
            return;
        }

        var spawnBuildMessage = spawnPool.Get();
        spawnBuildMessage.Id = RTSNetworkManager.Instance.ServerGetNewEntityId();
        spawnBuildMessage.OwnerId = OwnerSignatureId;
        spawnBuildMessage.PrefabId = buildingToPlace.GetComponent<NetworkEntity>().PrefabId;

        spawnBuildMessage.Position = positionToSpawn;
        spawnBuildMessage.Rotation = Quaternion.identity;
        spawnBuildMessage.Scale = Vector3.one;

        EntitySpawner.SpawnEntityFromMessage(RTSNetworkManager.Instance.Facade, spawnBuildMessage);

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(spawnBuildMessage);

        ServerAddResources(-buildingToPlace.GetPrice());
    }

    #endregion

    #region Client


    public void OnStartAuthority()
    {
        if (!IsOwner)
        {
            return;
        }

        RTSNetworkManager.Instance.ClientSetLocalPlayer(this);

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!IsClient)
        {
            return;
        }

        RTSNetworkManager.Instance.Players.Remove(this);

        if (!IsOwner)
        {
            return;
        }

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    public void ClientSetPlayerOwnsSession(bool newState)
    {
        if (!IsOwner)
        {
            return;
        }

        isPartyOwner = newState;
        AuthorityOnPartyOwnerChanged?.Invoke(newState);
    }

    public void ClientSetNewPlayerName(string newVal)
    {
        playerName = newVal;
        ClientOnInfoUpdated?.Invoke();
    }

    public void ClientSetCameraStartingPosition(Vector3 newVec)
    {
        if (!IsOwner)
        {
            return;
        }

        cameraStartPosition = newVec;
        cameraTransform.position = new Vector3(newVec.x + cameraStartOffset.x, 21, newVec.z + cameraStartOffset.y);
    }

    public void ClientSetNewTeamColor(Color newCol)
    {
        teamColor = newCol;

        ClientOnColorUpdated?.Invoke(teamColor);
    }

    public void ClientSetResources(int newValue)
    {
        resources = newValue;
        ClientOnResourcesUpdated?.Invoke(newValue);
    }    

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        myUnits.Remove(unit);
    }

    private void AuthorityHandleBuildingDespawned(Building building)
    {
        myBuildings.Remove(building);
    }

    private void AuthorityHandleBuildingSpawned(Building building)
    {
        myBuildings.Add(building);
    }

    #endregion

    public int GetResources()
    {
        return resources;
    }

    public IEnumerable<Unit> GetMyUnits()
    {
        return myUnits;
    }

    public IEnumerable<Building> GetMyBuildings()
    {
        return myBuildings;
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 positionToSpawn)
    {
        if (Physics.CheckBox(
            positionToSpawn + buildingCollider.center,
            buildingCollider.size / 2,
            Quaternion.identity,
            buildingBlockCollisionLayer))
        {
            return false;
        }

        RaycastHit[] hits = Physics.SphereCastAll(positionToSpawn, buildingFromEnemyLimit, Vector3.up, buildingKeepDistanceLayer);
        foreach (RaycastHit hit in hits)
        {
            Unit possibleUnit = hit.transform.GetComponent<Unit>();

            if (possibleUnit != null)
            {
                bool hasAuth = IsClient ? possibleUnit.IsOwner : possibleUnit.OwnerClientId == OwnerClientId;
                if (!hasAuth)
                {
                    return false;
                }
            }

            Building possibleBuilding = hit.transform.GetComponent<Building>();
            if (possibleBuilding != null)
            {
                bool hasAuth = IsClient ? possibleBuilding.IsOwner : possibleBuilding.OwnerClientId == OwnerClientId;
                if (!hasAuth)
                {
                    return false;
                }
            }
        }

        foreach (Building build in myBuildings)
        {
            if ((positionToSpawn - build.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }

        return false;
    }

    public Color GetTeamColor()
    {
        return teamColor;
    }

    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }

    public bool IsPartyOwner()
    {
        return isPartyOwner;
    }

    public string GetDisplayName()
    {
        return playerName;
    }
}
