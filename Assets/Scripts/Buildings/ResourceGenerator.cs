using UnityEngine;

/// <summary>
/// Generates resources for the player that spawned it with certain interval and ammount per interval. Resource generation happens on the server.
/// </summary>
public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private int resourcesPerInterval = 10;
    [SerializeField] private float interval = 2f;

    private float timer;
    private RTSPlayer player;

    #region Server

#if UNITY_SERVER

    public void Start()
    {
        if (IsServer)
        {
            timer = interval;
            player = RTSNetworkManager.Instance.GetRTSPlayerById(OwnerClientIntId);

            health.ServerOnDie += ServerHandleDie;
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }
    }

    private void OnDestroy()
    {
        if (IsServer)
        {
            health.ServerOnDie -= ServerHandleDie;
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }
    }

    private void Update()
    {
        if (IsServer)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                player.ServerAddResources(resourcesPerInterval);
                timer += interval;
            }
        }        
    }

#endif

    private void ServerHandleGameOver()
    {
        enabled = false;
    }

    private void ServerHandleDie()
    {
        Destroy(gameObject);
    }

    #endregion
}
