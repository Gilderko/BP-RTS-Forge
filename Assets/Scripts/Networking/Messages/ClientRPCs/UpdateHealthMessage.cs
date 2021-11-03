using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
    [EngineMessageContract(31, typeof(UpdateHealthMessage))]
    public class UpdateHealthMessage : ForgeMessage
    {
        public int EntityID { get; set; }
        public int NewHealthValue { get; set; }

        public override IMessageInterpreter Interpreter => UpdateHealthInterpreter.Instance;

        public override void Deserialize(BMSByte buffer)
        {
            EntityID = ForgeSerializer.Instance.Deserialize<int>(buffer);
            NewHealthValue = ForgeSerializer.Instance.Deserialize<int>(buffer);
        }

        public override void Serialize(BMSByte buffer)
        {
            ForgeSerializer.Instance.Serialize(EntityID, buffer);
            ForgeSerializer.Instance.Serialize(NewHealthValue, buffer);
        }
    }
}
