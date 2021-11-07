using System;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;

    public static event Action<UnitBase> ServerOnBaseSpawnend;
    public static event Action<UnitBase> ServerOnBaseDespawned;

    public static event Action<int> ServerOnPlayerDie;

    #region Server

    public void Start()
    {
        if (IsServer)
        {
            health.ServerOnDie += ServerHandleDeath;
            ServerOnBaseSpawnend?.Invoke(this);
        }
    }

    private void OnDestroy()
    {
        if (IsServer)
        {
            ServerOnBaseDespawned?.Invoke(this);
            health.ServerOnDie -= ServerHandleDeath;
        }
    }


    private void ServerHandleDeath()
    {
        ServerOnPlayerDie?.Invoke(OwnerClientIntId);

        Destroy(gameObject);
    }
    #endregion
}
