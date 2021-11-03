using Forge.Networking.Players;
using Forge.Networking.Unity;
using System;
using UnityEngine;

// Needs to be implemented with the Entity stuff from Forge
[RequireComponent(typeof(NetworkEntity))]
public class NetworkBehaviour : MonoBehaviour
{
    private NetworkEntity _netEntity = null;

    public bool IsOwner { get => _netEntity.OwnerID.GetId() == OwnerClientId; }

    public bool IsServer { get => RTSNetworkManager.Instance.Facade.IsServer; }

    public bool IsClient { get => !IsServer; }

    public bool IsLocalPlayer { get => IsClient && IsOwner; }

    public int OwnerClientId { get => _netEntity.OwnerID.GetId(); }

    public IPlayerSignature OwnerSignatureId { get => _netEntity.OwnerID; }

    public int EntityId { get => _netEntity.Id; }

    public int PrefabId { get => _netEntity.PrefabId; }

    public void SetEntity(NetworkEntity networkEntity)
    {
        _netEntity = networkEntity;
    }
}
