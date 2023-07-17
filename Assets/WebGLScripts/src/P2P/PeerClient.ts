import { io, Socket, SocketOptions, ManagerOptions } from "socket.io-client";
import { ClientState, OnStarted } from "./ClientState";
import { PeerRole } from "./PeerRole";
import { isAsync } from "../WebGL";

type PcCreateHook = (id: string, isOffer: boolean, pc: RTCPeerConnection) => void | Promise<void>;

type PcCloseHook = (id: string) => void;

type HandlePc = (pc: RTCPeerConnection) => void;
type HandlePcAsync = (pc: RTCPeerConnection) => Promise<void>;

type StartHostResponse = {
    status: number;
    message: string;
};

type ListHostsResponse = {
    status: number;
    hosts: Array<{ id: string; name: string }>;
};

type Message = {
    type: string | RTCSdpType;
    from?: string;
    to?: string;
    sdp?: string;
    ice?: RTCIceCandidate;
    me?: string;
};

type PeerConfig = {
    url: string;
    socketOptions: SocketOptions & ManagerOptions;
    pcConfig: RTCConfiguration;
    isDebug: boolean;
};

type PeerClientCallbacks = {
    onStarted: OnStarted;
    onConnectFailed: (reason: string) => void;
    onDissconnected: (reason: string) => void;
};

class PeerClient {
    private readonly isDebug: boolean;

    private readonly peerConfig: PeerConfig;
    private socket: Socket | null;
    private readonly pcMap: Map<string, RTCPeerConnection>;
    private readonly pcCreateHooks: Array<PcCreateHook>;
    private readonly pcCloseHooks: Array<PcCloseHook>;
    private readonly clientState: ClientState;

    private readonly callbacks: PeerClientCallbacks;

    public role: PeerRole;
    public hostId: string | null;

    constructor(peerConfig: PeerConfig, callbacks: PeerClientCallbacks) {
        this.socket = null;
        this.peerConfig = peerConfig;
        this.isDebug = peerConfig.isDebug;
        this.pcMap = new Map();
        this.pcCreateHooks = [];
        this.pcCloseHooks = [];
        this.clientState = new ClientState(callbacks.onStarted);
        this.callbacks = callbacks;
        this.role = PeerRole.None;
        this.hostId = null;
    }

    public addPcCreateHook = (hook: PcCreateHook) => {
        this.pcCreateHooks.push(hook);
    };

    public addPcCloseHook = (hook: PcCloseHook) => {
        this.pcCloseHooks.push(hook);
    };

    private getSocket = () => {
        if (this.socket !== null) {
            if (this.socket.connected) {
                return this.socket;
            }
            this.stopSocket();
        }

        const socket = io(this.peerConfig.url, this.peerConfig.socketOptions);
        this.socket = socket;

        this.socket.on("connect", () => {
            if (this.isDebug) {
                console.log(`Socket connected: id=${socket.id}`);
            }
        });
        this.socket.on("message", this.receiveMessageAsync);
        this.socket.on("user disconnected", this.receiveUserDisconnected);
        this.socket.on("connect_error", this.receiveConnectError);
        this.socket.on("disconnect", this.receiveDisconnect);

        this.socket.connect();

        return this.socket;
    };

    private receiveMessageAsync = async (message: Message) => {
        if (this.isDebug) {
            console.log(`Receive message: ${JSON.stringify(message)}`);
        }

        const from = this.getFrom(message);
        switch (message.type) {
            case "join": {
                await this.receiveJoinAsync(from);
                break;
            }
            case "call me": {
                await this.sendOfferAsync(this.getMe(message));
                break;
            }
            case "offer": {
                await this.receiveOfferAsync(from, message as RTCSessionDescriptionInit);
                break;
            }
            case "answer": {
                await this.receiveAnswerAsync(from, message as RTCSessionDescriptionInit);
                break;
            }
            case "done": {
                this.receiveDone(from);
                break;
            }
            case "candidate": {
                this.receiveCandidate(from, new RTCIceCandidate(message.ice));
                break;
            }
            case "bye": {
                this.receiveBye(from);
                break;
            }
            default: {
                if (this.isDebug) {
                    console.log(`Unknown message received!!! type=${message.type}`);
                }
                break;
            }
        }
    };

    private getFrom = (message: Message) => {
        if (!message.from) {
            throw new Error("Not occurring because from is set on the server side.");
        }
        return message.from;
    };

    private getMe = (message: Message) => {
        if (!message.me) {
            throw new Error("Not occurring because me is set on the caller.");
        }
        return message.me;
    };

    private receiveUserDisconnected = (event: { id: string }) => {
        if (this.isDebug) {
            console.log(`Receive user disconnected: ${event}`);
        }
        this.closePc(event.id);
    };

    private receiveConnectError = (error: Error) => {
        if (this.isDebug) {
            console.log(error.message);
        }
        this.callbacks.onConnectFailed(error.message);
    };

    private receiveDisconnect = (reason: string) => {
        if (this.isDebug) {
            console.log(reason);
        }
        this.callbacks.onDissconnected(reason);
    };

    public startHost = (name: string, handle: (response: StartHostResponse) => void) => {
        this.role = PeerRole.Host;
        this.getSocket().emit("create host", name, (response: StartHostResponse) => {
            if (this.isDebug) {
                console.log(response);
            }
            handle(response);
        });
    };

    public listHosts = (handle: (response: ListHostsResponse) => void) => {
        this.getSocket().emit("list hosts", (response: ListHostsResponse) => {
            if (this.isDebug) {
                console.log(response);
            }
            handle(response);
        });
    };

    public startClientAsync = async (hostId: string) => {
        this.role = PeerRole.Client;
        this.hostId = hostId;
        this.sendMessage(hostId, { type: "join" });
    };

    private sendOfferAsync = async (to: string) => {
        if (this.pcMap.has(to)) {
            if (this.isDebug) {
                console.log(`Send offer: Not sent as it already exists. to=${to}`);
            }
            return;
        }

        await this.createPcAsync(to, true);

        await this.handlePcAsync("sendOffer", to, async (pc: RTCPeerConnection) => {
            const sd = await pc.createOffer();
            await pc.setLocalDescription(sd);
            this.sendSdp(to, pc.localDescription as RTCSessionDescription);
        });
    };

    public stop = () => {
        this.role = PeerRole.None;
        this.hostId = null;

        for (const id of this.pcMap.keys()) {
            this.sendMessage(id, { type: "bye" });
            this.closePc(id);
        }

        this.pcMap.clear();
        this.clientState.clear();
        this.stopSocket();
    };

    private stopSocket = () => {
        if (this.socket === null) {
            return;
        }
        this.socket.close();
        this.socket = null;
    };

    private createPcAsync = async (id: string, isOffer: boolean) => {
        if (this.pcMap.has(id)) {
            return;
        }

        const pc = new RTCPeerConnection(this.peerConfig.pcConfig);

        pc.onicecandidate = (event) => {
            if (!event.candidate) {
                return;
            }
            if (this.isDebug) {
                console.log(`Receive ice candidate: state=${event.candidate} id=${id}`);
            }
            this.sendIce(id, event.candidate);
        };

        pc.oniceconnectionstatechange = () => {
            if (this.isDebug) {
                console.log(`Receive ice connection change: state=${pc.iceConnectionState} id=${id}`);
            }
            switch (pc.iceConnectionState) {
                case "new":
                case "checking":
                case "disconnected": {
                    // do nothing
                    break;
                }
                case "connected":
                case "completed": {
                    if (this.role === PeerRole.Client) {
                        this.clientState.finishIceCandidateGathering();
                    }
                    break;
                }
                case "failed":
                case "closed": {
                    this.closePc(id);
                    break;
                }
            }
        };

        for (const hook of this.pcCreateHooks) {
            if (isAsync(hook)) {
                await hook(id, isOffer, pc);
            } else {
                hook(id, isOffer, pc);
            }
        }
        this.pcMap.set(id, pc);
    };

    private closePc = (from: string) => {
        this.handlePc("closePc", from, (pc: RTCPeerConnection) => {
            this.pcCloseHooks.forEach((hook) => hook(from));
            pc.close();
            this.pcMap.delete(from);
        });
    };

    private sendSdp = (to: string, sd: RTCSessionDescription) => {
        this.sendMessage(to, { type: sd.type, sdp: sd.sdp });
    };

    private sendIce = (to: string, candidate: RTCIceCandidate) => {
        this.handlePc("sendIce", to, () => this.sendMessage(to, { type: "candidate", ice: candidate }));
    };

    private sendMessage = (to: string, message: Message) => {
        message.to = to;
        if (this.isDebug) {
            console.log(`Send message: ${JSON.stringify(message)}`);
        }
        this.getSocket().emit("message", message);
    };

    private receiveJoinAsync = async (from: string) => {
        await this.sendOfferAsync(from);
        for (const to of this.pcMap.keys()) {
            if (from === to) {
                continue;
            }
            this.sendMessage(to, { type: "call me", me: from });
        }
    };

    private receiveOfferAsync = async (from: string, sd: RTCSessionDescriptionInit) => {
        await this.createPcAsync(from, false);

        await this.handlePcAsync("receiveOfferAsync", from, async (pc: RTCPeerConnection) => {
            await pc.setRemoteDescription(sd);
            await this.sendAnswerAsync(from);
        });
    };

    private sendAnswerAsync = async (from: string) => {
        await this.handlePcAsync("sendAnswerAsync", from, async (pc: RTCPeerConnection) => {
            const sd = await pc.createAnswer();
            await pc.setLocalDescription(sd);
            this.sendSdp(from, pc.localDescription as RTCSessionDescription);
        });
    };

    private receiveAnswerAsync = async (from: string, sd: RTCSessionDescriptionInit) => {
        await this.handlePcAsync("receiveAnswerAsync", from, async (pc: RTCPeerConnection) => {
            await pc.setRemoteDescription(sd);
            this.sendMessage(from, { type: "done" });
        });
    };

    private receiveDone = (from: string) => {
        if (this.role === PeerRole.Client && from === this.hostId) {
            this.clientState.finishOfferAnswerProcess();
        }
    };

    private receiveCandidate = (from: string, candidate: RTCIceCandidate) => {
        this.handlePc("receiveCandidate", from, (pc: RTCPeerConnection) => pc.addIceCandidate(candidate));
    };

    private receiveBye = (from: string) => this.closePc(from);

    private handlePc = (funcName: string, id: string, handle: HandlePc) => {
        const pc = this.pcMap.get(id);
        if (!pc) {
            return;
        }
        try {
            handle(pc);
        } catch (e) {
            this.logError(funcName, e);
        }
    };

    private handlePcAsync = async (funcName: string, id: string, handle: HandlePcAsync) => {
        const pc = this.pcMap.get(id);
        if (!pc) {
            return;
        }
        try {
            await handle(pc);
        } catch (e) {
            this.logError(funcName, e);
        }
    };

    private logError = (funcName: string, e: unknown) => {
        if (this.isDebug) {
            console.error(`Error has occurred at ${funcName}`, e);
        }
    };
}

export { PeerClient };
