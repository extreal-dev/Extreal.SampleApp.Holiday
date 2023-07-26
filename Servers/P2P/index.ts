import { serve } from "https://deno.land/std@0.192.0/http/server.ts";
import { createRedisAdapter, createRedisClient, Server, Socket } from "https://deno.land/x/socket_io@0.2.0/mod.ts";

type CreateHostRespone = {
    status: number;
    message: string;
};

type Host = {
    id: string;
    name: string;
};

type ListHostsResponse = {
    status: number;
    hosts: Host[];
};

type Message = {
    from: string;
    to: string;
};

const redisHost = "signaling-redis";

const isLogging = Deno.env.get("SIGNALING_LOGGING")?.toLowerCase() === "on";
const logOn = (event: string, socket: Socket) => {
    if (isLogging) {
        console.log(`on ${event}: ${socket.id}`);
    }
};
const log = (logMessage: () => string | object) => {
    if (isLogging) {
        console.log(logMessage());
    }
};

const corsConfig = {
    origin: Deno.env.get("SIGNALING_CORS_ORIGIN"),
};

const [pubClient, subClient] = await Promise.all([
    createRedisClient({
        hostname: redisHost,
    }),
    createRedisClient({
        hostname: redisHost,
    }),
]);

const io = new Server({
    cors: corsConfig,
    adapter: createRedisAdapter(pubClient, subClient),
});

const adapter = io.of("/").adapter;

const rooms = (): Map<string, Set<string>> => {
    // @ts-ignore See https://socket.io/docs/v4/rooms/#implementation-details
    return adapter.rooms;
};

io.on("connection", (socket: Socket) => {
    logOn("connection", socket);

    const setHost = (host: string): void => {
        // @ts-ignore To store additional information in the socket
        socket.host = host;
        return;
    };

    // @ts-ignore To store additional information in the socket
    const getHost = (): string => socket.host;

    socket.on("create host", (host: string, callback: (response: CreateHostRespone) => void) => {
        logOn("create host", socket);

        const wrapper = (response: CreateHostRespone) => {
            log(() => response);
            callback(response);
        };

        if (rooms().get(host)) {
            const message = `Host already exists. host: ${host}`;
            wrapper({ status: 409, message: message });
            return;
        }

        setHost(host);
        socket.join(host);

        const message = `Host have been created. host: ${host}`;
        wrapper({ status: 200, message: message });
    });

    socket.on("list hosts", (callback: (response: ListHostsResponse) => void) => {
        logOn("list hosts", socket);

        const wrapper = (response: ListHostsResponse) => {
            log(() => response);
            callback(response);
        };

        wrapper({
            status: 200,
            hosts: [...rooms().entries()]
                .filter((entry) => !entry[1].has(entry[0]))
                .map((entry) => ({ name: entry[0], id: [...entry[1]][0] })),
        });
    });

    socket.on("message", function (message: Message) {
        logOn("message", socket);
        log(() => message);

        if (!message.to) {
            return;
        }
        message.from = socket.id;
        socket.to(message.to).emit("message", message);
    });

    socket.on("disconnect", function () {
        logOn("disconnect", socket);

        const host = getHost();
        if (host) {
            socket.leave(host);
        }
        socket.broadcast.emit("user disconnected", { id: socket.id });
    });
});

await serve(io.handler(), {
    port: 3000,
});
