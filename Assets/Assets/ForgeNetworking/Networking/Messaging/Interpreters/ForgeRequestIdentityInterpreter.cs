using Forge.Networking.Messaging.Messages;
using System.Net;

namespace Forge.Networking.Messaging.Interpreters
{
    public class ForgeRequestIdentityInterpreter : IMessageInterpreter
    {
        public bool ValidOnClient => false;
        public bool ValidOnServer => true;

        public void Interpret(INetworkMediator netHost, EndPoint sender, IMessage message)
        {
            var player = netHost.PlayerRepository.GetPlayer(sender);
            var identityMessage = new ForgeNetworkIdentityMessage
            {
                Identity = player.Id
            };
            netHost.MessageBus.SendReliableMessage(identityMessage, netHost.SocketFacade.ManagedSocket, sender);
        }
    }
}
