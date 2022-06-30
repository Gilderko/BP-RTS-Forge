using Forge.Networking.Messaging;
using System.Net;

namespace Forge.Networking.Unity.Messages.Interpreters
{
    public class DespawnEntityInterpreter : IMessageInterpreter
    {
        public static DespawnEntityInterpreter Instance { get; private set; } = new DespawnEntityInterpreter();

        public bool ValidOnClient => true;
        public bool ValidOnServer => false;

        public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
        {
            var castedMessage = (DespawnEntityMessage)message;

            try
            {
                var engine = ((IEngineFacade)netMediator.EngineProxy);
                var entity = engine.EntityRepository.Get(castedMessage.EntityId);

                UnityEngine.Object.Destroy(entity.OwnerGameObject);

                engine.EntityRepository.Remove(entity);
            }
            catch (EntityNotFoundException)
            {
                return;
            }

        }
    }
}