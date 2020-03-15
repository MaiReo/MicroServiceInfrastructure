using Core.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Core.PersistentStore.Uow
{
    public class UnitOfWorkBase : ICurrentUnitOfWork, IUnitOfWork
    {
        private readonly ConcurrentDictionary<string, bool> _filterStatus;

        public UnitOfWorkBase(IUnitOfWorkOptions unitOfWorkOptions)
        {
            _filterStatus = new ConcurrentDictionary<string, bool>(unitOfWorkOptions.GetAllFilters());
        }

        public IDisposable DisableFilters(params string[] names)
        {
            if (names is null || names.Length == 0)
            {
                return new DelegateDisposabler(null);
            }
            var disabledNames = new List<string>();
            foreach (var name in names)
            {
                if (DisableFilterByName(name))
                {
                    disabledNames.Add(name);
                }
            }
            if (disabledNames.Count == 0)
            {
                return new DelegateDisposabler(null);
                
            }
            return new DelegateDisposabler(() => 
            {
                foreach (var name in disabledNames)
                {
                    EnableFilterByName(name);
                }
            });
        }

        private bool EnableFilterByName(string name)
        {
            if (_filterStatus.TryGetValue(name, out var enabled) && enabled)
            {
                return false;
            }
            return _filterStatus.TryUpdate(name, true, false);

        }
        private bool DisableFilterByName(string name)
        {
            if (_filterStatus.TryGetValue(name, out var enabled) && !enabled)
            {
                return false;
            }
            return _filterStatus.TryUpdate(name, false, true);
        }

        public IDisposable EnableFilters(params string[] names)
        {
            if (names is null || names.Length == 0)
            {
                return new DelegateDisposabler(null);
            }
            var enabledNames = new List<string>();
            foreach (var name in names)
            {
                if (EnableFilterByName(name))
                {
                    enabledNames.Add(name);
                }
            }
            if (enabledNames.Count == 0)
            {
                return new DelegateDisposabler(null);

            }
            return new DelegateDisposabler(() =>
            {
                foreach (var name in enabledNames)
                {
                    DisableFilterByName(name);
                }
            });
        }

        public bool IsFilterEnabled(string name)
        {
            return _filterStatus.TryGetValue(name, out var enabled) && enabled;
        }
    }
}
