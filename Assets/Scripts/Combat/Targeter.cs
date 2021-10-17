using UnityEngine;

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


    public void CmdSetTargetServerRpc(int playerID, int instanceID)
    {
        var targetGameObject = RTSNetworkManager.Instance..ConnectedClients[playerID].OwnedObjects.Find(obj => obj.NetworkObjectId == instanceID);

        Targetable newTarget;

        if (!targetGameObject.TryGetComponent<Targetable>(out newTarget))
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
