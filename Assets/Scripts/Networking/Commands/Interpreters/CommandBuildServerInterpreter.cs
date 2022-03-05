using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class CommandBuildServerInterpreter : IMessageInterpreter
	{
		public static CommandBuildServerInterpreter Instance { get; private set; } = new CommandBuildServerInterpreter();

		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		private int lastRequestId = -1;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (CommandBuildServerMessage)message;

			if (lastRequestId < castedMessage.RequestId)
            {
				Commander.Instance.CmdTellClientsToBuild(castedMessage.BuildingId, castedMessage.RequestId);
				lastRequestId = castedMessage.RequestId;
			}			
		}
	}
}