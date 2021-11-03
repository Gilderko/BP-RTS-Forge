using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUi;
    [SerializeField] private Button startGameButton;

    [SerializeField] private RectTransform playerParent;
    [SerializeField] private PlayerLobbyUIInstance playerLobbyUI;


    private void Start()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerChanged += AuthorityHandlePartyOwnerStateUpdate;
        RTSPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    private void OnDestroy()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSPlayer.AuthorityOnPartyOwnerChanged -= AuthorityHandlePartyOwnerStateUpdate;
        RTSPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }


    private void AuthorityHandlePartyOwnerStateUpdate(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    private void ClientHandleInfoUpdated()
    {
        foreach (Transform child in playerParent.transform)
        {
            Destroy(child.gameObject);
        }

        List<RTSPlayer> players = RTSNetworkManager.Instance.Players;

        for (int i = 0; i < players.Count; i++)
        {
            var playerUIInstance = Instantiate(playerLobbyUI, playerParent);
            playerUIInstance.SetName(players[i].GetDisplayName());
        }
    }

    private void HandleClientConnected()
    {
        lobbyUi.SetActive(true);
    }

    public void LeaveLobby()
    {
        if (RTSNetworkManager.Instance.IsServer)
        {
            RTSNetworkManager.Instance.Facade.ShutDown();
        }
        else
        {
            RTSNetworkManager.Instance.Facade.ShutDown();

            SceneManager.LoadScene(0);
        }
    }

    public void StartGame()
    {
        Debug.Log("Client wants to start");
        Debug.Log(RTSNetworkManager.Instance.LocalPlayer.IsPartyOwner());
        RTSNetworkManager.Instance.LocalPlayer.CmdStartGameServerRpc();
    }
}
