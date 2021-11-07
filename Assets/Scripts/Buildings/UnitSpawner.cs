using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages;
using Forge.Networking.Unity;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform spawnLocation = null;
    [SerializeField] private TextMeshProUGUI remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private int maxUnitQue = 5;
    [SerializeField] private float unitSpawnDuration = 5f;

    // Internal references
    // Server to Client
    private MessagePool<SpawnEntityMessage> spawnPool = new MessagePool<SpawnEntityMessage>();
    private MessagePool<UpdateUnitSpawnerQueMessage> spawnerUpdateClientPool = new MessagePool<UpdateUnitSpawnerQueMessage>();

    // Client to Server
    private MessagePool<CommandSpawnUnitsMessage> commandSpawnPool = new MessagePool<CommandSpawnUnitsMessage>();

    private int queuedUnits = 0;

    private float unitTimer = 0f;

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
    }

    private void OnDestroy()
    {
        if (IsServer)
        {
            health.ServerOnDie -= ServerHandleDie;
        }
    }

    #region Server

    private void ServerHandleDie()
    {
        Destroy(gameObject);
    }


    public void CmdSpawnUnitServerRpc()
    {
        if (queuedUnits == maxUnitQue)
        {
            return;
        }

        RTSPlayer player = RTSNetworkManager.Instance.GetRTSPlayerById(OwnerClientIntId);

        if (player.GetResources() < unitPrefab.GetResourceCost())
        {
            return;
        }

        queuedUnits++;

        // Send message to tell client new units have been added to que
        var queUpdateClient = spawnerUpdateClientPool.Get();
        queUpdateClient.EntityId = EntityId;
        queUpdateClient.NewQueAmmount = queuedUnits;
        queUpdateClient.IsIncrease = true;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(queUpdateClient);

        player.ServerAddResources(-unitPrefab.GetResourceCost());
    }

    private void ProduceUnits()
    {
        if (queuedUnits == 0)
        {
            return;
        }

        unitTimer += Time.deltaTime;

        if (unitTimer < unitSpawnDuration)
        {
            return;
        }

        queuedUnits--;

        // Send spawning message
        var spawnMessage = spawnPool.Get();
        spawnMessage.Id = RTSNetworkManager.Instance.ServerGetNewEntityId();
        spawnMessage.OwnerId = OwnerSignatureId;
        spawnMessage.PrefabId = unitPrefab.GetComponent<NetworkEntity>().PrefabId;

        spawnMessage.Position = spawnLocation.position;
        spawnMessage.Rotation = Quaternion.identity;
        spawnMessage.Scale = Vector3.one;

        EntitySpawner.SpawnEntityFromMessage(RTSNetworkManager.Instance.Facade, spawnMessage);

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(spawnMessage);        

        // Send message to update number of units in que
        var spawnerUpdate = spawnerUpdateClientPool.Get();
        spawnerUpdate.EntityId = EntityId;
        spawnerUpdate.NewQueAmmount = queuedUnits;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(spawnerUpdate);

        unitTimer = 0.0f;
    }

    #endregion

    #region Client

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || !IsOwner)
        {
            return;
        }

        var commandSpawnMessage = commandSpawnPool.Get();
        commandSpawnMessage.EntityId = EntityId;

        RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(commandSpawnMessage);
    }

    private void ClientHandleQueuedUnitsUpdated(int newAmount)
    {
        remainingUnitsText.text = newAmount.ToString();
    }

    public void ClientUpdateQueAmmount(int newAmount, bool isIncrease)
    {
        queuedUnits = newAmount;
        unitTimer = isIncrease ? 0 : unitTimer;
        ClientHandleQueuedUnitsUpdated(queuedUnits);
    }

    private void UpdateTimerDisplay()
    {
        if (queuedUnits == 0)
        {
            return;
        }

        unitTimer += Time.deltaTime;

        float newProgress = unitTimer / unitSpawnDuration;

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
