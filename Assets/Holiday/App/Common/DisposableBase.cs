using System;

namespace Extreal.SampleApp.Holiday.App.Common
{
    public abstract class DisposableBase : IDisposable
    {
        private readonly SafeDisposer safeDisposer;

        protected DisposableBase(Action freeManagedResources = null, Action freeUnmanagedResources = null)
            => safeDisposer = new SafeDisposer(this, freeManagedResources, freeUnmanagedResources);

        ~DisposableBase() => safeDisposer.DisposeByFinalizer();

        public void Dispose() => safeDisposer.Dispose();
    }
}
