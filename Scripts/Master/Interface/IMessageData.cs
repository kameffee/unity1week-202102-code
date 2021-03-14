using System.Collections.Generic;

namespace Master
{
    public interface IMessageData
    {
        int Id { get; }

        IReadOnlyList<MessageEntity> MessageList { get; }
    }
}