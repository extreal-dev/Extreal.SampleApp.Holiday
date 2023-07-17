type OnStarted = () => void;

class ClientState {
    private readonly onStarted: OnStarted;
    private isIceCandidateGatheringFinished: boolean;
    private isOfferAnswerProcessFinished: boolean;

    constructor(onStarted: OnStarted) {
        this.onStarted = onStarted;
        this.isIceCandidateGatheringFinished = false;
        this.isOfferAnswerProcessFinished = false;
    }

    public finishIceCandidateGathering = () => {
        this.isIceCandidateGatheringFinished = true;
        this.fireOnStarted();
    };

    public finishOfferAnswerProcess = () => {
        this.isOfferAnswerProcessFinished = true;
        this.fireOnStarted();
    };

    private fireOnStarted = () => {
        if (this.isIceCandidateGatheringFinished && this.isOfferAnswerProcessFinished) {
            this.onStarted();
        }
    };

    public clear = () => {
        this.isIceCandidateGatheringFinished = false;
        this.isOfferAnswerProcessFinished = false;
    };
}

export type { OnStarted };
export { ClientState };
