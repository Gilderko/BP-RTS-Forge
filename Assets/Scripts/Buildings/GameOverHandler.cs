using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    public static event Action ServerOnGameOver;

    private List<UnitBase> bases = new List<UnitBase>();

    public static event Action<string> ClientOnGameOver;



    #region Server
#if UNITY_SERVER
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
#endif

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

        ulong winnerIndex = bases[0].OwnerClientId;

        RpcGameOverClientRpc($"Player {winnerIndex.ToString()}");

        ServerOnGameOver?.Invoke();
    }

#endregion

#region Client

    private void RpcGameOverClientRpc(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

#endregion
}
