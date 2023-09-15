import typescript from "@rollup/plugin-typescript";
import terser from "@rollup/plugin-terser";
import { nodeResolve } from "@rollup/plugin-node-resolve";
import { RollupOptions } from "rollup";

const isProd = process.env.BUILD === "production";

const config: RollupOptions = {
    input: "src/index.ts",
    output: {
        file: "../WebGLTemplates/Holiday/index.js",
        format: "iife",
        plugins: isProd ? [terser()] : [],
        globals: {},
    },
    plugins: [
        typescript(),
        nodeResolve({
            browser: true,
        }),
    ],
};

export default config;
