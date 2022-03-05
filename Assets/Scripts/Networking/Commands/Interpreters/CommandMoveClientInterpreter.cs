using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class CommandMoveClientInterpreter : IMessageInterpreter
	{
		public static CommandMoveClientInterpreter Instance { get; private set; } = new CommandMoveClientInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		private int lastRequestId = -1;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (CommandMoveClientMessage)message;

			if (lastRequestId < castedMessage.RequestId)
			{
				Commander.Instance.RpcClientsSendUnits();
				lastRequestId = castedMessage.RequestId;
			}
		}
	}
}