using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;

    #region Server

    public void Start()
    {
        if (IsServer)
        {
            GameOverHandler.ServerOnGameOver += ServerHandleGameOverClientRpc;
        }
    }

    public void OnDestroy()
    {
        if (IsServer)
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOverClientRpc;
        }
    }
    private void ServerHandleGameOverClientRpc()
    {
        agent.ResetPath();
    }

    private void Update()
    {
        if (IsServer)
        {
            Targetable target = targeter.GetTarget();

            if (target != null)
            {
                if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
                {
                    agent.SetDestination(targeter.GetTarget().transform.position);
                }
                else if (agent.hasPath)
                {
                    agent.ResetPath();
                }
            }
            else
            {
                if (!agent.hasPath || agent.remainingDistance >= agent.stoppingDistance)
                {
                    return;
                }

                agent.ResetPath();
            }
        }
    }

    public void CmdMoveServerRpc(Vector3 position)
    {
        if (!IsOwner)
        {
            return;
        }

        targeter.ClearTarget();

        NavMeshHit hit;
        if (!NavMesh.SamplePosition(position, out hit, 1f, NavMesh.AllAreas))
        {

            return;
        }

        agent.SetDestination(position);
    }

    #endregion
}
