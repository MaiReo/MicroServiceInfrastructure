using System;
using Microsoft.Extensions.DependencyInjection;

namespace Core.KeyValues
{
    public interface IKeyValueBuilder
    {
        IServiceCollection Services { get; }

        IKeyValueBuilder AddConfiguration<T>(T configuration) where T : class, IKeyValueConfiguration;
    }
}