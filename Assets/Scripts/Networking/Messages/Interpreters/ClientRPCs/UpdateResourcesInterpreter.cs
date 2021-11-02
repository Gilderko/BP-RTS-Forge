using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class UpdateResourcesInterpreter : IMessageInterpreter
	{
		public static UpdateResourcesInterpreter Instance { get; private set; } = new UpdateResourcesInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (UpdateResourcesMessage)message;
			var player = RTSNetworkManager.Instance.LocalPlayer;

			player.ClientSetResources(castedMessage.ResourcesValue);
		}
	}
}