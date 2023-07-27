import { PeerAdapter } from "@extreal-dev/extreal.integration.p2p.webrtc";
import { WebRtcAdapter } from "./NGO";
import { TextChatAdapter, VoiceChatAdapter } from "./Chat";
import { addFunction, isDebug } from "@extreal-dev/extreal.integration.web.common";
import { isTouchDevice } from "./isTouchDevice";

const peerAdapter = new PeerAdapter();
peerAdapter.adapt();

const webRtcAdapter = new WebRtcAdapter();
webRtcAdapter.adapt(peerAdapter.getPeerClient);

const textChatAdapter = new TextChatAdapter();
textChatAdapter.adapt(peerAdapter.getPeerClient);

const voiceChatAdapter = new VoiceChatAdapter();
voiceChatAdapter.adapt(peerAdapter.getPeerClient);

addFunction("IsTouchDevice", () => {
    const result = isTouchDevice();
    if (isDebug) {
        console.log(`call isTouchDevice: ${result}`);
    }
    return result.toString();
});
