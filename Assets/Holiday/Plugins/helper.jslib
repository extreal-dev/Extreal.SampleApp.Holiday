const plugin = {};

const helperNames = ["lengthBytesUTF8", "stringToUTF8", "UTF8ToString", "Module"];
const helper = "{" + helperNames.map(func => `${func}:${func}`).join(",") + "}";
plugin["Nop"] = function () {};
plugin["Nop__postset"] = `_Nop = __getNop(${helper})`;

const methods = ["CallAction", "CallFunction", "AddCallback"];
for (let i = 0; i < methods.length; i++) {
    const method = methods[i];
    plugin[method] = function () {};
    plugin[method + "__postset"] = `_${method} = __getMethod("${method}")`;
}

mergeInto(LibraryManager.library, plugin);
