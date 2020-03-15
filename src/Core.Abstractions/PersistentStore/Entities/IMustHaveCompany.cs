using System;
using System.Collections.Generic;
using System.Text;

namespace Core.PersistentStore
{
    public interface IMustHaveCompany : IMustHaveCompanyId
    {
        string CompanyName { get; set; }
    }
}
