using Forge.Networking.Unity.Messages;
using System;
using System.Collections.Generic;

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
