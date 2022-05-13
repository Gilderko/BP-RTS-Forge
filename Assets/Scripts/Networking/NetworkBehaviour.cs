using Forge.Networking.Players;
using Forge.Networking.Unity;
using UnityEngine;


/// <summary>
/// Custom implementation of NetworkBehaviours to resemble the implementation of the NetworkBehaviour of both Mirror or Netcode.
/// </summary>
[RequireComponent(typeof(NetworkEntity))]
public class NetworkBehaviour : MonoBehaviour
{
    // The entity is set in runtime by the EntitySpawner
    private NetworkEntity _netEntity = null;

    public bool IsOwner { get => RTSNetworkManager.Instance.Facade.NetworkMediator.SocketFacade.NetPlayerId.Equals(OwnerSignatureId); }

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
