using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages;
using UnityEngine;

/// <summary>
/// Custom implementation with the help from an existing NetworkTransform
/// </summary>
public class NetworkTransform : NetworkBehaviour
{
    // Server variables

    [Header("Server config")]
    [SerializeField] private float UPDATE_INTERVAL = 0.1f;
    [SerializeField] private float MIN_POSITION_DELTA = 0.1f;
    [SerializeField] private float MIN_ROTATION_DELTA = 0.1f;

    private float _updateDelta = 0;
    private MessagePool<TransformUpdateMessage> _msgPool = new MessagePool<TransformUpdateMessage>();

    // Server variables

    // Client variables
    private Vector3 _targetPosition = Vector3.zero;
    private Quaternion _targetRotation = Quaternion.identity;

    private Vector3 _previousPosition = Vector3.zero;
    private Quaternion _previousRotation = Quaternion.identity;

    // Client variables

    private void Awake()
    {
        _targetPosition = transform.position;
        _targetRotation = transform.rotation;
        _previousPosition = transform.position;
        _previousRotation = transform.rotation;
    }

    private void Update()
    {
        if (IsServer)
        {
            _updateDelta += Time.deltaTime;

            if ((transform.position - _previousPosition).sqrMagnitude < MIN_POSITION_DELTA * MIN_POSITION_DELTA
                && Quaternion.Angle(_previousRotation, transform.rotation) < MIN_ROTATION_DELTA)
            {
                return;
            }

            if (_updateDelta >= UPDATE_INTERVAL)
            {
                var transfUpdateMessage = _msgPool.Get();
                transfUpdateMessage.EntityId = EntityId;
                transfUpdateMessage.Position = transform.position;

                transfUpdateMessage.RotationX = transform.rotation.eulerAngles.x;
                transfUpdateMessage.RotationY = transform.rotation.eulerAngles.y;
                transfUpdateMessage.RotationZ = transform.rotation.eulerAngles.z;

                RTSNetworkManager.Instance.Facade.NetworkMediator.SendMessage(transfUpdateMessage);

                _previousPosition = transform.position;
                _previousRotation = transform.rotation;
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
