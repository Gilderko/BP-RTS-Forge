using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class ChangePlayerSessionOwnershipInterpreter : IMessageInterpreter
	{
		public static ChangePlayerSessionOwnershipInterpreter Instance { get; private set; } = new ChangePlayerSessionOwnershipInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (ChangePlayerSessionOwnershipMessage)message;
			var player = RTSNetworkManager.Instance.GetRTSPlayerByUID(castedMessage.PlayerId);
			player.ClientSetPlayerOwnsSession(castedMessage.IsOwner);
		}
	}
}