using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(40, typeof(CommandBuildClientsMessage))]
	public class CommandBuildClientsMessage : ForgeMessage
	{
		public int RequestId { get; set; }
		public int BuildingId { get; set; }

		public override IMessageInterpreter Interpreter => CommandBuildClientsInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			RequestId = ForgeSerializer.Instance.Deserialize<int>(buffer);
			BuildingId = ForgeSerializer.Instance.Deserialize<int>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize<int>(RequestId, buffer);
			ForgeSerializer.Instance.Serialize<int>(BuildingId, buffer);
		}
	}
}
