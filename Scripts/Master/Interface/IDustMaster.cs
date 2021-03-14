using System.Collections.Generic;

namespace Master
{
    public interface IDustMaster
    {
        IEnumerable<DustEntity> All();
        
        DustEntity FindById(int id);
    }
}