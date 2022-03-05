using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(41, typeof(ForceBuildServerMessage))]
	public class ForceBuildServerMessage : ForgeMessage
	{
		public IPlayerSignature OwnerId { get; set; }

		public int PrefabId { get; set; }
		public float PosX { get; set; }
		public float PosY { get; set; }
		public float PosZ { get; set; }
		public int RequestId { get; set; }

		public override IMessageInterpreter Interpreter => ForceBuildServerInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			RequestId = ForgeSerializer.Instance.Deserialize<int>(buffer);
			OwnerId = ForgeSerializer.Instance.Deserialize<IPlayerSignature>(buffer);
			PrefabId = ForgeSerializer.Instance.Deserialize<int>(buffer);
			PosX = ForgeSerializer.Instance.Deserialize<float>(buffer);
			PosY = ForgeSerializer.Instance.Deserialize<float>(buffer);
			PosZ = ForgeSerializer.Instance.Deserialize<float>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(RequestId, buffer);
			ForgeSerializer.Instance.Serialize(OwnerId, buffer);
			ForgeSerializer.Instance.Serialize(PrefabId, buffer);
			ForgeSerializer.Instance.Serialize(PosX, buffer);
			ForgeSerializer.Instance.Serialize(PosY, buffer);
			ForgeSerializer.Instance.Serialize(PosZ, buffer);
		}
	}
}
