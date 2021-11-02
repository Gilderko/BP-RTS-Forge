using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(21, typeof(LoadSceneFromServerMessage))]
	public class LoadSceneFromServerMessage : ForgeMessage
	{
		public string SceneName { get; set; }

		public override IMessageInterpreter Interpreter => LoadSceneFromServerInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			SceneName = ForgeSerializer.Instance.Deserialize<string>(buffer);	
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(SceneName, buffer);
		}
	}
}
