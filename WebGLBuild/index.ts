import { serve } from "https://deno.land/std@0.192.0/http/mod.ts";
import { serveDir } from "https://deno.land/std@0.192.0/http/file_server.ts";
serve(
    (req) => {
        const path = new URL(req.url).pathname;
        if (path.startsWith("/assets")) {
            return serveDir(req, { fsRoot: "./Data/" });
        }
        return serveDir(req, { fsRoot: "./Holiday/" });
    },
    { port: 3333 },
);
