import { resolve } from "path";

type Pointer = number;

type ActionMethod = (namePtr: Pointer, strParamPtr1: Pointer, strParamPtr2: Pointer) => void;
type FunctionMethod = (namePtr: Pointer, strParamPtr1: Pointer, strParamPtr2: Pointer) => string;
type CallbackMethod = (namePtr: Pointer, callbackPtr: Pointer) => void;
type BindMethod = ActionMethod | FunctionMethod | CallbackMethod;

type Module = {
    _malloc(size: number): Pointer;
    _free(ptr: Pointer): void;
    dynCall_vii(funcPtr: Pointer, buf1: Pointer, buf2: Pointer): void;
};

type HelperConfig = {
    isDebug: boolean;
};

type Helper = {
    Module: Module;
    lengthBytesUTF8: (str: string) => number;
    stringToUTF8: (str: string, buf: Pointer, size: number) => void;
    UTF8ToString: (ptr: Pointer) => string;
};

let isDebug: boolean;
let helper: Helper;
const boundMethods = new Map<string, BindMethod>();

declare global {
    // rome-ignore lint/style/noVar: To call from Unity's jslib
    var __getNop: (helperObj: Helper) => void;
    // rome-ignore lint/style/noVar: To call from Unity's jslib
    var __getMethod: (name: string) => BindMethod;
}

globalThis["__getNop"] = (helperObj) => {
    helper = helperObj;
    return (jsonConfigPtr: Pointer) => {
        isDebug = (JSON.parse(ptrToStr(jsonConfigPtr)) as HelperConfig).isDebug;
        console.log(`helper: isDebug=${isDebug}`);
    };
};

globalThis["__getMethod"] = (name) => {
    const method = boundMethods.get(name);
    if (!method) {
        throw new Error(`bound method not found. name=${name}`);
    }
    return method;
};

const bindMethod = (name: string, method: BindMethod) => {
    boundMethods.set(name, method);
    return;
};

const ptrToStr = (ptr: Pointer): string => helper.UTF8ToString(ptr);

const strToPtr = (str: string): Pointer => {
    const size = helper.lengthBytesUTF8(str) + 1;
    const ptr = helper.Module._malloc(size);
    helper.stringToUTF8(str, ptr, size);
    return ptr;
};

const callbackToUnity = (callbackPtr: Pointer, str1: string, str2: string): void => {
    const ptr1 = strToPtr(str1);
    const ptr2 = strToPtr(str2);
    helper.Module.dynCall_vii(callbackPtr, ptr1, ptr2);
    helper.Module._free(ptr1);
    helper.Module._free(ptr2);
};

type Action = (str1: string, str2: string) => void;
type Function = (str1: string, str2: string) => string;
type Callback = (str1: string, str2: string) => void;

const actions = new Map<string, Action>();
const functions = new Map<string, Function>();
const callbacks = new Map<string, Callback>();

const UNUSED = "";

bindMethod("CallAction", (namePtr: Pointer, strParamPtr1: Pointer, strParamPtr2: Pointer) => {
    const name = ptrToStr(namePtr);
    const action = actions.get(name);
    if (!action) {
        throw new Error(`A action to call not found. name=${name}`);
    }
    const strParam1 = strParamPtr1 ? ptrToStr(strParamPtr1) : UNUSED;
    const strParam2 = strParamPtr2 ? ptrToStr(strParamPtr2) : UNUSED;
    if (isDebug) {
        console.log(`call action: name=${name} strParam1=${strParam1} strParam2=${strParam2}`);
    }
    action(strParam1, strParam2);
});

bindMethod("CallFunction", (namePtr: Pointer, strParamPtr1: Pointer, strParamPtr2: Pointer) => {
    const name = ptrToStr(namePtr);
    const func = functions.get(name);
    if (!func) {
        throw new Error(`A function to call not found. name=${name}`);
    }
    const strParam1 = strParamPtr1 ? ptrToStr(strParamPtr1) : UNUSED;
    const strParam2 = strParamPtr2 ? ptrToStr(strParamPtr2) : UNUSED;
    if (isDebug) {
        console.log(`call function: name=${name} strParam1=${strParam1} strParam2=${strParam2}`);
    }
    return strToPtr(func(strParam1, strParam2));
});

bindMethod("AddCallback", (namePtr: Pointer, callbackPtr: Pointer) => {
    const name = ptrToStr(namePtr);
    if (isDebug) {
        console.log(`add callback: name=${name}`);
    }
    callbacks.set(name, (str1, str2) => {
        callbackToUnity(callbackPtr, str1, str2);
    });
});

const addAction = (name: string, action: Action) => actions.set(name, action);

const addFunction = (name: string, func: Function) => functions.set(name, func);

const callback = (name: string, strParam1?: string, strParam2?: string) => {
    const cb = callbacks.get(name);
    if (!cb) {
        throw new Error(`A callback to call not found. name=${name}`);
    }
    if (isDebug) {
        console.log(`call callback: name=${name} strParam1=${strParam1} strParam2=${strParam2}`);
    }
    cb(strParam1 ?? UNUSED, strParam2 ?? UNUSED);
};

const waitUntil = (condition: () => boolean, cancel: () => boolean, interval = 100) => {
    return new Promise<void>((resolve, reject) => {
        const checkCondition = () => {
            if (condition() || cancel()) {
                resolve();
            } else {
                setTimeout(checkCondition, interval);
            }
        };
        checkCondition();
    });
};

const isAsync = (func: object) => {
    return typeof func === "function" && Object.prototype.toString.call(func) === "[object AsyncFunction]";
};

export { addAction, addFunction, callback, isDebug, waitUntil, isAsync };
