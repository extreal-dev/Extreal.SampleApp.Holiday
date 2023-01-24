using System;

namespace Extreal.SampleApp.Holiday.App.Common
{
    public abstract class DisposableBase : IDisposable
    {
        private readonly SafeDisposer safeDisposer;

        protected DisposableBase()
            => safeDisposer = new SafeDisposer(this, FreeManagedResources, FreeUnmanagedResources);

        protected virtual void FreeManagedResources() { }

        protected virtual void FreeUnmanagedResources() { }

        ~DisposableBase() => safeDisposer.DisposeByFinalizer();

        public void Dispose() => safeDisposer.Dispose();
    }
}
