using System.Collections.Generic;

namespace Core.EnumClasses
{
    public interface IEnumClass
    {
        string Id { get; }

        IEnumerable<IEnumClass> GetAll();
    }
}