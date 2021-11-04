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

    private RTSPlayer localPlayer;

    public static event System.Action ClientOnConnected;
    public static event System.Action ClientOnDisconnected;

    // Client variables

    // Server variables

    private List<IPlayerSignature> playersConfirmedLoadedGame = new List<IPlayerSignature>();

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
        var factory = AbstractFactory.Get<INetworkTypeFactory>();
        Facade.NetworkMediator = factory.GetNew<INetworkMediator>();        

        Facade.NetworkMediator.PlayerRepository.onPlayerAddedSubscription += HandleNewClientConnected;
        Facade.NetworkMediator.PlayerRepository.onPlayerRemovedSubscription += HandleNewClientDisconnected;

        Facade.NetworkMediator.ChangeEngineProxy(Facade);
        
        Facade.NetworkMediator.StartServer(portNumber, maxPlayers);

        ServerSignature = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IPlayerSignature>();
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
        ServerHandleClientConnected(player);
    }

    private void HandleNewClientDisconnected(INetPlayer player)
    {
        ServerHandleClientDisconnected(player);
    }

    private void InitializeNewGame()
    {
        if (IsServer)
        {
            Debug.Log("Scene changed on server");
            Debug.Log(SceneManager.GetActiveScene().name);
            if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
            {
                Debug.Log("Going to spawn stuff");
                SpawnEntityMessage spawnMessage = spawnPool.Get();
                spawnMessage.Id = ServerGetNewEntityId();
                spawnMessage.OwnerId = ServerSignature;
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

                            occupiedIndexes.Add(index);
                        break;
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

    public IEnumerator ClientLoadLevel(string levelName)
    {
        yield return SceneManager.LoadSceneAsync(levelName);

        var confirmMessage = new ConfirmLevelLoadedMessage();
        confirmMessage.ConfirmedPlayer = LocalPlayer.OwnerSignatureId;

        Facade.NetworkMediator.SendReliableMessage(confirmMessage);
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
        var playerSpawnMessage = new SpawnPlayerObjectMessage();
        playerSpawnMessage.Id = ServerGetNewEntityId();
        playerSpawnMessage.OwnerId = player.Id;
        playerSpawnMessage.PrefabId = playerPrefab.GetComponent<NetworkEntity>().PrefabId;

        playerSpawnMessage.Position = Vector3.zero;
        playerSpawnMessage.Rotation = Quaternion.identity;
        playerSpawnMessage.Scale = Vector3.one;

        playerSpawnMessage.PlayerName = $"Player {Players.Count}";
        playerSpawnMessage.IsTeamOwner = (Players.Count == 1);

        var color = Random.ColorHSV();
        playerSpawnMessage.Red = color.r;
        playerSpawnMessage.Green = color.g;
        playerSpawnMessage.Blue = color.b;

        playerSpawnMessage.MoneyStart = playerMoneyStart;

        SpawnPlayerObjectInterpreter.Instance.Interpret(Facade.NetworkMediator, null, playerSpawnMessage);

        Facade.NetworkMediator.SendReliableMessage(playerSpawnMessage);        

        // Tell the client to spawn all the previous PlayerObjects (with the values)

        Debug.Log(Players.Count);

        foreach (var spawnedPlayer in Players)
        {
            if (spawnedPlayer.OwnerSignatureId == player.Id)
            {
                Debug.Log("Not sending myself again");
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

            var playerColor = spawnedPlayer.GetTeamColor();
            spawnExistingPlayer.Red = playerColor.r;
            spawnExistingPlayer.Green = playerColor.g;
            spawnExistingPlayer.Blue = playerColor.b;

            spawnExistingPlayer.MoneyStart = playerMoneyStart;

            Facade.NetworkMediator.SendReliableMessage(spawnExistingPlayer, player);
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

    public IEnumerator StartGame()
    {
        if (Players.Count < 2) { yield break; }

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
        playersConfirmedLoadedGame.Add(playerSign);

        if (playersConfirmedLoadedGame.Count == Players.Count)
        {
            InitializeNewGame();
        }
    }

    public RTSPlayer GetRTSPlayerById(int UID)
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
