using Forge.Serialization;
using System.Net;

namespace Forge.Networking.Messaging.Paging
{
    public interface IMessageConstructor
    {
        int Id { get; }
        EndPoint Sender { get; }
        bool MessageReconstructed { get; }
        BMSByte MessageBuffer { get; }
        void Setup(BMSBytePool bufferPool, int id);
        void Teardown();
        void ReconstructMessagePage(BMSByte page, EndPoint sender);
    }
}
