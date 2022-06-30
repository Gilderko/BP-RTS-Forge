using Forge.Networking.Messaging;
using System.Net;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
    public class SetPlayerCameraStartPositionInterpreter : IMessageInterpreter
    {
        public static SetPlayerCameraStartPositionInterpreter Instance { get; private set; } = new SetPlayerCameraStartPositionInterpreter();

        public bool ValidOnClient => true;
        public bool ValidOnServer => false;

        public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
        {
            var castedMessage = (SetPlayerCameraStartPositionMessage)message;
            var player = RTSNetworkManager.Instance.LocalPlayer;

            if (player == null)
            {
                return;
            }

            player.ClientSetCameraStartingPosition(new Vector3(castedMessage.PosX, castedMessage.PosY, castedMessage.PosZ));
        }
    }
}