using Forge.Networking.Messaging;
using System.Net;

namespace Forge.Networking.Unity.Messages.Interpreters
{
    public class AnnounceWinnerInterpreter : IMessageInterpreter
    {
        public static AnnounceWinnerInterpreter Instance { get; private set; } = new AnnounceWinnerInterpreter();

        public bool ValidOnClient => true;
        public bool ValidOnServer => false;

        public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
        {
            var castedMessage = (AnnounceWinnerMessage)message;
            try
            {
                var entity = ((IEngineFacade)netMediator.EngineProxy).EntityRepository.Get(castedMessage.ObjectId);
                entity.OwnerGameObject.GetComponent<GameOverHandler>().ClientHandleGameOver(castedMessage.WinningPlayerName);
            }
            catch (EntityNotFoundException)
            {
                return;
            }
        }
    }
}