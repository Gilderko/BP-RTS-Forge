using Forge.Networking.Messaging;
using Forge.Networking.Unity;
using Forge.Networking.Unity.Messages;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private NetworkEntity projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f;

    private float lastFireTime;
    private MessagePool<SpawnEntityMessage> spawnPool = new MessagePool<SpawnEntityMessage>();

    #region Server

#if UNITY_SERVER

    private void Update()
    {
        if (IsServer)
        {
            Targetable target = targeter.GetTarget();

            if (target == null)
            {
                return;
            }

            if (!CanFireAtTarget())
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Time.time > (1 / fireRate) + lastFireTime)
            {
                Quaternion projectRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoint.position);

                var projectileSpawnMessage = spawnPool.Get();
                projectileSpawnMessage.Id = RTSNetworkManager.Instance.ServerGetNewEntityId();
                projectileSpawnMessage.OwnerId = OwnerSignatureId;
                projectileSpawnMessage.PrefabId = projectilePrefab.PrefabId;

                projectileSpawnMessage.Position = projectileSpawnPoint.position;
                projectileSpawnMessage.Rotation = projectRotation;
                projectileSpawnMessage.Scale = projectilePrefab.transform.localScale;

                EntitySpawner.SpawnEntityFromMessage(RTSNetworkManager.Instance.Facade, projectileSpawnMessage);

                RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(projectileSpawnMessage);

                lastFireTime = Time.time;
            }
        }
    }

#endif

    private bool CanFireAtTarget()
    {
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= fireRange * fireRange;
    }

    #endregion
}
