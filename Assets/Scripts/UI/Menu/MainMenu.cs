using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;





    public void HostServerCallback()
    {
        Debug.Log("Started server");
        landingPagePanel.SetActive(false);

        //NetworkManager.Singleton.StartServer();
    }
}
