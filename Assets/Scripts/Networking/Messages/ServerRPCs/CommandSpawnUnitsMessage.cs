using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(20, typeof(CommandSpawnUnitsMessage))]
	public class CommandSpawnUnitsMessage : ForgeMessage
	{
		public int EntityId { get; set; }

		public override IMessageInterpreter Interpreter => CommandSpawnUnitsInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			base.Deserialize(buffer);

			EntityId = ForgeSerializer.Instance.Deserialize<int>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			base.Serialize(buffer);

			ForgeSerializer.Instance.Serialize(EntityId, buffer);
		}
	}
}
