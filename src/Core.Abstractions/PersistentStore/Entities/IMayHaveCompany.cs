using System;
using System.Collections.Generic;
using System.Text;

namespace Core.PersistentStore
{
    public interface IMayHaveCompany : IMayHaveCompanyId
    {
        string CompanyName { get; set; }
    }
}
