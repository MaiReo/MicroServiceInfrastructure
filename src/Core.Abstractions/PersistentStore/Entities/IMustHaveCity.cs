using System.ComponentModel.DataAnnotations;

namespace Core.PersistentStore
{
    public interface IMustHaveCity
    {
        string CityId { get; set; }
    }
}
