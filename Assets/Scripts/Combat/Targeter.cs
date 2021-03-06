using UnityEngine;

/// <summary>
/// Component that handles current enemy target that the GameObject should attack.
/// </summary>
public class Targeter : NetworkBehaviour
{
    [SerializeField] private Targetable target;

    #region Server

    public void Start()
    {
        if (IsServer)
        {
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }
    }

    private void OnDestroy()
    {
        if (IsServer)
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }
    }

    private void ServerHandleGameOver()
    {
        ClearTarget();
    }

    public void CmdSetTargetServerRpc(int instanceID)
    {
        var targetGameObject = RTSNetworkManager.Instance.Facade.EntityRepository.Get(instanceID);

        if (targetGameObject.OwnerId.Equals(OwnerSignatureId))
        {
            return;
        }

        Targetable newTarget;

        if (!targetGameObject.OwnerGameObject.TryGetComponent<Targetable>(out newTarget))
        {
            return;
        }

        target = newTarget;
    }

    public void ClearTarget()
    {
        target = null;
    }

    #endregion

    public Targetable GetTarget()
    {
        return target;
    }
}
