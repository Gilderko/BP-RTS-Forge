using Forge.Networking.Messaging;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Unity.Messages
{
    [EngineMessageContract(22, typeof(SpawnPlayerObjectMessage))]
    public class SpawnPlayerObjectMessage : SpawnEntityMessage
    {
        public string PlayerName { get; set; }
        public bool IsTeamOwner { get; set; }
        public float Red { get; set; }
        public float Green { get; set; }
        public float Blue { get; set; }
        public int MoneyStart { get; set; }

        public override IMessageInterpreter Interpreter => SpawnPlayerObjectInterpreter.Instance;

        public override void Deserialize(BMSByte buffer)
        {
            base.Deserialize(buffer);

            PlayerName = ForgeSerializer.Instance.Deserialize<string>(buffer);
            IsTeamOwner = ForgeSerializer.Instance.Deserialize<bool>(buffer);
            Red = ForgeSerializer.Instance.Deserialize<float>(buffer);
            Green = ForgeSerializer.Instance.Deserialize<float>(buffer);
            Blue = ForgeSerializer.Instance.Deserialize<float>(buffer);
            MoneyStart = ForgeSerializer.Instance.Deserialize<int>(buffer);
        }

        public override void Serialize(BMSByte buffer)
        {
            base.Serialize(buffer);

            ForgeSerializer.Instance.Serialize(PlayerName, buffer);
            ForgeSerializer.Instance.Serialize(IsTeamOwner, buffer);
            ForgeSerializer.Instance.Serialize(Red, buffer);
            ForgeSerializer.Instance.Serialize(Green, buffer);
            ForgeSerializer.Instance.Serialize(Blue, buffer);
            ForgeSerializer.Instance.Serialize(MoneyStart, buffer);
        }
    }
}
