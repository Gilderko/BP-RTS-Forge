using System;
using System.Collections.Generic;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class ForceBuildServerInterpreter : IMessageInterpreter
	{
		public static ForceBuildServerInterpreter Instance { get; private set; } = new ForceBuildServerInterpreter();

		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		private Dictionary<int, int> dictionary = new Dictionary<int, int>();

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (ForceBuildServerMessage)message;

			if (!dictionary.ContainsKey(castedMessage.OwnerId.GetId()))
            {
				dictionary.Add(castedMessage.OwnerId.GetId(), castedMessage.RequestId);
			}


			if (dictionary[castedMessage.OwnerId.GetId()] < castedMessage.RequestId)
            {
				var player = RTSNetworkManager.Instance.GetRTSPlayerById(castedMessage.OwnerId.GetId());
				player.ForcePlaceBuildingServerRPc(castedMessage.PrefabId,	new Vector3(castedMessage.PosX, castedMessage.PosY, castedMessage.PosZ));
				dictionary[castedMessage.OwnerId.GetId()] = castedMessage.RequestId;
			}
		}
	}
}