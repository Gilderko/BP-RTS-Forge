using Forge.Networking.Messaging;
using System.Net;

namespace Forge.Networking.Unity.Messages.Interpreters
{
    public class UpdateHealthInterpreter : IMessageInterpreter
    {
        public static UpdateHealthInterpreter Instance { get; private set; } = new UpdateHealthInterpreter();

        public bool ValidOnClient => true;
        public bool ValidOnServer => false;

        public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
        {
            var castedMessage = (UpdateHealthMessage)message;

            var entity = ((IEngineFacade)netMediator.EngineProxy).EntityRepository.Get(castedMessage.EntityID);
            entity.OwnerGameObject.GetComponent<Health>().ClientSetHealth(castedMessage.NewHealthValue);
        }
    }
}