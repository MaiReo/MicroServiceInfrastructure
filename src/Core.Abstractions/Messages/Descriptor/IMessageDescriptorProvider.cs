using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messages
{
    public interface IMessageDescriptorProvider
    {
        IMessageDescriptor GetMessageDescriptor(string defaultGroup, string defaultTopic);
    }
}
