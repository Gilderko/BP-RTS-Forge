using Forge.Networking.Players;
using Forge.Networking.Unity;
using System;
using UnityEngine;

// Needs to be implemented with the Entity stuff from Forge
[RequireComponent(typeof(NetworkEntity))]
public class NetworkBehaviour : MonoBehaviour
{
    public NetworkEntity _netEntity = null;

    public bool IsOwner { get => RTSNetworkManager.Instance.gameInstanceOwner.Equals(OwnerSignatureId); }

    public bool IsServer { get => RTSNetworkManager.Instance.Facade.IsServer; }

    public bool IsClient { get => !IsServer; }

    public bool IsLocalPlayer { get => IsClient && IsOwner; }

    public int OwnerClientIntId { get => _netEntity.OwnerId.GetId(); }

    public IPlayerSignature OwnerSignatureId { get => _netEntity.OwnerId; }

    public int EntityId { get => _netEntity.Id; }

    public void SetEntity(NetworkEntity networkEntity)
    {
        _netEntity = networkEntity;
    }
}
