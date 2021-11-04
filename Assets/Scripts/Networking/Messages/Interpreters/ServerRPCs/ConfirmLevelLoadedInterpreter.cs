using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class ConfirmLevelLoadedInterpreter : IMessageInterpreter
	{
		public static ConfirmLevelLoadedInterpreter Instance { get; private set; } = new ConfirmLevelLoadedInterpreter();

		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (ConfirmLevelLoadedMessage)message;

			RTSNetworkManager.Instance.AddPlayerReady(castedMessage.ConfirmedPlayer);
		}
	}
}