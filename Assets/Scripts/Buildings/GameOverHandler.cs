using Forge.Networking.Unity.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles what happens when the last player base is despawned. Can include logic specific for both the server and the client.
/// </summary>
public class GameOverHandler : NetworkBehaviour
{
    public static event Action ServerOnGameOver;

    private List<UnitBase> bases = new List<UnitBase>();

    public static event Action<string> ClientOnGameOver;

    private bool hasClientAnnounced = false;

    #region Server

    private void Start()
    {
        if (IsServer)
        {
            UnitBase.ServerOnBaseSpawnend += ServerHandleBaseSpawned;
            UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
        }
    }

    private void OnDestroy()
    {
        if (IsServer)
        {
            UnitBase.ServerOnBaseSpawnend -= ServerHandleBaseSpawned;
            UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
        }
    }

    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
    }

    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);

        if (bases.Count != 1)
        {
            return;
        }

        int winnerIndex = bases[0].OwnerClientIntId;
        ServerSendGameOverMessage(winnerIndex);

        ServerOnGameOver?.Invoke();

        StartCoroutine(DelayedQuit(5));
    }

    private IEnumerator DelayedQuit(float timer)
    {
        yield return new WaitForSeconds(timer);

        Application.Quit();
    }

    private void ServerSendGameOverMessage(int winnerIndex)
    {
        AnnounceWinnerMessage announceMessage = new AnnounceWinnerMessage();
        announceMessage.ObjectId = EntityId;
        announceMessage.WinningPlayerName = winnerIndex.ToString();

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendMessage(announceMessage);
    }

    #endregion

    #region Client

    public void ClientHandleGameOver(string winner)
    {
        if (hasClientAnnounced)
        {
            return;
        }

        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
