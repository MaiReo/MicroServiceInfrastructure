using System;

namespace Core.Utilities
{
    public class DelegateDisposabler : IDisposable
    {
        private readonly Action _onDisposing;
        private bool _disposed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="onDisposing"></param>
        public DelegateDisposabler(Action onDisposing)
        {
            _onDisposing = onDisposing;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _onDisposing?.Invoke();
                }
                _disposed = true;
            }
        }
    }
}
