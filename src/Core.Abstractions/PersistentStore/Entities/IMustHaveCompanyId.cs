using System;
using System.Collections.Generic;
using System.Text;

namespace Core.PersistentStore
{
    public interface IMustHaveCompanyId
    {
        Guid CompanyId { get; set; }
    }
}
