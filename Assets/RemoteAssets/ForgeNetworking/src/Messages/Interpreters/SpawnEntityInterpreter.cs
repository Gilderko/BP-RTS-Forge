using Forge.Networking.Messaging;
using System.Net;

namespace Forge.Networking.Unity.Messages.Interpreters
{
    public class SpawnEntityInterpreter : IMessageInterpreter
    {
        public static SpawnEntityInterpreter Instance { get; private set; } = new SpawnEntityInterpreter();

        public bool ValidOnClient => true;
        public bool ValidOnServer => true;

        public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
        {
            var msg = (SpawnEntityMessage)message;
            try
            { 
                IEngineFacade engine = (IEngineFacade)netMediator.EngineProxy;

                if (engine.EntityRepository.Exists(msg.Id))
                {
                    return;
                }

                EntitySpawner.SpawnEntityFromMessage(engine, msg);
            }
            catch (EntityAlreadyInRepositoryException)
            {
                return;
            }
        }
    }
}