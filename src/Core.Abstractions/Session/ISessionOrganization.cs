namespace Core.Session
{
    public interface ISessionOrganization
    {
        ICoreSessionContainer<System.Guid?, string> Department { get; }
        ICoreSessionContainer<System.Guid?, string> BigRegion { get; }
        ICoreSessionContainer<System.Guid?, string> Region { get; }
        ICoreSessionContainer<System.Guid?, string> Store { get; }
        ICoreSessionContainer<System.Guid?, string> Group { get; }
    }
}