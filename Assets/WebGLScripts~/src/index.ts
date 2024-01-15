import { addFunction, isDebug } from "@extreal-dev/extreal.integration.web.common";
import { isTouchDevice } from "./isTouchDevice";
import { RedisMessagingAdapter } from "@extreal-dev/extreal.integration.messaging.redis";
import { VoiceChatAdapter } from "@extreal-dev/extreal.integration.chat.ome";

const voiceChatAdapter = new VoiceChatAdapter();
voiceChatAdapter.adapt();

const redisMessagingAdapter = new RedisMessagingAdapter();
redisMessagingAdapter.adapt();

addFunction("IsTouchDevice", () => {
    const result = isTouchDevice();
    if (isDebug) {
        console.log(`call isTouchDevice: ${result}`);
    }
    return result.toString();
});
