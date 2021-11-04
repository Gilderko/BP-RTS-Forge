﻿using Forge.Networking.Messaging.Interpreters;
using Forge.Serialization;

namespace Forge.Networking.Messaging.Messages
{
    [MessageContract(7, typeof(ForgeReceiptAcknowledgementMessage))]
    public class ForgeReceiptAcknowledgementMessage : ForgeMessage
    {
        public IMessageReceiptSignature ReceiptSignature { get; set; }
        public override IMessageInterpreter Interpreter => new ForgeReceiptAcknowledgementInterpreter();

        public override void Deserialize(BMSByte buffer)
        {
            base.Deserialize(buffer);

            ReceiptSignature = ForgeSerializer.Instance.Deserialize<IMessageReceiptSignature>(buffer);
        }

        public override void Serialize(BMSByte buffer)
        {
            base.Serialize(buffer);

            ForgeSerializer.Instance.Serialize(ReceiptSignature, buffer);
        }
    }
}
