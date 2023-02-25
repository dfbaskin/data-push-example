import { GridData, GridDataType } from '@example/dataui';
import { applyPatch } from 'fast-json-patch';
import { useEffect, useState } from 'react';
import { tap } from 'rxjs';
import {
  deltasStreamObservable,
  isInitialData,
  isPatchesData,
  requestDeltasStream,
} from './deltasStreamObservable';

export function Grid() {
  const [data, setData] = useState<GridDataType[]>([]);

  useEffect(() => {
    const subscription = deltasStreamObservable()
      .pipe(
        tap((data) => {
          if (data.streamType === 'TRANSPORTS') {
            if (isInitialData(data)) {
              setData(data.initial as GridDataType[]);
            } else if (isPatchesData(data)) {
              setData((items) => {
                return items.map((row) => {
                  if (row.transportId === data.id) {
                    row = applyPatch(
                      row,
                      data.patches,
                      false,
                      false
                    ).newDocument;
                  }
                  return row;
                });
              });
            }
          }
        })
      )
      .subscribe();
    requestDeltasStream({
      streamType: 'TRANSPORTS',
      subscribe: true,
    });
    return () => subscription.unsubscribe();
  }, []);

  const gridData = data.sort((a, b) =>
    a.transportId.localeCompare(b.transportId)
  );

  return <GridData data={gridData} />;
}

export default Grid;
