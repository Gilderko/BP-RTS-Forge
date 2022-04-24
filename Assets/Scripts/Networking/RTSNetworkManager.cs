using Forge.Factory;
using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Unity;
using Forge.Networking.Unity.Messages;
using Forge.Networking.Unity.Messages.Interpreters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : MonoBehaviour
{
    public RTSPlayer LocalPlayer { get => localPlayer; }
    public ForgeEngineFacade Facade { get => forgeEngineFacade; }
    public bool IsServer { get => Facade.IsServer; }
    public bool IsClient { get => !Facade.IsServer; }

    [Header("Player")]
    [SerializeField] private RTSPlayer playerPrefab = null;
    [SerializeField] private int playerMoneyStart = 500;

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

    public static event System.Action ClientOnConnected;
    public static event System.Action ClientOnDisconnected;

    private RTSPlayer localPlayer;

    private readonly object isLoadingLock = new object();
    private bool hadLoaded = false;

    // Client variables

    // Server variables

    private List<int> playersConfirmedLoadedGame = new List<int>();

    private MessagePool<SpawnEntityMessage> spawnPool = new MessagePool<SpawnEntityMessage>();

    private bool isGameInProgress = false;

    // Server variables

    // Variables Client and Server

    /// <summary>
    /// On server it is server on client it is local client
    /// </summary>
    public IPlayerSignature gameInstanceOwner { get => Facade.NetworkMediator.SocketFacade.NetPlayerId; }

    public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();

    // Variable Client and Server

    private void Awake()
    {        
        instance = this;
        Application.quitting += () => Facade.ShutDown();
    }

    /// <summary>
    /// Here I need to do all the network starting stuff
    /// </summary>
    public void StartServer(ushort portNumber, int maxPlayers)
    {
        var factory = AbstractFactory.Get<INetworkTypeFactory>();
        Facade.NetworkMediator = factory.GetNew<INetworkMediator>();        

        Facade.NetworkMediator.PlayerRepository.onPlayerAddedSubscription += HandleNewClientConnected;
        Facade.NetworkMediator.PlayerRepository.onPlayerRemovedSubscription += HandleNewClientDisconnected;

        Facade.NetworkMediator.ChangeEngineProxy(Facade);
        
        Facade.NetworkMediator.StartServer(portNumber, maxPlayers);
    }

    public void StartClient(string address, ushort portNumber)
    {
        var factory = AbstractFactory.Get<INetworkTypeFactory>();
        Facade.NetworkMediator = factory.GetNew<INetworkMediator>();

        Facade.NetworkMediator.ChangeEngineProxy(Facade);

        try
        {
            Facade.NetworkMediator.StartClient(address, portNumber);
        }
        catch (System.Exception ex)
        {
            Facade.Logger.LogException(ex);
        }
    }

    private void HandleNewClientConnected(INetPlayer player)
    {
        Debug.Log($"New client connected with ID: {player.Id}");
    }

    private void HandleNewClientDisconnected(INetPlayer player)
    {
        ServerHandleClientDisconnected(player);
    }

    private void InitializeNewGame()
    {
        if (IsServer)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
            {
                SpawnEntityMessage spawnMessage = spawnPool.Get();
                spawnMessage.Id = ServerGetNewEntityId();
                spawnMessage.OwnerId = gameInstanceOwner;
                spawnMessage.PrefabId = gameOverHandler.GetComponent<NetworkEntity>().PrefabId;

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
                        {
                            occupiedIndexes.Add(index);
                            break;
                        }
                    }

                    SpawnEntityMessage baseSpawnMessage = spawnPool.Get();
                    baseSpawnMessage.Id = ServerGetNewEntityId();
                    baseSpawnMessage.OwnerId = player.OwnerSignatureId;
                    baseSpawnMessage.PrefabId = playerBase.GetComponent<NetworkEntity>().PrefabId;

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

    public void ServerHandleClientConnected(IPlayerSignature player)
    {        
        if (isGameInProgress)
        {
            // Here I need to find a way to disconnect a person
            Facade.NetworkMediator.PlayerRepository.RemovePlayer(player);
            return;
        }

        // Need to spawn new player object from a message and tell all the other clients to also spawn it
        // set its values as name color and owner
        var playerSpawnMessage = new SpawnPlayerObjectMessage();
        playerSpawnMessage.Id = ServerGetNewEntityId();
        playerSpawnMessage.OwnerId = player;
        playerSpawnMessage.PrefabId = playerPrefab.GetComponent<NetworkEntity>().PrefabId;

        playerSpawnMessage.Position = Vector3.zero;
        playerSpawnMessage.Rotation = Quaternion.identity;
        playerSpawnMessage.Scale = Vector3.one;

        playerSpawnMessage.PlayerName = $"Player {Players.Count}";
        playerSpawnMessage.IsTeamOwner = (Players.Count == 0);
        playerSpawnMessage.MoneyStart = playerMoneyStart;

        var color = Random.ColorHSV();
        playerSpawnMessage.Red = color.r;
        playerSpawnMessage.Green = color.g;
        playerSpawnMessage.Blue = color.b;        

        SpawnPlayerObjectInterpreter.Instance.Interpret(Facade.NetworkMediator, null, playerSpawnMessage);

        Facade.NetworkMediator.SendReliableMessage(playerSpawnMessage);        

        // Tell the client to spawn all the previous PlayerObjects (with the values)

        foreach (var spawnedPlayer in Players)
        {
            if (spawnedPlayer.OwnerSignatureId.Equals(player))
            {
                continue;
            }

            var spawnExistingPlayer = new SpawnPlayerObjectMessage();
            spawnExistingPlayer.Id = spawnedPlayer.EntityId;
            spawnExistingPlayer.OwnerId = spawnedPlayer.OwnerSignatureId;
            spawnExistingPlayer.PrefabId = playerPrefab.GetComponent<NetworkEntity>().PrefabId;

            spawnExistingPlayer.Position = Vector3.zero;
            spawnExistingPlayer.Rotation = Quaternion.identity;
            spawnExistingPlayer.Scale = Vector3.one;

            spawnExistingPlayer.PlayerName = spawnedPlayer.GetDisplayName();
            spawnExistingPlayer.IsTeamOwner = spawnedPlayer.IsPartyOwner();
            spawnExistingPlayer.MoneyStart = playerMoneyStart;

            var playerColor = spawnedPlayer.GetTeamColor();
            spawnExistingPlayer.Red = playerColor.r;
            spawnExistingPlayer.Green = playerColor.g;
            spawnExistingPlayer.Blue = playerColor.b;

            Facade.NetworkMediator.SendReliableMessage(spawnExistingPlayer, Facade.NetworkMediator.PlayerRepository.GetPlayer(player));
        }
    }

    public IEnumerator ClientLoadLevel(string levelName)
    {
        lock (isLoadingLock)
        {
            if (levelName == SceneManager.GetActiveScene().name || hadLoaded)
            {
                yield break;
            }

            yield return SceneManager.LoadSceneAsync(levelName);
            hadLoaded = true;

            var confirmMessage = new ConfirmLevelLoadedMessage();
            confirmMessage.ConfirmedPlayer = LocalPlayer.OwnerSignatureId;

            Facade.NetworkMediator.SendReliableMessage(confirmMessage);
        }    
    }

    public void ClientHandleClientConnected(int obj)
    {
        if (IsServer)
        {
            return;
        }

        ClientOnConnected?.Invoke();
    }

    private void ServerHandleClientDisconnected(INetPlayer play)
    {
        Players.RemoveAll(player => player.OwnerSignatureId.Equals(play.Id));
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

    public IEnumerator StartGame()
    {
        if (Players.Count < 2 || isGameInProgress ) { yield break; }

        isGameInProgress = true;

        LoadSceneFromServerMessage loadSceneMessage = new LoadSceneFromServerMessage();
        loadSceneMessage.SceneName = "Scene_Map";

        yield return SceneManager.LoadSceneAsync(loadSceneMessage.SceneName);

        Facade.NetworkMediator.SendReliableMessage(loadSceneMessage);
    }

    public int ServerGetNewEntityId()
    {
        return Facade.GetNewEntityId();
    }

    public void AddPlayerReady(IPlayerSignature playerSign)
    {
        if (playersConfirmedLoadedGame.Contains(playerSign.GetId()))
        {
            return;
        }

        playersConfirmedLoadedGame.Add(playerSign.GetId());

        if (playersConfirmedLoadedGame.Count == Players.Count)
        {
            InitializeNewGame();
        }
    }

    public RTSPlayer GetRTSPlayerById(int UID)
    {
        return Players.Find(player => player.OwnerClientIntId == UID);
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
