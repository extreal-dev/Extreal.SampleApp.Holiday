import { PeerAdapter } from "@extreal-dev/extreal.integration.p2p.webrtc";
import { WebRtcAdapter } from "@extreal-dev/extreal.integration.multiplay.ngo.webrtc";
import { TextChatAdapter, VoiceChatAdapter } from "@extreal-dev/extreal.integration.chat.webrtc";
import { addFunction, isDebug } from "@extreal-dev/extreal.integration.web.common";
import { isTouchDevice } from "./isTouchDevice";
import { RedisMessagingAdapter } from "@extreal-dev/extreal.integration.messaging.redis";

const peerAdapter = new PeerAdapter();
peerAdapter.adapt();

const voiceChatAdapter = new VoiceChatAdapter();
voiceChatAdapter.adapt(peerAdapter.getPeerClient);

const redisMessagingAdapter = new RedisMessagingAdapter();
redisMessagingAdapter.adapt();

addFunction("IsTouchDevice", () => {
    const result = isTouchDevice();
    if (isDebug) {
        console.log(`call isTouchDevice: ${result}`);
    }
    return result.toString();
});
