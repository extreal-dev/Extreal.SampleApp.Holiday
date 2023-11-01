import { PeerAdapter } from "@extreal-dev/extreal.integration.p2p.webrtc";
import { WebRtcAdapter } from "@extreal-dev/extreal.integration.multiplay.ngo.webrtc";
import { TextChatAdapter } from "@extreal-dev/extreal.integration.chat.webrtc";
import { addFunction, isDebug } from "@extreal-dev/extreal.integration.web.common";
import { isTouchDevice } from "./isTouchDevice";
import { AppVoiceChatAdapter } from "./AppVoiceChatAdapter";
import { hasMic } from "./hasMic";

const peerAdapter = new PeerAdapter();
peerAdapter.adapt();

const webRtcAdapter = new WebRtcAdapter();
webRtcAdapter.adapt(peerAdapter.getPeerClient);

const textChatAdapter = new TextChatAdapter();
textChatAdapter.adapt(peerAdapter.getPeerClient);

const voiceChatAdapter = new AppVoiceChatAdapter();
voiceChatAdapter.adapt(peerAdapter.getPeerClient);

addFunction("IsTouchDevice", () => {
    const result = isTouchDevice();
    if (isDebug) {
        console.log(`call isTouchDevice: ${result}`);
    }
    return result.toString();
});

addFunction("HasMic", () => {
    const result = hasMic();
    if (isDebug) {
        console.log(`call hasMic: ${result}`);
    }
    return result.toString();
});
