import { useState } from 'react';

export function TestPing() {
  const [result, setResult] = useState<string>('');
  const pingServer = () => {
    setResult('');
    fetch('/api/ping')
      .then((rsp) => {
        if (rsp.ok) {
          return rsp.json();
        }
        return {
          error: `${rsp.status} ${rsp.statusText}`,
        };
      })
      .then((json) => {
        setResult(JSON.stringify(json, null, 2));
      })
      .catch((err) => {
        setResult(err.toString());
      });
  };

  return (
    <div>
      <div>
        <button type="button" onClick={pingServer}>
          Ping Server
        </button>
      </div>
      {result && (
        <div>
          <pre>{result}</pre>
        </div>
      )}
    </div>
  );
}

export default TestPing;
