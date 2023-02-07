import { ReactNode } from 'react';
import { Client, defaultExchanges, subscriptionExchange, Provider } from 'urql';
import { createClient as createWSClient } from 'graphql-ws';

const url = new URL('/graphql', globalThis.window.location.href);
const wsUrl = new URL('/graphql', globalThis.window.location.href);
wsUrl.protocol = "ws:";

const wsClient = createWSClient({
  url: wsUrl.toString(),
});

const client = new Client({
  url: url.toString(),
  maskTypename: true,
  exchanges: [
    ...defaultExchanges,
    subscriptionExchange({
      forwardSubscription: (operation) => ({
        subscribe: (sink) => ({
          unsubscribe: wsClient.subscribe(operation, sink),
        }),
      }),
    }),
  ],
});

export function GraphqlProvider({ children }: { children?: ReactNode }) {
  return <Provider value={client}>{children}</Provider>;
}
