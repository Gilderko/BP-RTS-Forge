using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(32, typeof(UpdateResourcesMessage))]
	public class UpdateResourcesMessage : ForgeMessage
	{
		public int PlayerId { get; set; }

		public int ResourcesValue { get; set; }

		public override IMessageInterpreter Interpreter => UpdateResourcesInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			base.Deserialize(buffer);

			PlayerId = ForgeSerializer.Instance.Deserialize<int>(buffer);
			ResourcesValue = ForgeSerializer.Instance.Deserialize<int>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			base.Serialize(buffer);

			ForgeSerializer.Instance.Serialize(PlayerId, buffer);
			ForgeSerializer.Instance.Serialize(ResourcesValue, buffer);
		}
	}
}
