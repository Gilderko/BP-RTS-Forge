using Forge.Networking.Messaging.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Messages
{
    [MessageContract(5, typeof(ForgePingMessage))]
    public class ForgePingMessage : ForgeMessage
    {
        public override IMessageInterpreter Interpreter => ForgePingInterpreter.Instance;

        public override void Deserialize(BMSByte buffer)
        {
            base.Deserialize(buffer);

            // Nothing to deserialize
        }

        public override void Serialize(BMSByte buffer)
        {
            base.Serialize(buffer);

            // Nothing to serialize
        }
    }
}
