using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class CommandMoveServerInterpreter : IMessageInterpreter
	{
		public static CommandMoveServerInterpreter Instance { get; private set; } = new CommandMoveServerInterpreter();

		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		private int lastRequestId = -1;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (CommandMoveServerMessage)message;

			if (lastRequestId < castedMessage.RequestId)
			{
				Commander.Instance.CmdTellClientsToMoveUnits(castedMessage.RequestId);
				lastRequestId = castedMessage.RequestId;
			}
		}
	}
}