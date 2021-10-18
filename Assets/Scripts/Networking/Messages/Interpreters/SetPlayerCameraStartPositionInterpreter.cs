using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class SetPlayerCameraStartPositionInterpreter : IMessageInterpreter
	{
		public static SetPlayerCameraStartPositionInterpreter Instance { get; private set; } = new SetPlayerCameraStartPositionInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (SetPlayerCameraStartPositionMessage)message;
			RTSNetworkManager.Instance.LocalPlayer.ClientSetCameraStartingPosition(new Vector3(castedMessage.PosX, castedMessage.PosY, castedMessage.PosZ));
		}
	}
}