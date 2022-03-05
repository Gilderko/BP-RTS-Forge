using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class CommandBuildClientsInterpreter : IMessageInterpreter
	{
		public static CommandBuildClientsInterpreter Instance { get; private set; } = new CommandBuildClientsInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		private int lastRequestId = -1;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (CommandBuildClientsMessage)message;

			if (lastRequestId < castedMessage.RequestId)
			{
				Commander.Instance.RpcClientsBuild(castedMessage.BuildingId, castedMessage.RequestId);
				lastRequestId = castedMessage.RequestId;
			}
		}
	}
}