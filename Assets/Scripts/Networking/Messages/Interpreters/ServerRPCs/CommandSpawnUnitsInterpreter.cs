using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class CommandSpawnUnitsInterpreter : IMessageInterpreter
	{
		public static CommandSpawnUnitsInterpreter Instance { get; private set; } = new CommandSpawnUnitsInterpreter();

		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (CommandSpawnUnitsMessage)message;
			try
            {
				var entity = ((IEngineFacade)(netMediator.EngineProxy)).EntityRepository.Get(castedMessage.EntityId);
				entity.OwnerGameObject.GetComponent<UnitSpawner>().CmdSpawnUnitServerRpc();
			}
			catch (EntityNotFoundException)
            {
				return;
            }		
		}
	}
}