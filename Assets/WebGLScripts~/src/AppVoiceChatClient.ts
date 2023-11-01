﻿import { PeerClientProvider } from "@extreal-dev/extreal.integration.p2p.webrtc";

type VoiceChatConfig = {
    initialMute: boolean;
    isDebug: boolean;
};

class AppVoiceChatClient {
    private readonly isDebug: boolean;
    private readonly voiceChatConfig: VoiceChatConfig;
    private readonly getPeerClient: PeerClientProvider;

    private inStreams: Map<string, MediaStream>;
    private inTracks: Map<string, MediaStreamTrack>;
    private outAudios: Map<string, HTMLAudioElement>;
    private outStreams: Map<string, MediaStream>;

    private mute: boolean;

    constructor(voiceChatConfig: VoiceChatConfig, getPeerClient: PeerClientProvider) {
        this.isDebug = voiceChatConfig.isDebug;
        this.voiceChatConfig = voiceChatConfig;
        this.mute = voiceChatConfig.initialMute;
        this.getPeerClient = getPeerClient;
        this.inStreams = new Map();
        this.inTracks = new Map();
        this.outAudios = new Map();
        this.outStreams = new Map();
        this.getPeerClient().addPcCreateHook(this.createPc);
        this.getPeerClient().addPcCloseHook(this.closePc);
    }

    private createPc = async (id: string, isOffer: boolean, pc: RTCPeerConnection) => {
        if (this.outAudios.has(id)) {
            return;
        }

        if (this.isDebug) {
            console.log(`New MediaStream: id=${id}`);
        }

        const client = this;

        try {
            const inStream = await navigator.mediaDevices.getUserMedia({ audio: true });
            client.inStreams.set(id, inStream);
            const inTrack = inStream.getAudioTracks()[0];
            client.inTracks.set(id, inTrack);

            pc.addTrack(inTrack, inStream);
            inTrack.enabled = !this.mute;

            const outAudio = new Audio();
            client.outAudios.set(id, outAudio);

            pc.addEventListener("track", async (event) => {
                const outStream = event.streams[0];
                outAudio.srcObject = outStream;
                outAudio.loop = true;
                await outAudio.play();
                client.outStreams.set(id, outStream);
            });
        } catch (e) {
            if (this.isDebug) {
                console.error(e);
            }
        }
    };

    private closePc = (id: string) => {
        const outAudio = this.outAudios.get(id);
        if (outAudio) {
            outAudio.pause();
            this.outAudios.delete(id);
        }
        const outStream = this.outStreams.get(id);
        if (outStream) {
            outStream.getTracks().forEach((track) => track.stop());
            this.outStreams.delete(id);
        }
    };

    public clear = () => {
        [...this.inStreams.values()].forEach((stream) => {
            stream.getTracks().forEach((track) => track.stop());
        });
        this.inStreams.clear();
        this.inTracks.clear();
        [...this.outAudios.keys()].forEach(this.closePc);
        this.outAudios.clear();
        this.outStreams.clear();
        this.mute = this.voiceChatConfig.initialMute;
    };

    public toggleMute = () => {
        this.mute = !this.mute;
        [...this.inTracks.values()].forEach((track) => {
            track.enabled = !this.mute;
        });
        return this.mute;
    };
}

export { AppVoiceChatClient };
