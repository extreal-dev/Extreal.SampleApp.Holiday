import { PeerClientProvider } from "@extreal-dev/extreal.integration.p2p.webrtc";
import { addAction, addFunction } from "@extreal-dev/extreal.integration.web.common";
import { AppVoiceChatClient } from "./AppVoiceChatClient";

class AppVoiceChatAdapter {
    private voiceChatClient: AppVoiceChatClient | undefined;

    public adapt = (getPeerClient: PeerClientProvider) => {
        addAction(this.withPrefix("WebGLVoiceChatClient"), (jsonConfig) => {
            this.voiceChatClient = new AppVoiceChatClient(JSON.parse(jsonConfig), getPeerClient);
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

export { AppVoiceChatAdapter };
