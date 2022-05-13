using Forge.Networking.Messaging;
using System.Net;

namespace Forge.Networking.Unity.Messages.Interpreters
{
    public class LoadSceneFromServerInterpreter : IMessageInterpreter
    {
        public static LoadSceneFromServerInterpreter Instance { get; private set; } = new LoadSceneFromServerInterpreter();

        public bool ValidOnClient => true;
        public bool ValidOnServer => false;

        public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
        {
            var castedMessage = (LoadSceneFromServerMessage)message;

            RTSNetworkManager.Instance.StartCoroutine(RTSNetworkManager.Instance.ClientLoadLevel(castedMessage.SceneName));
        }
    }
}