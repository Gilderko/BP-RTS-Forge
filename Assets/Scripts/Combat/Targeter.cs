using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    [SerializeField] private Targetable target;

    #region Server

#if UNITY_SERVER
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
#endif

    private void ServerHandleGameOver()
    {
        ClearTarget();
    }


    public void CmdSetTargetServerRpc(ulong playerID, ulong instanceID)
    {
        /*var targetGameObject = NetworkManager.ConnectedClients[playerID].OwnedObjects.Find(obj => obj.NetworkObjectId == instanceID);

        Targetable newTarget;

        if (!targetGameObject.TryGetComponent<Targetable>(out newTarget))
        {
            return;
        }

        target = newTarget;*/
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
