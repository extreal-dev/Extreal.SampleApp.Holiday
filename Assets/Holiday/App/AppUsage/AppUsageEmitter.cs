using System;
using System.Diagnostics.CodeAnalysis;
using Extreal.Core.Common.System;
using UniRx;
using UnityEngine;

namespace Extreal.SampleApp.Holiday.App.AppUsage
{
    public class AppUsageEmitter : DisposableBase
    {
        [SuppressMessage("Usage", "CC0033")]
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public IObservable<Unit> OnFirstUsed => onFirstUsed.AddTo(disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<Unit> onFirstUsed = new Subject<Unit>();

        public IObservable<Unit> OnApplicationExiting => onApplicationExiting.AddTo(disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<Unit> onApplicationExiting = new Subject<Unit>();

        public IObservable<ErrorLog> OnErrorOccurred => onErrorOccurred.AddTo(disposables);
        [SuppressMessage("Usage", "CC0033")]
        private readonly Subject<ErrorLog> onErrorOccurred = new Subject<ErrorLog>();

        private bool isHandling;

        public void Handle()
        {
            isHandling = true;
            Application.wantsToQuit += WantsToQuit;
            Application.logMessageReceived += LogMessageReceived;
        }

        protected override void ReleaseManagedResources()
        {
            disposables.Dispose();

            if (!isHandling)
            {
                return;
            }

            Application.wantsToQuit -= WantsToQuit;
            Application.logMessageReceived -= LogMessageReceived;
        }

        private bool WantsToQuit()
        {
            onApplicationExiting.OnNext(Unit.Default);
            return true;
        }

        private void LogMessageReceived(string logString, string stackTrace, LogType type)
            => onErrorOccurred.OnNext(new ErrorLog(logString, stackTrace, type));

        public void PlayForFirstTime() => onFirstUsed.OnNext(Unit.Default);
    }
}
