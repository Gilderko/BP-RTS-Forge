using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
    [EngineMessageContract(38, typeof(DespawnEntityMessage))]
    public class DespawnEntityMessage : ForgeMessage
    {
        public int EntityId { get; set; }
        public override IMessageInterpreter Interpreter => DespawnEntityInterpreter.Instance;

        public override void Deserialize(BMSByte buffer)
        {
            EntityId = ForgeSerializer.Instance.Deserialize<int>(buffer);
        }

        public override void Serialize(BMSByte buffer)
        {
            ForgeSerializer.Instance.Serialize(EntityId, buffer);
        }
    }
}
