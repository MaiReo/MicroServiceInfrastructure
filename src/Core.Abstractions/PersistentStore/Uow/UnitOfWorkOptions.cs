using System.Collections.Generic;

namespace Core.PersistentStore.Uow
{
    public class UnitOfWorkOptions : IUnitOfWorkOptions
    {
        private readonly Dictionary<string, bool> _filterStatus;

        public UnitOfWorkOptions()
        {
            _filterStatus = new Dictionary<string, bool>
            {
                [DataFilters.SoftDelete] = true,
                [DataFilters.MayHaveCity] = true,
                [DataFilters.MustHaveCity] = true,
                [DataFilters.MayHaveCompany] = true,
                [DataFilters.MustHaveCompany] = true,
                [DataFilters.MayHaveStore] = false,
                [DataFilters.MustHaveStore] = false,
                [DataFilters.MayHaveStoreGroup] = true,
                [DataFilters.MustHaveStoreGroup] = true,
                [DataFilters.MustHaveBroker] = true,
            };
        }

        public void DisableFilters(params string[] names)
        {
            foreach (var name in names)
            {
                _filterStatus[name] = false;
            }
        }

        public void EnableFilters(params string[] names)
        {
            foreach (var name in names)
            {
                _filterStatus[name] = true;
            }
        }

        public IReadOnlyDictionary<string, bool> GetAllFilters() => _filterStatus;

        public bool IsFilterEnabled(string name) => _filterStatus[name];
    }
}