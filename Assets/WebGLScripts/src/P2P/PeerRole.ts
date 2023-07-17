const PeerRole = {
    None: 0,
    Host: 1,
    Client: 2,
} as const;

type PeerRole = typeof PeerRole[keyof typeof PeerRole];

export { PeerRole };
