using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class PlayerObjectRequestInterpreter : IMessageInterpreter
	{
		public static PlayerObjectRequestInterpreter Instance { get; private set; } = new PlayerObjectRequestInterpreter();

		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (PlayerObjectRequestMessage)message;

			var player = RTSNetworkManager.Instance.GetRTSPlayerById(castedMessage.RequestingPlayer.GetId());

			if (player != null)
            {
				return;
            }

			Debug.Log($"Requesting player is {castedMessage.RequestingPlayer}");

			RTSNetworkManager.Instance.ServerHandleClientConnected(castedMessage.RequestingPlayer);
	}
	}
}