using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform spawnLocation = null;
    [SerializeField] private TextMeshProUGUI remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private int maxUnitQue = 5;
    [SerializeField] private float spawnMoveRange = 7;
    [SerializeField] private float unitSpawnDuration = 5f;

    private NetworkVariable<int> queuedUnits = new NetworkVariable<int>(
        new NetworkVariableSettings() { WritePermission = NetworkVariablePermission.ServerOnly, ReadPermission = NetworkVariablePermission.Everyone });

    private NetworkVariable<float> unitTimer = new NetworkVariable<float>(
        new NetworkVariableSettings() { WritePermission = NetworkVariablePermission.ServerOnly, ReadPermission = NetworkVariablePermission.Everyone });

    private float progressImageVelocity;

    private void Update()
    {
        if (IsServer)
        {
            ProduceUnits();
        }
        if (IsClient)
        {
            UpdateTimerDisplay();
        }
    }

    public void Start()
    {
        if (IsServer)
        {
            health.ServerOnDie += ServerHandleDie;
        }

        if (IsClient)
        {
            queuedUnits.OnValueChanged += ClientHandleQueuedUnitsUpdated;
        }

    }

    private void OnDestroy()
    {

        if (IsServer)
        {
            health.ServerOnDie -= ServerHandleDie;
        }

        if (IsClient)
        {
            queuedUnits.OnValueChanged -= ClientHandleQueuedUnitsUpdated;
        }

    }

    #region Server

    private void ServerHandleDie()
    {
        Destroy(gameObject);
    }


    private void CmdSpawnUnitServerRpc()
    {
        if (queuedUnits.Value == maxUnitQue)
        {
            return;
        }

        RTSPlayer player = (NetworkManager.Singleton as RTSNetworkManager).GetRTSPlayerByUID(OwnerClientId);

        if (player.GetResources() < unitPrefab.GetResourceCost())
        {
            return;
        }

        queuedUnits.Value++;

        player.ServerAddResources(-unitPrefab.GetResourceCost());
    }

    private void ProduceUnits()
    {
        if (queuedUnits.Value == 0)
        {
            return;
        }

        unitTimer.Value += Time.deltaTime;

        if (unitTimer.Value < unitSpawnDuration)
        {
            return;
        }

        GameObject spawnedUnit = Instantiate(unitPrefab.gameObject, spawnLocation.position, spawnLocation.rotation);

        spawnedUnit.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);

        queuedUnits.Value--;
        unitTimer.Value = 0.0f;
    }

    #endregion

    #region Client

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left && !IsOwner)
        {
            return;
        }

        CmdSpawnUnitServerRpc();
    }


    private void ClientHandleQueuedUnitsUpdated(int oldAmount, int newAmount)
    {
        remainingUnitsText.text = newAmount.ToString();
    }

    private void UpdateTimerDisplay()
    {
        float newProgress = unitTimer.Value / unitSpawnDuration;

        if (newProgress < unitProgressImage.fillAmount)
        {
            unitProgressImage.fillAmount = newProgress;
        }
        else
        {
            unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, newProgress, ref progressImageVelocity, 0.1f);
        }
    }
    #endregion
}
