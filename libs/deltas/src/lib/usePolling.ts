import { useEffect, useRef } from 'react';

export function usePolling(fn: () => void) {
  const ref = useRef<typeof fn>(fn);
  ref.current = fn;

  useEffect(() => {
    const handle = setInterval(() => {
      ref.current();
    }, 2000);
    return () => clearInterval(handle);
  }, []);
}
