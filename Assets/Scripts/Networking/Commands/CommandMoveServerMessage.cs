using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(44, typeof(CommandMoveServerMessage))]
	public class CommandMoveServerMessage : ForgeMessage
	{
		public int RequestId { get; set; }

		public override IMessageInterpreter Interpreter => CommandMoveServerInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			RequestId = ForgeSerializer.Instance.Deserialize<int>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize<int>(RequestId, buffer);
		}
	}
}
