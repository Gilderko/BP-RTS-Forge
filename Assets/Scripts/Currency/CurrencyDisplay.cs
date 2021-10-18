using TMPro;
using UnityEngine;

public class CurrencyDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resourcesText = null;

    private RTSPlayer player;

    private void Start()
    {
        if (!RTSNetworkManager.Instance.Facade.IsServer)
        {
            player = RTSNetworkManager.Instance.LocalPlayer;
            ClientHandleResourcesUpdated(player.GetResources());
            player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
        }
    }

    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }


    private void ClientHandleResourcesUpdated(int obj)
    {
        resourcesText.text = $"Gold {obj.ToString()}";
    }
}
