import { useEffect, useRef } from 'react';
import { POLLING_DELAY_MS } from './config';

export function usePolling(fn: () => void) {
  const ref = useRef<typeof fn>(fn);
  ref.current = fn;

  useEffect(() => {
    const handle = setInterval(() => {
      ref.current();
    }, POLLING_DELAY_MS);
    return () => clearInterval(handle);
  }, []);
}
