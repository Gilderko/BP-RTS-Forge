using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class ChangePlayerNameInterpreter : IMessageInterpreter
	{
		public static ChangePlayerNameInterpreter Instance { get; private set; } = new ChangePlayerNameInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (ChangePlayerNameMessage)message;
			var player = RTSNetworkManager.Instance.GetRTSPlayerById(castedMessage.PlayerId);
			player.ClientSetNewPlayerName(castedMessage.NewPlayerName);
		}
	}
}