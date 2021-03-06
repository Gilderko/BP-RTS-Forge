using Forge.Networking.Messaging;
using System.Linq;
using System.Net;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
    public class ServerAskSpawnBuildingInterpreter : IMessageInterpreter
    {
        public static ServerAskSpawnBuildingInterpreter Instance { get; private set; } = new ServerAskSpawnBuildingInterpreter();

        public bool ValidOnClient => false;
        public bool ValidOnServer => true;

        public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
        {
            var castedMessage = (ServerAskSpawnBuildingMessage)message;

            //Debug.Log($"Player {castedMessage.OwnerId} is asking to build a building");

            var player = RTSNetworkManager.Instance.GetRTSPlayerById(castedMessage.OwnerId.GetId());
            if (player == null)
            {
                return;
            }

            if (player.GetMyBuildings().Count() != castedMessage.BuildingCount)
            {
                Debug.Log($"Server sice Building count of player {player.GetMyBuildings().Count()} is different from one in message {castedMessage.BuildingCount}");
                return;
            }

            player.CmdTryPlaceBuildingServerRpc(castedMessage.PrefabId, new Vector3(castedMessage.PosX, castedMessage.PosY, castedMessage.PosZ));
        }
    }
}