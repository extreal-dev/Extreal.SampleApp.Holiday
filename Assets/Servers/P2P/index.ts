import { serve } from "https://deno.land/std@0.192.0/http/server.ts";
import {
  createRedisAdapter,
  createRedisClient,
  Server,
  Socket,
} from "https://deno.land/x/socket_io@0.2.0/mod.ts";

const redisHost = "signaling-redis";

const [pubClient, subClient] = await Promise.all([
  createRedisClient({
    hostname: redisHost,
  }),
  createRedisClient({
    hostname: redisHost,
  }),
]);

const io = new Server({
  cors: {
    origin: "*",
  },
  adapter: createRedisAdapter(pubClient, subClient),
});

const adapter = io.of("/").adapter;

const rooms = (): Map<string, Set<string>> => {
  // @ts-ignore See https://socket.io/docs/v4/rooms/#implementation-details
  return adapter.rooms;
};

io.on("connection", (socket: Socket) => {
  // @ts-ignore To store additional information in the socket
  const setHost = (host: string): void => socket.host = host;

  // @ts-ignore To store additional information in the socket
  const getHost = (): string => socket.host;

  socket.on("create host", (host, callback) => {
    if (rooms().get(host)) {
      const message = "Host already exists. host: " + host;
      callback({ status: 409, message: message });
      return;
    }

    setHost(host);
    socket.join(host);

    const message = `Host have been created. host: ${host}`;
    callback({ status: 200, message: message });
  });

  socket.on("list hosts", (callback) => {
    callback({
      status: 200,
      hosts: [...rooms().entries()]
        .filter((entry) => !entry[1].has(entry[0]))
        .map((entry) => ({ name: entry[0], id: [...entry[1]][0] })),
    });
  });

  socket.on("message", function (message) {
    if (!message.to) {
      return;
    }
    message.from = socket.id;
    socket.to(message.to).emit("message", message);
  });

  socket.on("disconnect", function () {
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
