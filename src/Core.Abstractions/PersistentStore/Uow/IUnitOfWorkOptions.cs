using System.Collections.Generic;

namespace Core.PersistentStore.Uow
{
    public interface IUnitOfWorkOptions
    {
        /// <summary>
        /// 默认过滤器是否开启
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool IsFilterEnabled(string name);

        /// <summary>
        /// 获取所有过滤器
        /// </summary>
        /// <returns></returns>
        IReadOnlyDictionary<string, bool> GetAllFilters();

        /// <summary>
        /// 默认禁用过滤器
        /// </summary>
        /// <param name="names"></param>
        void DisableFilters(params string[] names);

        /// <summary>
        /// 默认启用过滤器
        /// </summary>
        /// <param name="names"></param>
        void EnableFilters(params string[] names);
    }
}