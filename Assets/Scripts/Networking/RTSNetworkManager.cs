using Forge.Networking.Unity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkBehaviour
{
    public ForgeEngineFacade Facade { get => forgeEngineFacade; }

    [SerializeField] private GameObject playerBase = null;
    [SerializeField] private GameOverHandler gameOverHandler = null;
    [SerializeField] private ForgeEngineFacade forgeEngineFacade = null;

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

    public static event System.Action ClientOnConnected;
    public static event System.Action ClientOnDisconnected;

    public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();

    private bool isGameInProgress = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        OnClientConnectedCallback += HandleClientConnected;
        OnClientDisconnectCallback += HandleClientDisconnected;
        NetworkSceneManager.OnSceneSwitched += OnServerSceneChanged;
    }

    private void OnServerSceneChanged()
    {
        if (IsServer)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
            {
                GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandler);
                gameOverHandlerInstance.GetComponent<NetworkObject>().Spawn();

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

                    GameObject baseInstance = Instantiate(playerBase, parentToSpawnPoints.GetChild(index).position, Quaternion.identity);

                    Debug.Log(player.OwnerClientId);

                    baseInstance.GetComponent<NetworkObject>().SpawnWithOwnership(player.OwnerClientId);

                    player.ChangeStartingPosition(baseInstance.transform.position);
                }
            }
        }
    }

    private void HandleClientDisconnected(int obj)
    {
        if (IsClient)
        {
            Players.Clear();
            ClientOnDisconnected?.Invoke();
        }
        else if (IsServer)
        {
            Players.RemoveAll(player => player.OwnerClientId == obj);
        }
    }

    private void HandleClientConnected(int obj)
    {
        if (IsClient)
        {
            ClientOnConnected?.Invoke();
        }
        else if (IsServer)
        {
            if (isGameInProgress)
            {
                DisconnectClient(obj);
                return;
            }

            RTSPlayer player = ConnectedClients[obj].PlayerObject.GetComponent<RTSPlayer>();

            player.SetPlayerName($"Player {Players.Count}");

            player.SetTeamColor(Random.ColorHSV());

            player.SetPartyOwner(Players.Count == 1);
        }
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

        NetworkSceneManager.SwitchScene("Scene_Map");
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
}
