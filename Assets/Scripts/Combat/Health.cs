using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages;
using System;
using UnityEngine;

/// <summary>
/// Basic health component where all damage dealing is handled on server.
/// </summary>
public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    public event Action ServerOnDie;

    public event Action<int, int> ClientOnHealthUpdated;

    // Pool of messages to make the updates faster
    private MessagePool<UpdateHealthMessage> healthPool = new MessagePool<UpdateHealthMessage>();

    public void Start()
    {
        if (IsServer)
        {
            currentHealth = maxHealth;
            ServerUpdateHealth();
            UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
        }
    }

    private void OnDestroy()
    {
        if (IsServer)
        {
            UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
        }
    }

    #region Server

    private void ServerHandlePlayerDie(int connectionID)
    {
        if (connectionID != OwnerClientIntId)
        {
            return;
        }

        DealDamage(currentHealth);
    }

    public void DealDamage(int damageAmount)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth - damageAmount, 0, maxHealth);
        ServerUpdateHealth();

        if (currentHealth != 0)
        {
            return;
        }

        ServerOnDie?.Invoke();
    }

    private void ServerUpdateHealth()
    {
        var newMessage = healthPool.Get();
        newMessage.EntityID = EntityId;
        newMessage.NewHealthValue = currentHealth;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(newMessage);
    }

    #endregion

    #region Client

    public void ClientSetHealth(int newHealthValue)
    {
        currentHealth = newHealthValue;
        ClientOnHealthUpdated(newHealthValue, maxHealth);
    }

    #endregion
}
