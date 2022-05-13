using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
    [EngineMessageContract(23, typeof(SetNewTargetMessage))]
    public class SetNewTargetMessage : ForgeMessage
    {
        public int EntityId { get; set; }
        public int TargetEntityId { get; set; }

        public override IMessageInterpreter Interpreter => SetNewTargetInterpreter.Instance;

        public override void Deserialize(BMSByte buffer)
        {
            EntityId = ForgeSerializer.Instance.Deserialize<int>(buffer);
            TargetEntityId = ForgeSerializer.Instance.Deserialize<int>(buffer);
        }

        public override void Serialize(BMSByte buffer)
        {


            ForgeSerializer.Instance.Serialize(EntityId, buffer);
            ForgeSerializer.Instance.Serialize(TargetEntityId, buffer);
        }
    }
}
