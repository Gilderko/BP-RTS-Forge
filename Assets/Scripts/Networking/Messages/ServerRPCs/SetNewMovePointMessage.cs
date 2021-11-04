using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;
using UnityEngine;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(24, typeof(SetNewMovePointMessage))]
	public class SetNewMovePointMessage : ForgeMessage
	{
		public int EntityId { get; set; }
		public float PosX { get; set; }
		public float PosY { get; set; }
		public float PosZ { get; set; }

		public override IMessageInterpreter Interpreter => SetNewMovePointInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			base.Deserialize(buffer);

			EntityId = ForgeSerializer.Instance.Deserialize<int>(buffer);
			PosX = ForgeSerializer.Instance.Deserialize<float>(buffer);
			PosY = ForgeSerializer.Instance.Deserialize<float>(buffer);
			PosZ = ForgeSerializer.Instance.Deserialize<float>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			base.Serialize(buffer);

			ForgeSerializer.Instance.Serialize(EntityId, buffer);
			ForgeSerializer.Instance.Serialize(PosX, buffer);
			ForgeSerializer.Instance.Serialize(PosY, buffer);
			ForgeSerializer.Instance.Serialize(PosZ, buffer);
		}
	}
}
