using Microsoft.Extensions.DependencyInjection;

namespace Core.KeyValues
{
    internal class KeyValueBuilder : IKeyValueBuilder
    {
        public KeyValueBuilder(IServiceCollection services) => Services = services;
        public IServiceCollection Services { get; }

        public IKeyValueBuilder AddConfiguration<T>(T configuration) where T : class, IKeyValueConfiguration
        {
            Services.AddSingleton<IKeyValueConfiguration, T>(sp => configuration);
            return this;
        }
    }
}