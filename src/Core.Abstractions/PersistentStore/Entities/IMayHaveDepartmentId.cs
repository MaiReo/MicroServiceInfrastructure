using System;

namespace Core.PersistentStore
{
    public interface IMayHaveDepartmentId
    {
        Guid? DepartmentId { get; set; }
    }

}
