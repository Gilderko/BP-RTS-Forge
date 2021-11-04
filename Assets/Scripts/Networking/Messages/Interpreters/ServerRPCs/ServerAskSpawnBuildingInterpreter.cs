using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
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

			var player = RTSNetworkManager.Instance.GetRTSPlayerById(castedMessage.OwnerId.GetId());
			player.CmdTryPlaceBuildingServerRpc(castedMessage.PrefabId, new Vector3(castedMessage.PosX, castedMessage.PosY, castedMessage.PosZ));
		}
	}
}