import { PeerClient } from "./PeerClient";
import { addAction, callback } from "../WebGL";

type PeerClientProvider = () => PeerClient;

class PeerAdapter {
    private peerClient: PeerClient | undefined;

    public adapt = () => {
        addAction(this.withPrefix("WebGLPeerClient"), (jsonPeerConfig) => {
            this.peerClient = new PeerClient(JSON.parse(jsonPeerConfig), {
                onStarted: () => callback(this.withPrefix("HandleOnStarted")),
                onConnectFailed: (reason) => callback(this.withPrefix("HandleOnConnectFailed"), reason),
                onDissconnected: (reason) => callback(this.withPrefix("HandleOnDisconnected"), reason),
            });
        });

        addAction(this.withPrefix("DoStartHostAsync"), (name) => {
            this.getPeerClient().startHost(name, (response) =>
                callback(this.withPrefix("ReceiveStartHostResponse"), JSON.stringify(response)),
            );
        });

        addAction(this.withPrefix("DoListHostsAsync"), () => {
            this.getPeerClient().listHosts((response) =>
                callback(this.withPrefix("ReceiveListHostsResponse"), JSON.stringify(response)),
            );
        });

        addAction(
            this.withPrefix("DoStartClientAsync"),
            async (hostId) => await this.getPeerClient().startClientAsync(hostId),
        );

        addAction(this.withPrefix("DoStopAsync"), () => this.getPeerClient().stop());
    };

    private withPrefix = (name: string) => `WebGLPeerClient#${name}`;

    public getPeerClient: PeerClientProvider = () => {
        if (!this.peerClient) {
            throw new Error("Call the WebGLPeerClient constructor first in Unity.");
        }
        return this.peerClient;
    };
}

export { PeerAdapter, PeerClientProvider };
