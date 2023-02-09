import { useMemo, useRef } from 'react';
import { useQuery, useSubscription } from 'urql';

export interface QueryAndSubscriptionConfig<S, Q, T = Q> {
  query: string;
  subscription: string;
  variables?: Record<string, unknown>;
  onQuery: (data: Q) => T;
  onSubscribe: (data: S, current?: T) => T;
}

export function useQueryAndSubscription<S, Q, T = Q>(
  config: QueryAndSubscriptionConfig<S, Q, T>
): [T | undefined] {
  const onQueryRef = useRef(config.onQuery);
  onQueryRef.current = config.onQuery;

  const [{ data: initialQueryData, fetching }] = useQuery<Q>({
    query: config.query,
    variables: config.variables,
  });

  const queryData = useMemo(() => {
    return initialQueryData ? onQueryRef.current(initialQueryData) : undefined;
  }, [initialQueryData]);

  const [{ data: subscriptionData }] = useSubscription<S, T | undefined>(
    {
      query: config.subscription,
      variables: config.variables,
      pause: fetching,
    },
    (current = queryData, data) => {
      return current ? config.onSubscribe(data, current) : undefined;
    }
  );

  return [subscriptionData ?? queryData];
}
