using Forge.Networking.Players;
using Forge.Serialization;
using System.Net;

namespace Forge.Networking.Messaging.Paging
{
    public interface IMessageBufferInterpreter
    {
        IMessageConstructor ReconstructPacketPage(BMSByte buffer, EndPoint sender);
        void ClearBufferFor(INetPlayer player);
        void Release(IMessageConstructor constructor);
    }
}
