import { VideoPlayerAdapter, addFunction, isDebug } from "@extreal-dev/extreal.integration.web.common";
import { SocketIOMessagingAdapter } from "@extreal-dev/extreal.integration.messaging.socket.io";
import { isTouchDevice } from "./isTouchDevice";
import { OmeAdapter } from "@extreal-dev/extreal.integration.sfu.ome";
import { VoiceChatAdapter } from "@extreal-dev/extreal.integration.chat.ome";

const videoPlayerAdapter = new VideoPlayerAdapter();
videoPlayerAdapter.adapt();

const omeAdapter = new OmeAdapter();
omeAdapter.adapt();

const voiceChatAdapter = new VoiceChatAdapter();
voiceChatAdapter.adapt(omeAdapter.getOmeClient);

const socketIOMessagingAdapter = new SocketIOMessagingAdapter();
socketIOMessagingAdapter.adapt();

addFunction("IsTouchDevice", () => {
    const result = isTouchDevice();
    if (isDebug) {
        console.log(`call isTouchDevice: ${result}`);
    }
    return result.toString();
});
