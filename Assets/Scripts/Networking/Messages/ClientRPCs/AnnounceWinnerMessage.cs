using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(15, typeof(AnnounceWinnerMessage))]
	public class AnnounceWinnerMessage : ForgeMessage
	{
		public int ObjectId { get; set; }
		public string WinningPlayerName { get; set; }

		public override IMessageInterpreter Interpreter => AnnounceWinnerInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			ObjectId = ForgeSerializer.Instance.Deserialize<int>(buffer);
			WinningPlayerName = ForgeSerializer.Instance.Deserialize<string>(buffer);	
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(ObjectId, buffer);
			ForgeSerializer.Instance.Serialize(WinningPlayerName, buffer);
		}
	}
}
