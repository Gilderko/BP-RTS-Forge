using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(19, typeof(UpdateUnitSpawnerQueMessage))]
	public class UpdateUnitSpawnerQueMessage : ForgeMessage
	{
		public int EntityId { get; set; }

		public int NewQueAmmount { get; set; }

		public override IMessageInterpreter Interpreter => UpdateUnitSpawnerQueInterpreter.Instance;

        public bool IsIncrease { get; internal set; }

        public override void Deserialize(BMSByte buffer)
		{
			base.Deserialize(buffer);

			EntityId = ForgeSerializer.Instance.Deserialize<int>(buffer);
			NewQueAmmount = ForgeSerializer.Instance.Deserialize<int>(buffer);
			IsIncrease = ForgeSerializer.Instance.Deserialize<bool>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			base.Serialize(buffer);

			ForgeSerializer.Instance.Serialize(EntityId, buffer);
			ForgeSerializer.Instance.Serialize(NewQueAmmount, buffer);
			ForgeSerializer.Instance.Serialize(IsIncrease, buffer);
		}
	}
}
