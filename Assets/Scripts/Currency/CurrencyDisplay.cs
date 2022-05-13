using TMPro;
using UnityEngine;

/// <summary>
/// Displays the ammount of resources we have locally in UI.
/// </summary>
public class CurrencyDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resourcesText = null;

    private RTSPlayer player;

#if !UNITY_SERVER

    private void Start()
    {
        if (!RTSNetworkManager.Instance.Facade.IsServer)
        {
            player = RTSNetworkManager.Instance.LocalPlayer;
            ClientHandleResourcesUpdated(player.GetResources());
            player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
        }
    }

#endif

    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int obj)
    {
        resourcesText.text = $"Gold {obj.ToString()}";
    }
}
