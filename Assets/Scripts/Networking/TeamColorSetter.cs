using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

    /*private NetworkVariable<Color> teamColor = new NetworkVariable<Color>(
        new NetworkVariableSettings() {WritePermission = NetworkVariablePermission.ServerOnly, ReadPermission = NetworkVariablePermission.Everyone });*/

    #region Server

    public void Start()
    {
        if (IsClient)
        {
            teamColor.OnValueChanged += HandleTeamColorUpdated;
        }
        else if (IsServer)
        {
            RTSPlayer player = (NetworkManager.Singleton as RTSNetworkManager).GetRTSPlayerByUID(OwnerClientId);

            teamColor.Value = player.GetTeamColor();
        }
    }

    #endregion

    #region Client

    private void HandleTeamColorUpdated(Color oldColor, Color newColor)
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
