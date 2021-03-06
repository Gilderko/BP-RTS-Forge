using Forge.Networking.Messaging;
using Forge.Networking.Players;
using Forge.Networking.Unity.Messages.Interpreters;
using Forge.Serialization;
using UnityEngine;

namespace Forge.Networking.Unity.Messages
{
    [EngineMessageContract(7, typeof(SpawnEntityMessage))]
    public class SpawnEntityMessage : ForgeMessage
    {
        public IPlayerSignature OwnerId { get; set; }
        public int Id { get; set; }
        public int PrefabId { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public override IMessageInterpreter Interpreter => SpawnEntityInterpreter.Instance;

        public override void Deserialize(BMSByte buffer)
        {
            OwnerId = ForgeSerializer.Instance.Deserialize<IPlayerSignature>(buffer);
            Id = ForgeSerializer.Instance.Deserialize<int>(buffer);
            PrefabId = ForgeSerializer.Instance.Deserialize<int>(buffer);
            Position = ForgeSerializer.Instance.Deserialize<Vector3>(buffer);
            Rotation = ForgeSerializer.Instance.Deserialize<Quaternion>(buffer);
            Scale = ForgeSerializer.Instance.Deserialize<Vector3>(buffer);
        }

        public override void Serialize(BMSByte buffer)
        {
            ForgeSerializer.Instance.Serialize(OwnerId, buffer);
            ForgeSerializer.Instance.Serialize(Id, buffer);
            ForgeSerializer.Instance.Serialize(PrefabId, buffer);
            ForgeSerializer.Instance.Serialize(Position, buffer);
            ForgeSerializer.Instance.Serialize(Rotation, buffer);
            ForgeSerializer.Instance.Serialize(Scale, buffer);
        }
    }
}