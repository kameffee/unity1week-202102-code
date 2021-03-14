using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Master
{
    public class MessageDatabase : IMessageDatabase
    {
        private  readonly MessageData[] dataCache;

        public MessageDatabase()
        {
            dataCache = Resources.LoadAll<MessageData>("Master/");
        }

        public IEnumerable<IMessageData> All() => dataCache;

        public IMessageData FindById(int id)
        {
            return dataCache.First(data => data.Id == id);
        }
    }
}