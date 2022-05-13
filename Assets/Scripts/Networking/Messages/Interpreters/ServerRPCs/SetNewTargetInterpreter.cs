using Forge.Networking.Messaging;
using System.Net;

namespace Forge.Networking.Unity.Messages.Interpreters
{
    public class SetNewTargetInterpreter : IMessageInterpreter
    {
        public static SetNewTargetInterpreter Instance { get; private set; } = new SetNewTargetInterpreter();

        public bool ValidOnClient => false;
        public bool ValidOnServer => true;

        public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
        {
            var castedMessage = (SetNewTargetMessage)message;
            try
            {
                var entityToModify = ((IEngineFacade)netMediator.EngineProxy).EntityRepository.Get(castedMessage.EntityId);
                entityToModify.OwnerGameObject.GetComponent<Unit>().GetTargeter().CmdSetTargetServerRpc(castedMessage.TargetEntityId);
            }
            catch (EntityNotFoundException)
            {
                return;
            }
        }
    }
}