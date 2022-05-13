using Forge.Networking.Messaging;
using System.Net;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
    public class TransformUpdateInterpreter : IMessageInterpreter
    {
        public static TransformUpdateInterpreter Instance { get; private set; } = new TransformUpdateInterpreter();

        public bool ValidOnClient => true;
        public bool ValidOnServer => false;

        public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
        {
            var castedMessage = (TransformUpdateMessage)message;
            IEngineFacade engine = (IEngineFacade)netMediator.EngineProxy;
            try
            {
                IUnityEntity entity = engine.EntityRepository.Get(castedMessage.EntityId);
                var interpolator = entity.OwnerGameObject.GetComponent<NetworkTransform>();

                interpolator.UpdateInterpolation(castedMessage.Position, Quaternion.Euler(castedMessage.RotationX, castedMessage.RotationY, castedMessage.RotationZ));
            }
            catch (EntityNotFoundException)
            {
                return;
            }

        }
    }
}