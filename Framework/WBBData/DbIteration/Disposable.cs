using System;

namespace WBBData.DbIteration
{
    public class Disposable : IDisposable
    {
        private bool isDisposed;

        ~Disposable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!isDisposed && disposing)
            {
                DisposeContext();
            }

            isDisposed = true;
        }

        protected virtual void DisposeContext()
        {
        }
    }
}