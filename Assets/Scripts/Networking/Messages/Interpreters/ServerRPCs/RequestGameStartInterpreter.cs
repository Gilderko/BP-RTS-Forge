using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class RequestGameStartInterpreter : IMessageInterpreter
	{
		public static RequestGameStartInterpreter Instance { get; private set; } = new RequestGameStartInterpreter();

		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (RequestGameStartMessage)message;
			RTSPlayer player = RTSNetworkManager.Instance.GetRTSPlayerById(castedMessage.PlayerId);

			player.CmdStartGameServerRpc();
		}
	}
}