using System;
using System.Linq;
using Extreal.Core.Common.System;
using UniRx;

namespace Extreal.P2P.Dev
{
    internal class NativeClientState : DisposableBase
    {
        private readonly CompositeDisposable disposables = new CompositeDisposable();

        internal IObservable<Unit> OnStarted => onStarted.AddTo(disposables);
        private readonly Subject<Unit> onStarted = new Subject<Unit>();

        private readonly BoolReactiveProperty isIceCandidateGatheringFinished = new BoolReactiveProperty(false);
        private readonly BoolReactiveProperty isOfferAnswerProcessFinished = new BoolReactiveProperty(false);

        internal NativeClientState()
        {
            isIceCandidateGatheringFinished.AddTo(disposables);
            isOfferAnswerProcessFinished.AddTo(disposables);
            Observable.CombineLatest(isIceCandidateGatheringFinished, isOfferAnswerProcessFinished)
                .Where(readies => readies.All(ready => ready))
                .Subscribe(_ => onStarted.OnNext(Unit.Default))
                .AddTo(disposables);
        }

        internal void FinishIceCandidateGathering() => isIceCandidateGatheringFinished.Value =true;
        internal void FinishOfferAnswerProcess() => isOfferAnswerProcessFinished.Value = true;

        internal void Clear()
        {
            isIceCandidateGatheringFinished.Value = false;
            isOfferAnswerProcessFinished.Value = false;
        }

        protected override void ReleaseManagedResources() => disposables.Dispose();
    }
}
