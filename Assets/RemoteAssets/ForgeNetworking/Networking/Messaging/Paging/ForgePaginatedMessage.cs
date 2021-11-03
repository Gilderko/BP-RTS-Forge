using Forge.Serialization;
using System.Collections.Generic;

namespace Forge.Networking.Messaging.Paging
{
    public class ForgePaginatedMessage : IPaginatedMessage
    {
        public BMSByte Buffer { get; set; }
        public int TotalSize => Buffer.Size;
        public List<IMessagePage> Pages => _pages;

        private readonly List<IMessagePage> _pages = new List<IMessagePage>();
    }
}
