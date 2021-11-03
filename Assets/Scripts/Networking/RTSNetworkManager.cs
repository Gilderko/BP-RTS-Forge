using Forge.Factory;
using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Unity;
using Forge.Networking.Unity.Messages;
using Forge.Networking.Unity.Messages.Interpreters;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : MonoBehaviour
{
    public RTSPlayer LocalPlayer { get => localPlayer; }
    public ForgeEngineFacade Facade { get => forgeEngineFacade; }
    public bool IsServer { get => Facade.IsServer; }
    public bool IsClient { get => !Facade.IsServer; }

    [SerializeField] private RTSPlayer playerPrefab = null;
    [SerializeField] private UnitBase playerBase = null;
    [SerializeField] private GameOverHandler gameOverHandler = null;
    [SerializeField] private ForgeEngineFacade forgeEngineFacade = null;       

    // Singleton pattern

    private static RTSNetworkManager instance = null;
    private static readonly object padlock = new object();

    public static RTSNetworkManager Instance
    {
        get
        {
            lock (padlock)
            {
                return instance;
            }
        }
    }

    // Singleton pattern

    // Client variables

    private RTSPlayer localPlayer;

    public static event System.Action ClientOnConnected;
    public static event System.Action ClientOnDisconnected;

    // Client variables

    // Server variables

    private MessagePool<SpawnEntityMessage> spawnPool = new MessagePool<SpawnEntityMessage>();

    private bool isGameInProgress = false;

    // Server variables

    // Variables Client and Server

    public IPlayerSignature ServerSignature;

    public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();

    // Variable Client and Server

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Here I need to do all the network starting stuff
    /// </summary>
    public void StartServer(ushort portNumber, int maxPlayers)
    {
        Debug.Log("Server start");
        var factory = AbstractFactory.Get<INetworkTypeFactory>();
        Facade.NetworkMediator = factory.GetNew<INetworkMediator>();
        ServerSignature = AbstractFactory.Get<IPlayerSignature>();

        Facade.NetworkMediator.PlayerRepository.onPlayerAddedSubscription += HandleNewClientConnected;
        Facade.NetworkMediator.PlayerRepository.onPlayerRemovedSubscription += HandleNewClientDisconnected;

        Facade.NetworkMediator.ChangeEngineProxy(Facade);
        
        Facade.NetworkMediator.StartServer(portNumber, maxPlayers);
    }

    private void HandleNewClientDisconnected(INetPlayer player)
    {
        ServerHandleClientConnected(player);
    }

    private void HandleNewClientConnected(INetPlayer player)
    {
        ServerHandleClientDisconnected(player);
    }

    private void OnServerSceneChanged()
    {
        if (IsServer)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
            {
                SpawnEntityMessage spawnMessage = spawnPool.Get();
                spawnMessage.Id = ServerGetNewEntityId();
                spawnMessage.OwnerId = ServerSignature;
                spawnMessage.PrefabId = gameOverHandler.PrefabId;

                spawnMessage.Position = Vector3.zero;
                spawnMessage.Rotation = Quaternion.identity;
                spawnMessage.Scale = Vector3.one;

                EntitySpawner.SpawnEntityFromMessage(Facade, spawnMessage);

                Facade.NetworkMediator.SendReliableMessage(spawnMessage);

                Transform parentToSpawnPoints = GameObject.FindGameObjectWithTag("SpawnPoints").transform;

                List<int> occupiedIndexes = new List<int>();

                foreach (RTSPlayer player in Players)
                {
                    int index = 0;

                    while (true)
                    {
                        index = Random.Range(0, parentToSpawnPoints.childCount);
                        if (!occupiedIndexes.Contains(index))

                            occupiedIndexes.Add(index);
                        break;
                    }

                    SpawnEntityMessage baseSpawnMessage = spawnPool.Get();
                    baseSpawnMessage.Id = ServerGetNewEntityId();
                    baseSpawnMessage.OwnerId = player.OwnerSignatureId;
                    baseSpawnMessage.PrefabId = playerBase.PrefabId;

                    baseSpawnMessage.Position = parentToSpawnPoints.GetChild(index).position;
                    baseSpawnMessage.Rotation = Quaternion.identity;
                    baseSpawnMessage.Scale = Vector3.one;

                    EntitySpawner.SpawnEntityFromMessage(Facade, spawnMessage);

                    Facade.NetworkMediator.SendReliableMessage(spawnMessage);

                    player.ServerSetStartingCameraPosition(parentToSpawnPoints.GetChild(index).position);
                }
            }
        }
    }

    private void ServerHandleClientConnected(INetPlayer player)
    {
        if (isGameInProgress)
        {
            // Here I need to find a way to disconnect a person
            
            return;
        }

        // Need to spawn new player object from a message and tell all the other clients to also spawn it
        // set its values as name color and owner
        // the values will be synced with ServerSet used

        var playerSpawnMessage = new SpawnPlayerObjectMessage();
        playerSpawnMessage.Id = ServerGetNewEntityId();
        playerSpawnMessage.OwnerId = player.Id;
        playerSpawnMessage.PrefabId = playerPrefab.PrefabId;
        
        playerSpawnMessage.Position = Vector3.zero;
        playerSpawnMessage.Rotation = Quaternion.identity;
        playerSpawnMessage.Scale = Vector3.one;

        playerSpawnMessage.PlayerName = $"Player {Players.Count}";
        playerSpawnMessage.IsTeamOwner = (Players.Count == 1);

        var color = Random.ColorHSV();
        playerSpawnMessage.Red = color.r;
        playerSpawnMessage.Green = color.g;
        playerSpawnMessage.Blue = color.b;

        SpawnPlayerObjectInterpreter.Instance.Interpret(Facade.NetworkMediator, null, playerSpawnMessage);

        Facade.NetworkMediator.SendReliableMessage(playerSpawnMessage);

        // Tell the client to spawn all the previous PlayerObjects (with the values)

        foreach (var spawnedPlayer in Players)
        {
            if (spawnedPlayer.OwnerSignatureId == player.Id)
            {
                continue;
            }

            var spawnExistingPlayer = new SpawnPlayerObjectMessage();
            spawnExistingPlayer.Id = spawnedPlayer.EntityId;
            spawnExistingPlayer.OwnerId = spawnedPlayer.OwnerSignatureId;
            spawnExistingPlayer.PrefabId = playerPrefab.PrefabId;

            spawnExistingPlayer.Position = Vector3.zero;
            spawnExistingPlayer.Rotation = Quaternion.identity;
            spawnExistingPlayer.Scale = Vector3.one;

            spawnExistingPlayer.PlayerName = spawnedPlayer.GetDisplayName();
            spawnExistingPlayer.IsTeamOwner = spawnedPlayer.IsPartyOwner();

            var playerColor = spawnedPlayer.GetTeamColor();
            spawnExistingPlayer.Red = playerColor.r;
            spawnExistingPlayer.Green = playerColor.g;
            spawnExistingPlayer.Blue = playerColor.b;
        }
    }

    public void ClientHandleClientConnected(int obj)
    {
        ClientOnConnected?.Invoke();
    }

    private void ServerHandleClientDisconnected(INetPlayer play)
    {
        Players.RemoveAll(player => player.OwnerSignatureId == play.Id);
    }

    public void ClientHandleClientDisconnected(int obj)
    {
        Players.Clear();
        ClientOnDisconnected?.Invoke();
    }

    public void OnDestroy()
    {
        if (IsServer)
        {
            Players.Clear();

            isGameInProgress = false;
        }
    }

    #region Server  

    public void StartGame()
    {
        if (Players.Count < 2) { return; }

        isGameInProgress = true;

        LoadSceneFromServerMessage loadSceneMessage = new LoadSceneFromServerMessage();
        loadSceneMessage.SceneName = "Scene_Map";

        SceneManager.LoadScene(loadSceneMessage.SceneName);
        Facade.NetworkMediator.SendReliableMessage(loadSceneMessage);

        OnServerSceneChanged();
    }

    public int ServerGetNewEntityId()
    {
        return Facade.GetNewEntityId();
    }

    #endregion

    public RTSPlayer GetRTSPlayerByUID(int UID)
    {
        return Players.Find(player => player.OwnerClientId == UID);
    }

    public int GetPlayerCount()
    {
        return Players.Count;
    }

    public void ClientSetLocalPlayer(RTSPlayer myPlayer)
    {
        localPlayer = myPlayer; 
    }
}
