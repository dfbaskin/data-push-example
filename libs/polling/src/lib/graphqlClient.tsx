import { ReactNode } from 'react';
import { createClient, Provider } from 'urql';

const client = createClient({
  url: '/graphql',
  maskTypename: true
});

export function GraphqlProvider({ children }: { children?: ReactNode }) {
  return <Provider value={client}>{children}</Provider>;
}
