using Forge.Networking.Messaging;
using System.Net;

namespace Forge.Networking.Unity.Messages.Interpreters
{
    public class ChangePlayerSessionOwnershipInterpreter : IMessageInterpreter
    {
        public static ChangePlayerSessionOwnershipInterpreter Instance { get; private set; } = new ChangePlayerSessionOwnershipInterpreter();

        public bool ValidOnClient => true;
        public bool ValidOnServer => false;

        public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
        {
            var castedMessage = (ChangePlayerSessionOwnershipMessage)message;
            var player = RTSNetworkManager.Instance.GetRTSPlayerById(castedMessage.PlayerId);

            if (player == null)
            {
                return;
            }

            player.ClientSetPlayerOwnsSession(castedMessage.IsOwner);
        }
    }
}