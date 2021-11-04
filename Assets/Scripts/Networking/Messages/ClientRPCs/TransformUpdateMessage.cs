using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;
using UnityEngine;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(36, typeof(TransformUpdateMessage))]
	public class TransformUpdateMessage : ForgeMessage
	{
		public int EntityId { get; set; }
		public Vector3 Position { get; set; }
		public float RotationX { get; set; }
		public float RotationY { get; set; }
		public float RotationZ { get; set; }

		public override IMessageInterpreter Interpreter => TransformUpdateInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			base.Deserialize(buffer);

			EntityId = ForgeSerializer.Instance.Deserialize<int>(buffer);
			Position = ForgeSerializer.Instance.Deserialize<Vector3>(buffer);
			RotationX = ForgeSerializer.Instance.Deserialize<float>(buffer);
			RotationY = ForgeSerializer.Instance.Deserialize<float>(buffer);
			RotationZ = ForgeSerializer.Instance.Deserialize<float>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			base.Serialize(buffer);

			ForgeSerializer.Instance.Serialize(EntityId, buffer);
			ForgeSerializer.Instance.Serialize(Position, buffer);
			ForgeSerializer.Instance.Serialize(RotationX, buffer);
			ForgeSerializer.Instance.Serialize(RotationY, buffer);
			ForgeSerializer.Instance.Serialize(RotationZ, buffer);
		}
	}
}
