using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;
using UnityEngine;

namespace Forge.Networking.Unity.Messages
{
	[EngineMessageContract(18, typeof(SetTeamColorMessage))]
	public class SetTeamColorMessage : ForgeMessage
	{
		public int PlayerId { get; set; }

		public float ColorR { get; set; }

		public float ColorG { get; set; }

		public float ColorB { get; set; }

		public override IMessageInterpreter Interpreter => SetTeamColorInterpreter.Instance;

		public override void Deserialize(BMSByte buffer)
		{
			PlayerId = ForgeSerializer.Instance.Deserialize<int>(buffer);
			ColorR = ForgeSerializer.Instance.Deserialize<float>(buffer);
			ColorG = ForgeSerializer.Instance.Deserialize<float>(buffer);
			ColorB = ForgeSerializer.Instance.Deserialize<float>(buffer);
		}

		public override void Serialize(BMSByte buffer)
		{
			ForgeSerializer.Instance.Serialize(PlayerId, buffer);
			ForgeSerializer.Instance.Serialize(ColorR, buffer);
			ForgeSerializer.Instance.Serialize(ColorG, buffer);
			ForgeSerializer.Instance.Serialize(ColorB, buffer);
		}
	}
}
