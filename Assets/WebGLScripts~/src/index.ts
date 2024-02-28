import { addFunction, isDebug } from "@extreal-dev/extreal.integration.web.common";
import { SocketIOMessagingAdapter } from "@extreal-dev/extreal.integration.messaging.socket.io";
import { isTouchDevice } from "./isTouchDevice";
import { VoiceChatAdapter } from "./Extreal.Integration.Chat.OME";
import { OmeAdapter } from "./Extreal.Integration.SFU.OME";

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
