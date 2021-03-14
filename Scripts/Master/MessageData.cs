using System;
using System.Collections.Generic;
using UnityEngine;

namespace Master
{
    [CreateAssetMenu(fileName = "MessageData", menuName = "MessageData", order = 0)]
    public class MessageData : ScriptableObject, IMessageData
    {
        [SerializeField]
        private int id;
        
        [SerializeField]
        private List<MessageEntity> messageList;

        public int Id => id;

        public IReadOnlyList<MessageEntity> MessageList => messageList;
    }

    [Serializable]
    public class MessageEntity
    {
        private int id;
        public int Id => id;
        
        [TextArea]
        public string message;

        public MessageEntity(int id, string message)
        {
            this.id = id;
            this.message = message;
        }
    }
}