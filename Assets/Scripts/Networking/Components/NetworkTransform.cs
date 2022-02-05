using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTransform : NetworkBehaviour
{
    // Server variables
    
    [Header("Server config")]
    [SerializeField] private float UPDATE_INTERVAL = 0.1f;

    private float _updateDelta = 0;
    private MessagePool<TransformUpdateMessage> _msgPool = new MessagePool<TransformUpdateMessage>();

    // Server variables

    // Client variables
    private Vector3 _targetPosition = Vector3.zero;
    private Quaternion _targetRotation = Quaternion.identity;

    // Client variables

    private void Awake()
    {
        _targetPosition = transform.position;
        _targetRotation = transform.rotation;
    }

    void Update()
    {
        if (IsServer)
        {
            _updateDelta += Time.deltaTime;

            if (_updateDelta >= UPDATE_INTERVAL)
            {
                var transfUpdateMessage = _msgPool.Get();
                transfUpdateMessage.EntityId = EntityId;
                transfUpdateMessage.Position = transform.position;

                transfUpdateMessage.RotationX = transform.rotation.eulerAngles.x;
                transfUpdateMessage.RotationY = transform.rotation.eulerAngles.y;
                transfUpdateMessage.RotationZ = transform.rotation.eulerAngles.z;

                RTSNetworkManager.Instance.Facade.NetworkMediator.SendMessage(transfUpdateMessage);

                _updateDelta = 0;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, _targetPosition, UPDATE_INTERVAL);
            transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, UPDATE_INTERVAL);
        }
    }

    public void UpdateInterpolation(Vector3 position, Quaternion rotation)
    {
        _targetPosition = position;
        _targetRotation = rotation;
    }
}
