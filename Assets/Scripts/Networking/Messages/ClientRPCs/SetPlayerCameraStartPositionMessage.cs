using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(17, typeof(SetPlayerCameraStartPositionMessage))]
	public class SetPlayerCameraStartPositionMessage : ForgeMessage
	{
		public float PosX { get; set; }
		public float PosY { get; set; }
		public float PosZ { get; set; }

		public override IMessageInterpreter Interpreter => SetPlayerCameraStartPositionInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			

			PosX = ForgeSerializer.Instance.Deserialize<float>(buffer);
			PosY = ForgeSerializer.Instance.Deserialize<float>(buffer);
			PosZ = ForgeSerializer.Instance.Deserialize<float>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			

			ForgeSerializer.Instance.Serialize(PosX, buffer);
			ForgeSerializer.Instance.Serialize(PosY, buffer);
			ForgeSerializer.Instance.Serialize(PosZ, buffer);
		}
	}
}
