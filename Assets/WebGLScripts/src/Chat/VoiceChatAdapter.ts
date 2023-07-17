import { PeerClientProvider } from "../P2P";
import { addAction, addFunction } from "../WebGL";
import { VoiceChatClient } from "./VoiceChatClient";

class VoiceChatAdapter {
    private voiceChatClient: VoiceChatClient | undefined;

    public adapt = (getPeerClient: PeerClientProvider) => {
        addAction(this.withPrefix("WebGLVoiceChatClient"), (jsonConfig) => {
            this.voiceChatClient = new VoiceChatClient(JSON.parse(jsonConfig), getPeerClient);
        });

        addFunction(this.withPrefix("ToggleMute"), () => this.getVoiceChatClient().toggleMute().toString());

        addAction(this.withPrefix("Clear"), () => this.getVoiceChatClient().clear());
    };

    private withPrefix = (name: string) => `WebGLVoiceChatClient#${name}`;

    private getVoiceChatClient = () => {
        if (!this.voiceChatClient) {
            throw new Error("Call the WebGLVoiceChatClient constructor first in Unity.");
        }
        return this.voiceChatClient;
    };
}

export { VoiceChatAdapter };
