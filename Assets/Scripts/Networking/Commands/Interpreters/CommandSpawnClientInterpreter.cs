using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class CommandSpawnClientInterpreter : IMessageInterpreter
	{
		public static CommandSpawnClientInterpreter Instance { get; private set; } = new CommandSpawnClientInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		private int lastRequestId = -1;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (CommandSpawnClientMessage)message;

			if (lastRequestId < castedMessage.RequestId)
			{
				Commander.Instance.RpcClientsSpawnUnit();
				lastRequestId = castedMessage.RequestId;
			}
		}
	}
}