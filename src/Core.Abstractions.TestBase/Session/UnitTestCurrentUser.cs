using Core.Session;

namespace Core.TestBase
{
    public class UnitTestCurrentUser : ICoreSessionContainer<string, string>
    {
        public UnitTestCurrentUser()
        {

        }

        public string Id { get; private set; }

        public string Name { get; private set; }
        
        public void Set(string id, string name)
        {
            lock (this)
            {
                Id = id;
                Name = name;
            }
        }

        public static UnitTestCurrentUser Current { get; }

        static UnitTestCurrentUser()
        {
            Current = new UnitTestCurrentUser();
        }
    }
}
