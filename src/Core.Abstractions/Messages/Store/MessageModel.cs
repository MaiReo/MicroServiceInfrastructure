using System;

namespace Core.Messages.Store
{
    public struct MessageModel
    {
        public MessageModel(string typeName, string message, string hash, string group, string topic) : this()
        {
            TypeName = typeName;
            Message = message;
            Hash = hash;
            Group = group;
            Topic = topic;
        }

        public string TypeName { get; }

        public string Message { get; }

        public string Hash { get;  }

        public string Group { get; }

        public string Topic { get; }
    }
}
