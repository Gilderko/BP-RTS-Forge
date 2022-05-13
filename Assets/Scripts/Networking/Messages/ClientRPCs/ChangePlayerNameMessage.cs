using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
    [EngineMessageContract(16, typeof(ChangePlayerNameMessage))]
    public class ChangePlayerNameMessage : ForgeMessage
    {
        public int PlayerId { get; set; }

        public string NewPlayerName { get; set; }

        public override IMessageInterpreter Interpreter => ChangePlayerNameInterpreter.Instance;

        public override void Deserialize(BMSByte buffer)
        {


            PlayerId = ForgeSerializer.Instance.Deserialize<int>(buffer);
            NewPlayerName = ForgeSerializer.Instance.Deserialize<string>(buffer);
        }

        public override void Serialize(BMSByte buffer)
        {


            ForgeSerializer.Instance.Serialize(PlayerId, buffer);
            ForgeSerializer.Instance.Serialize(NewPlayerName, buffer);
        }
    }
}
