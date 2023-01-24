using System;

namespace Extreal.SampleApp.Holiday.App.Common
{
    public class SafeDisposer
    {
        private bool isDisposed;

        private readonly object target;
        private readonly Action freeManagedResources;
        private readonly Action freeUnmanagedResources;

        public SafeDisposer(object target, Action freeManagedResources = null, Action freeUnmanagedResources = null)
        {
            this.target = target;
            this.freeManagedResources = freeManagedResources;
            this.freeUnmanagedResources = freeUnmanagedResources;
            if (freeManagedResources == null && freeUnmanagedResources == null)
            {
                throw new ArgumentException(
                    $"Either {nameof(freeManagedResources)} or {nameof(freeUnmanagedResources)} is required");
            }
        }

        public void DisposeByFinalizer() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(target);
        }

        private void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                freeManagedResources?.Invoke();
            }

            freeUnmanagedResources?.Invoke();
            isDisposed = true;
        }
    }
}
