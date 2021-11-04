using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(33, typeof(RequestGameStartMessage))]
	public class RequestGameStartMessage : ForgeMessage
	{
		public int PlayerId { get; set; }

		public override IMessageInterpreter Interpreter => RequestGameStartInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			base.Deserialize(buffer);

			PlayerId = ForgeSerializer.Instance.Deserialize<int>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{

			base.Serialize(buffer);

			ForgeSerializer.Instance.Serialize(PlayerId, buffer);
		}
	}
}
