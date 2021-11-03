using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class SetNewMovePointInterpreter : IMessageInterpreter
	{
		public static SetNewMovePointInterpreter Instance { get; private set; } = new SetNewMovePointInterpreter();

		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (SetNewMovePointMessage)message;
			var entityToSet = ((IEngineFacade)netMediator.EngineProxy).EntityRepository.Get(castedMessage.EntityId);

			Vector3 newPosition = new Vector3(castedMessage.PosX, castedMessage.PosY, castedMessage.PosZ);
			entityToSet.OwnerGameObject.GetComponent<Unit>().GetUnitMovement().CmdMoveServerRpc(newPosition);
		}
	}
}