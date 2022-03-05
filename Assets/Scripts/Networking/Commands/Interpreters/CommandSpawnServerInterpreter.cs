using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class CommandSpawnServerInterpreter : IMessageInterpreter
	{
		public static CommandSpawnServerInterpreter Instance { get; private set; } = new CommandSpawnServerInterpreter();

		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		private int lastRequestId = -1;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (CommandSpawnServerMessage)message;

			if (lastRequestId < castedMessage.RequestId)
			{
				Commander.Instance.CmdTellClientsToSpawnUnits(castedMessage.RequestId);
				lastRequestId = castedMessage.RequestId;
			}
		}
	}
}