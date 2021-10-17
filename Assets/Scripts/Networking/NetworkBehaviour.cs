using Forge.Networking.Unity;
using System;
using UnityEngine;

// Needs to be implemented with the Entity stuff from Forge
public class NetworkBehaviour : MonoBehaviour
{
    private NetworkEntity _netEntity = null;

    public bool IsOwner { get; set; }

    public bool IsServer { get => RTSNetworkManager.Instance.Facade.IsServer; }

    public bool IsClient { get => !IsServer; }

    public bool IsLocalPlayer { get => IsClient && IsOwner; }

    public int OwnerClientId { get => GetComponent<NetworkEntity>().OwnerID.GetId(); }

    public int ObjectId { get => _netEntity.Id; }

    public void SetEntity(NetworkEntity networkEntity)
    {
        _netEntity = networkEntity;
    }
}
