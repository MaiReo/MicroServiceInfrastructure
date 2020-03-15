using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Session
{
    public interface ICoreSessionProvider
    {
        ICoreSession Session { get; }
    }
}
