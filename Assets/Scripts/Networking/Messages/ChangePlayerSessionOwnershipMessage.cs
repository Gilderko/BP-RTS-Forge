using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(15, typeof(ChangePlayerSessionOwnershipMessage))]
	public class ChangePlayerSessionOwnershipMessage : ForgeMessage
	{
		public override IMessageInterpreter Interpreter => ChangePlayerSessionOwnershipInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			
		}

		public override void Serialize(BMSByte buffer)
		{
			
		}
	}
}
