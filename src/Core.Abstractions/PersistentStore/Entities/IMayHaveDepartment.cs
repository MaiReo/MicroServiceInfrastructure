namespace Core.PersistentStore
{
    public interface IMayHaveDepartment : IMayHaveDepartmentId
    {
        string DepartmentName { get; set; }
    }

}
