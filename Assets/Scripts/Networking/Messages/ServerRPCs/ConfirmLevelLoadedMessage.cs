using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
    [EngineMessageContract(34, typeof(ConfirmLevelLoadedMessage))]
    public class ConfirmLevelLoadedMessage : ForgeMessage
    {
        public IPlayerSignature ConfirmedPlayer { get; set; }

        public override IMessageInterpreter Interpreter => ConfirmLevelLoadedInterpreter.Instance;

        public override void Deserialize(BMSByte buffer)
        {


            ConfirmedPlayer = ForgeSerializer.Instance.Deserialize<IPlayerSignature>(buffer);
        }

        public override void Serialize(BMSByte buffer)
        {


            ForgeSerializer.Instance.Serialize(ConfirmedPlayer, buffer);
        }
    }
}
