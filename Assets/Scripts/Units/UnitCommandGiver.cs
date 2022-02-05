using Forge.Networking.Messaging;
using Forge.Networking.Unity;
using Forge.Networking.Unity.Messages;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    private Camera mainCamera;

    private MessagePool<SetNewTargetMessage> newTargetPool = new MessagePool<SetNewTargetMessage>();
    private MessagePool<SetNewMovePointMessage> newMovePool = new MessagePool<SetNewMovePointMessage>();

#if !UNITY_SERVER

    private void Start()
    {
        mainCamera = Camera.main;

        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame)
        {
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return;
        }

        if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
        {
            if (target.IsOwner)
            {
                TryMove(hit.point);
                return;
            }

            TryTarget(target);
        }
        else
        {
            TryMove(hit.point);
        }
    }

#endif

    private void ClientHandleGameOver(string obj)
    {
        enabled = false;
    }


    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in unitSelectionHandler.GetSelectedUnits())
        {
            var netTargetMessage = newTargetPool.Get();
            netTargetMessage.EntityId = unit.EntityId;

            netTargetMessage.TargetEntityId = target.EntityId;

            RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(netTargetMessage);
        }
    }

    private void TryMove(Vector3 point)
    {
        foreach (Unit unit in unitSelectionHandler.GetSelectedUnits())
        {
            var newMovePointMessage = newMovePool.Get();
            newMovePointMessage.EntityId = unit.EntityId;

            newMovePointMessage.PosX = point.x;
            newMovePointMessage.PosY = point.y;
            newMovePointMessage.PosZ = point.z;

            RTSNetworkManager.Instance.Facade.NetworkMediator.SendReliableMessage(newMovePointMessage);
        }
    }
}
