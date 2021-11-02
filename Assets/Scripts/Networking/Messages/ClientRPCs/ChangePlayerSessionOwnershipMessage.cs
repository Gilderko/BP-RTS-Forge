using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(15, typeof(ChangePlayerSessionOwnershipMessage))]
	public class ChangePlayerSessionOwnershipMessage : ForgeMessage
	{
		public int PlayerId { get; set; }
		public bool IsOwner { get; set; }

		public override IMessageInterpreter Interpreter => ChangePlayerSessionOwnershipInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			PlayerId = ForgeSerializer.Instance.Deserialize<int>(buffer);
			IsOwner = ForgeSerializer.Instance.Deserialize<bool>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(PlayerId, buffer);
			ForgeSerializer.Instance.Serialize(IsOwner, buffer);
		}
	}
}
