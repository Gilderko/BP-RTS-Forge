using Forge.Networking.Messaging;
using System.Net;

namespace Forge.Networking.Unity.Messages.Interpreters
{
    public class UpdateResourcesInterpreter : IMessageInterpreter
    {
        public static UpdateResourcesInterpreter Instance { get; private set; } = new UpdateResourcesInterpreter();

        public bool ValidOnClient => true;
        public bool ValidOnServer => false;

        public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
        {
            var castedMessage = (UpdateResourcesMessage)message;
            var player = RTSNetworkManager.Instance.LocalPlayer;

            if (player == null)
            {
                return;
            }

            player.ClientSetResources(castedMessage.ResourcesValue);
        }
    }
}