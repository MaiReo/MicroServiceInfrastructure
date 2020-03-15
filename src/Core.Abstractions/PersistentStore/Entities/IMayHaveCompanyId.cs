using System;
using System.Collections.Generic;
using System.Text;

namespace Core.PersistentStore
{
    public interface IMayHaveCompanyId
    {
        Guid? CompanyId { get; set; }
    }
}
