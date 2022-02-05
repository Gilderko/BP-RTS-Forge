using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;

    [SerializeField] private int maxPlayers = 10;
    [SerializeField] private ushort serverPort;

#if UNITY_SERVER

    private void Start()
    {
        HostServerCallback();
    }

#endif


    public void HostServerCallback()
    {
        Debug.Log("Started server");
        landingPagePanel.SetActive(false);

        RTSNetworkManager.Instance.StartServer(serverPort, maxPlayers);
    }
}
