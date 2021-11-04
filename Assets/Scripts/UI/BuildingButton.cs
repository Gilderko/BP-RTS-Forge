using Forge.Networking.Messaging;
using Forge.Networking.Unity;
using Forge.Networking.Unity.Messages;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Building representedBuilding = null;
    [SerializeField] private Image iconMage = null;
    [SerializeField] private TextMeshProUGUI priceText = null;
    [SerializeField] private LayerMask floorMask = new LayerMask();

    [SerializeField] private Color canPlaceColor = new Color();
    [SerializeField] private Color canNotPlaceColor = new Color();

    private Camera mainCamera;
    private BoxCollider buildingCollider;
    private RTSPlayer player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;

    private MessagePool<ServerAskSpawnBuildingMessage> spawnBuildAsk = new MessagePool<ServerAskSpawnBuildingMessage>();

    private void Start()
    {
        mainCamera = Camera.main;
        iconMage.sprite = representedBuilding.GetIcon();
        priceText.text = representedBuilding.GetPrice().ToString();
        buildingCollider = representedBuilding.GetComponent<BoxCollider>();

        if (!RTSNetworkManager.Instance.IsServer)
        {
            player = RTSNetworkManager.Instance.LocalPlayer;
        }
    }

    private void Update()
    {
        if (buildingPreviewInstance == null)
        {
            return;
        }

        UpdateBuildingPreview();
    }


    private void UpdateBuildingPreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit, Mathf.Infinity, floorMask);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (hasHit)
            {
                var spawnBuildMessage = spawnBuildAsk.Get();
                spawnBuildMessage.OwnerId = player.OwnerSignatureId;
                spawnBuildMessage.PrefabId = representedBuilding.GetComponent<NetworkEntity>().PrefabId;

                spawnBuildMessage.PosX = hit.point.x;
                spawnBuildMessage.PosY = hit.point.y;
                spawnBuildMessage.PosZ = hit.point.z;

                RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(spawnBuildMessage);
            }

            Destroy(buildingPreviewInstance);
            buildingRendererInstance = null;
        }
        else if (hasHit)
        {
            buildingPreviewInstance.transform.position = hit.point;

            if (!buildingPreviewInstance.activeSelf)
            {
                buildingPreviewInstance.SetActive(true);
            }

            Color color = player.CanPlaceBuilding(buildingCollider, hit.point) ? canPlaceColor : canNotPlaceColor;

            foreach (Material material in buildingRendererInstance.materials)
            {
                material.SetColor("_BaseColor", color);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        if (player.GetResources() < representedBuilding.GetPrice())
        {
            return;
        }

        buildingPreviewInstance = Instantiate(representedBuilding.GetBuildingPreview());
        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

        buildingPreviewInstance.SetActive(false);
    }
}
