using System;
using System.Collections.Generic;
using System.Text;

namespace Core.PersistentStore
{
    public class DataFilters
    {
        public const string SoftDelete = "SoftDelete";
        public const string MayHaveCity = "MayHaveCity";
        public const string MustHaveCity = "MustHaveCity";
        public const string MayHaveCompany = "MayHaveCompany";
        public const string MustHaveCompany = "MustHaveCompany";
        public const string MayHaveStore = "MaytHaveStore";
        public const string MustHaveStore = "MustHaveStore";
        public const string MayHaveStoreGroup = "MaytHaveStoreGroup";
        public const string MustHaveStoreGroup = "MustHaveStoreGroup";
        public const string MustHaveBroker = "MustHaveBroker";
    }
}
