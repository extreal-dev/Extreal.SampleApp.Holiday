let result = true;

(async () => {
    try {
        const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
        stream.getTracks().forEach((track) => track.stop());
        result = true;
    } catch (e) {
        result = false;
    }
})();

const hasMic = () => result;

export { hasMic };
