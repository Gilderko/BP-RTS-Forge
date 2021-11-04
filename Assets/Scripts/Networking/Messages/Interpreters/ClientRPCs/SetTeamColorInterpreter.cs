using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class SetTeamColorInterpreter : IMessageInterpreter
	{
		public static SetTeamColorInterpreter Instance { get; private set; } = new SetTeamColorInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (SetTeamColorMessage)message;

			var player = RTSNetworkManager.Instance.GetRTSPlayerById(castedMessage.PlayerId);
			player.ClientSetNewTeamColor(new Color(castedMessage.ColorR, castedMessage.ColorG, castedMessage.ColorB));
		}
	}
}