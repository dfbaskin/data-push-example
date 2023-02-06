import { ReactNode } from 'react';
import { createClient, Provider } from 'urql';

const url = new URL('/graphql', globalThis.window.location.href);

const client = createClient({
  url: url.toString(),
  maskTypename: true
});

export function GraphqlProvider({ children }: { children?: ReactNode }) {
  return <Provider value={client}>{children}</Provider>;
}
