using System;
using System.Net;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using UnityEngine;

namespace Forge.Networking.Unity.Messages.Interpreters
{
	public class AnnounceWinnerInterpreter : IMessageInterpreter
	{
		public static AnnounceWinnerInterpreter Instance { get; private set; } = new AnnounceWinnerInterpreter();

		public bool ValidOnClient => true;
		public bool ValidOnServer => false;

		public void Interpret(INetworkMediator netMediator, EndPoint sender, IMessage message)
		{
			var castedMessage = (AnnounceWinnerMessage)message;
			var entity = ((IEngineFacade)netMediator.EngineProxy).EntityRepository.Get(castedMessage.ObjectId);
			entity.OwnerGameObject.GetComponent<GameOverHandler>().ClientHandleGameOver(castedMessage.WinningPlayerName);
		}
	}
}