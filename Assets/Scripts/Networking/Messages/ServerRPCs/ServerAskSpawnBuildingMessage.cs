using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(35, typeof(ServerAskSpawnBuildingMessage))]
	public class ServerAskSpawnBuildingMessage : ForgeMessage
	{
		public override IMessageInterpreter Interpreter => ServerAskSpawnBuildingInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			
		}

		public override void Serialize(BMSByte buffer)
		{
			
		}
	}
}
