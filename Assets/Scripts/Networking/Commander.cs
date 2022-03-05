using Forge.Networking.Messaging;
using Forge.Networking.Unity;
using Forge.Networking.Unity.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Commander : NetworkBehaviour
{
    public static Commander Instance;

    [SerializeField] private Building oilPump;
    [SerializeField] private Building tankSpawner;

    private Controls inputActions;
    private RTSPlayer localPlayer;

    private UnitSelectionHandler selectionHandler;
    private UnitCommandGiver commandGiver;

    private List<GameObject> waypoints;

    private MessagePool<CommandBuildServerMessage> buildServerPool = new MessagePool<CommandBuildServerMessage>();
    private MessagePool<CommandBuildClientsMessage> buildClientPool = new MessagePool<CommandBuildClientsMessage>();
    private MessagePool<ForceBuildServerMessage> forceBuildClientPool = new MessagePool<ForceBuildServerMessage>();

    private MessagePool<CommandSpawnServerMessage> spawnServerPool = new MessagePool<CommandSpawnServerMessage>();
    private MessagePool<CommandSpawnClientMessage> spawnClientPool = new MessagePool<CommandSpawnClientMessage>();
    private MessagePool<CommandSpawnUnitsMessage> spawnPool = new MessagePool<CommandSpawnUnitsMessage>();

    private MessagePool<CommandMoveServerMessage> moveServerPool = new MessagePool<CommandMoveServerMessage>();
    private MessagePool<CommandMoveClientMessage> moveClientPool = new MessagePool<CommandMoveClientMessage>();

    // RequestIds
    private int buildingReqId = 0;
    private int spawnUnitReqId = 0;
    private int moveReqId = 0;

    private void Start()
    {
        selectionHandler = FindObjectOfType<UnitSelectionHandler>();
        commandGiver = FindObjectOfType<UnitCommandGiver>();
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint").ToList();

        inputActions = new Controls();

        inputActions.Player.BuildMine.performed += SendCommandBuildMine;
        inputActions.Player.BuildSpawner.performed += SendCommandBuildSpawner;
        inputActions.Player.SpawnUnit.performed += SendCommandSpawnUnit;
        inputActions.Player.SendUnits.performed += SendCommandSendUnits;
        
        if (IsClient)
        {
            var players = RTSNetworkManager.Instance.Players;
            localPlayer = players.First(player => player.IsLocalPlayer);
        }

        inputActions.Enable();
        Instance = this;
    }

    // Main Client
    private void SendCommandBuildSpawner(InputAction.CallbackContext obj)
    {
        var msg = buildServerPool.Get();
        msg.RequestId = buildingReqId;
        msg.BuildingId = tankSpawner.GetComponent<NetworkEntity>().PrefabId;

        buildingReqId++;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(msg);
    }

    // Main Client
    private void SendCommandBuildMine(InputAction.CallbackContext obj)
    {
        var msg = buildServerPool.Get();
        msg.RequestId = buildingReqId;
        msg.BuildingId = oilPump.GetComponent<NetworkEntity>().PrefabId;

        buildingReqId++;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(msg);
    }

    // Server
    public void CmdTellClientsToBuild(int buildingId, int requestId)
    {
        var msg = buildClientPool.Get();
        msg.RequestId = requestId;
        msg.BuildingId = buildingId;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(msg);
    }

    // RPC on clients
    public void RpcClientsBuild(int buildingId, int requestId)
    {
        StartCoroutine(Building(buildingId, requestId));
    }

    // RPC on clients
    public IEnumerator Building(int buildingId, int requestId)
    {
        Building spawnBase = localPlayer.GetMyBuildings().First();

        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            Vector3 samplePos = spawnBase.transform.position + UnityEngine.Random.insideUnitSphere * 25f;
            if (!NavMesh.SamplePosition(samplePos, out NavMeshHit hit, 25f, NavMesh.AllAreas))
            {
                continue;
            }

            if (!localPlayer.CanPlaceBuilding(oilPump.GetComponent<BoxCollider>(), hit.position))
            {
                continue;
            }

            var spawnBuildMessage = forceBuildClientPool.Get();
            spawnBuildMessage.OwnerId = localPlayer.OwnerSignatureId;
            spawnBuildMessage.PrefabId = buildingId;
            spawnBuildMessage.RequestId = requestId;

            spawnBuildMessage.PosX = hit.position.x;
            spawnBuildMessage.PosY = hit.position.y;
            spawnBuildMessage.PosZ = hit.position.z;

            RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(spawnBuildMessage);
            break;
        }
    }

    // Main Client
    private void SendCommandSpawnUnit(InputAction.CallbackContext obj)
    {
        var msg = spawnServerPool.Get();
        msg.RequestId = spawnUnitReqId;

        spawnUnitReqId++;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(msg);
    }

    // Server
    public void CmdTellClientsToSpawnUnits(int requestId)
    {
        var msg = spawnClientPool.Get();
        msg.RequestId = requestId;

        spawnUnitReqId++;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(msg);
    }

    // RPC on clients
    public void RpcClientsSpawnUnit()
    {
        var spawners = localPlayer.GetMyBuildings().Where(build => build.GetComponent<UnitSpawner>() != null);

        var spawnIndex = UnityEngine.Random.Range(0, spawners.Count());

        var commandSpawnMessage = spawnPool.Get();
        commandSpawnMessage.EntityId = spawners.ElementAt(spawnIndex).EntityId;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(commandSpawnMessage);
    }

    // Main Client
    private void SendCommandSendUnits(InputAction.CallbackContext obj)
    {
        var msg = moveServerPool.Get();
        msg.RequestId = moveReqId;

        moveReqId++;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(msg);
    }

    // Server
    public void CmdTellClientsToMoveUnits(int requestId)
    {
        var msg = moveClientPool.Get();
        msg.RequestId = requestId;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(msg);
    }

    // RPC on clients
    public void RpcClientsSendUnits()
    {
        var playerUnits = localPlayer.GetMyUnits();
        
        if (playerUnits.Count() == 0)
        {
            return;
        }

        var averagePoint = playerUnits.First().transform.position;

        foreach (var unit in playerUnits)
        {
            selectionHandler.AddUnitToSelection(unit);
            averagePoint += unit.transform.position;
            averagePoint /= 2;
        }

        var furthest = new Tuple<GameObject,float>(waypoints[0],-1f);
        
        foreach (var waypoint in waypoints)
        {
            var currDist = Vector3.Distance(averagePoint, waypoint.transform.position);

            if (currDist > furthest.Item2)
            {
                furthest = new Tuple<GameObject, float>(waypoint, currDist);
            }
        }

        if (!NavMesh.SamplePosition(furthest.Item1.transform.position, out NavMeshHit hit, 25f, NavMesh.AllAreas))
        {
            return;
        }      

        commandGiver.TryMove(hit.position);
    }
}
