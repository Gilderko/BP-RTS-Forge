using Forge.Factory;
using Forge.Networking.Messaging.Messages;
using System.Net;

namespace Forge.Networking.Messaging.Interpreters
{
    public class ForgeConnectChallengeInterpreter : IMessageInterpreter
    {
        public bool ValidOnClient => true;
        public bool ValidOnServer => false;

        public void Interpret(INetworkMediator netHost, EndPoint sender, IMessage message)
        {
            var response = AbstractFactory.Get<INetworkTypeFactory>().GetNew<IChallengeResponseMessage>();
            response.GenerateResponse((IChallengeMessage)message);
            netHost.MessageBus.SendReliableMessage(response, netHost.SocketFacade.ManagedSocket, sender);
        }
    }
}
