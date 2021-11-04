using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

    #region Server

    public void Start()
    {
        RTSPlayer player = RTSNetworkManager.Instance.GetRTSPlayerById(OwnerClientId);

        if (IsClient)
        {
            player.ClientOnColorUpdated += HandleTeamColorUpdated;
            HandleTeamColorUpdated(player.GetTeamColor());
        }
    }

    private void OnDestroy()
    {
        if (IsClient)
        {
            RTSPlayer player = RTSNetworkManager.Instance.GetRTSPlayerById(OwnerClientId);

            player.ClientOnColorUpdated -= HandleTeamColorUpdated;
        }
    }

    #endregion

    #region Client

    private void HandleTeamColorUpdated(Color newColor)
    {
        foreach (Renderer render in colorRenderers)
        {
            foreach (Material material in render.materials)
            {
                material.SetColor("_BaseColor", newColor);
            }
        }
    }

    #endregion
}
