using TMPro;
using UnityEngine;

public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUIParent = null;
    [SerializeField] private TextMeshProUGUI winnerText = null;


    private void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }


    public void LeaveGame()
    {
        if (RTSNetworkManager.Instance.IsServer)
        {
            RTSNetworkManager.Instance.Facade.ShutDown();
        }
        else if (RTSNetworkManager.Instance.IsClient)
        {
            RTSNetworkManager.Instance.Facade.ShutDown();
        }
    }

    private void ClientHandleGameOver(string playerName)
    {
        winnerText.text = $"{playerName} has won the game!";
        gameOverUIParent.SetActive(true);
    }
}
