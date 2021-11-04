using Forge.Serialization;

namespace Forge.Networking.Messaging
{
    public abstract class ForgeMessage : IMessage
    {
        public event MessageSent OnMessageSent;
        public IMessageReceiptSignature Receipt { get; set; }
        public abstract IMessageInterpreter Interpreter { get; }
        public long MessageInstanceId { get; set; }

        public virtual void Serialize(BMSByte buffer)
        {
            ForgeSerializer.Instance.Serialize(MessageInstanceId, buffer);
        }

        public virtual void Deserialize(BMSByte buffer)
        {
            MessageInstanceId = ForgeSerializer.Instance.Deserialize<long>(buffer);
        }

        public void Sent()
        {
            OnMessageSent?.Invoke(this);
        }
    }
}
