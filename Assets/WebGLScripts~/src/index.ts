import { addFunction, isDebug } from "@extreal-dev/extreal.integration.web.common";
import { RedisMessagingAdapter } from "@extreal-dev/extreal.integration.messaging.redis";
import { isTouchDevice } from "./isTouchDevice";
import { VoiceChatAdapter } from "./Extreal.Integration.Chat.OME";
import { OmeAdapter } from "./Extreal.Integration.SFU.OME";

const omeAdapter = new OmeAdapter();
omeAdapter.adapt();

const voiceChatAdapter = new VoiceChatAdapter();
voiceChatAdapter.adapt(omeAdapter.getOmeClient);

const redisMessagingAdapter = new RedisMessagingAdapter();
redisMessagingAdapter.adapt();

addFunction("IsTouchDevice", () => {
    const result = isTouchDevice();
    if (isDebug) {
        console.log(`call isTouchDevice: ${result}`);
    }
    return result.toString();
});
