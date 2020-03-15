using System;

namespace Core.PersistentStore.Uow
{
    public interface IUnitOfWork
    {
        bool IsFilterEnabled(string name);
        IDisposable DisableFilters(params string[] names);
        IDisposable EnableFilters(params string[] names);
    }
}