using Core.Session;
using System;

namespace Core.TestBase
{

    public class UnitTestCoreSessionProvider : ICoreSessionProvider
    {
        private readonly UnitTestCurrentUser _currentUser;

        public ICoreSession Session { get; private set; }

        public UnitTestCoreSessionProvider(UnitTestCurrentUser currentUser)
        {
            _currentUser = currentUser;
            Session = new UnitTestCoreSession(null, null, _currentUser);
        }

        public IDisposable Use(
            string cityId,
            Guid? companyId,
            string companyName,
            Guid? groupId,
            string groupName,
            string brokerId,
            string brokerName)
        {
            lock (this)
            {
                var currentSession = Session;
                var newSession = new UnitTestCoreSession(cityId,
                    CoreSessionContainer.Create(companyId, companyName),
                    _currentUser,
                    new SessionOrganization(
                        default, default, default, default, default, default, default, default, groupId, groupName
                    ),
                    CoreSessionContainer.Create(brokerId, brokerName)
                );
                var disposable = new SessionRestore(() => Restore(currentSession));
                Session = newSession;
                return disposable;
            }
        }

        private void Restore(ICoreSession session)
        {
            lock (this)
            {
                Session = session;
            }
        }

        private class SessionRestore : IDisposable
        {

            #region IDisposable Support
            private bool disposedValue = false; // 要检测冗余调用
            private readonly Action _onDisposing;

            public SessionRestore(Action onDisposing)
            {
                _onDisposing = onDisposing;
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: 释放托管状态(托管对象)。
                        _onDisposing?.Invoke();
                    }

                    // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                    // TODO: 将大型字段设置为 null。

                    disposedValue = true;
                }
            }

            // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
            // ~SessionRestore() {
            //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            //   Dispose(false);
            // }

            // 添加此代码以正确实现可处置模式。
            public void Dispose()
            {
                // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
                Dispose(true);
                // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
                // GC.SuppressFinalize(this);
            }
            #endregion

        }
    }
}
