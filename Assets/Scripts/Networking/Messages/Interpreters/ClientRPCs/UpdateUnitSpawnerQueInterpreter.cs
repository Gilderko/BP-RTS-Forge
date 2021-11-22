using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class UpdateUnitSpawnerQueInterpreter : IMessageInterpreter
	{
		public static UpdateUnitSpawnerQueInterpreter Instance { get; private set; } = new UpdateUnitSpawnerQueInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (UpdateUnitSpawnerQueMessage)message;

			try
            {
				var unitSpawner = ((IEngineFacade)netMediator.EngineProxy).EntityRepository.Get(castedMessage.EntityId).OwnerGameObject;

				unitSpawner.GetComponent<UnitSpawner>().ClientUpdateQueAmmount(castedMessage.NewQueAmmount, castedMessage.IsIncrease);
			}
			catch (EntityNotFoundException)
            {
				return;
            }			
		}
	}
}