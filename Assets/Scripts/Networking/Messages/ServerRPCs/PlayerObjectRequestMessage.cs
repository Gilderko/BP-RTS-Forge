using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
    [EngineMessageContract(37, typeof(PlayerObjectRequestMessage))]
    public class PlayerObjectRequestMessage : ForgeMessage
    {
        public IPlayerSignature RequestingPlayer { get; set; }

        public override IMessageInterpreter Interpreter => PlayerObjectRequestInterpreter.Instance;

        public override void Deserialize(BMSByte buffer)
        {
            RequestingPlayer = ForgeSerializer.Instance.Deserialize<IPlayerSignature>(buffer);
        }

        public override void Serialize(BMSByte buffer)
        {
            ForgeSerializer.Instance.Serialize(RequestingPlayer, buffer);
        }
    }
}
