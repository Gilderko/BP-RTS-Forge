using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class SpawnPlayerObjectInterpreter : IMessageInterpreter
	{
		public static SpawnPlayerObjectInterpreter Instance { get; private set; } = new SpawnPlayerObjectInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (SpawnPlayerObjectMessage)message;

			if (RTSNetworkManager.Instance.GetRTSPlayerById(castedMessage.OwnerId.GetId()) != null)
            {
				return;
            }

			IEngineFacade engine = (IEngineFacade)netMediator.EngineProxy;
            try
            {
				var playerObject = EntitySpawner.SpawnEntityFromMessage(engine, castedMessage).OwnerGameObject.GetComponent<RTSPlayer>();
				playerObject.ClientSetNewTeamColor(new Color(castedMessage.Red, castedMessage.Green, castedMessage.Blue));
				playerObject.ClientSetNewPlayerName(castedMessage.PlayerName);
				playerObject.ClientSetPlayerOwnsSession(castedMessage.IsTeamOwner);
				playerObject.ClientSetResources(castedMessage.MoneyStart);

				RTSNetworkManager.Instance.ClientHandleClientConnected(castedMessage.OwnerId.GetId());
			}
			catch (EntityAlreadyInRepositoryException)
            {
				return;
            }			
		}
	}
}