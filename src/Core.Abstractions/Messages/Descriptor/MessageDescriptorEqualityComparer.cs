using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messages
{
    public class MessageDescriptorEqualityComparer : IEqualityComparer<IMessageDescriptor>
    {
        public bool Equals(IMessageDescriptor x, IMessageDescriptor y)
        {
            return x?.MessageGroup == y?.MessageGroup
                && x?.MessageTopic == y?.MessageTopic;
        }

        public int GetHashCode(IMessageDescriptor obj)
        {
            if (obj == null) return 0;
            return (obj.MessageGroup + obj.MessageTopic).GetHashCode();
        }

        public static IEqualityComparer<IMessageDescriptor> Instance => new MessageDescriptorEqualityComparer();
    }
}
