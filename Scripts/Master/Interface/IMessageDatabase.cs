using System.Collections.Generic;

namespace Master
{
    public interface IMessageDatabase
    {
        IEnumerable<IMessageData> All();

        IMessageData FindById(int id);
    }
}