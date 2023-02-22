import { TransportView, TransportViewData } from '@example/dataui';
import { useEffect, useState } from 'react';
import {
  deltasStreamObservable,
  isInitialData,
  isPatchesData,
  requestDeltasStream,
} from './deltasStreamObservable';
import { tap } from 'rxjs';
import { applyPatch } from 'fast-json-patch';

interface Props {
  transportId: string;
}

export function TransportDetails(props: Props) {
  const { transportId } = props;
  const [data, setData] = useState<TransportViewData | undefined>();

  useEffect(() => {
    const subscription = deltasStreamObservable()
      .pipe(
        tap((data) => {
          if (
            data.streamType === 'TRANSPORT_DETAILS' &&
            data.id === transportId
          ) {
            if (isInitialData(data)) {
              setData(data.initial as TransportViewData);
            } else if (isPatchesData(data)) {
              setData((doc) => {
                if (!doc) {
                  return undefined;
                }
                return applyPatch(doc, data.patches, false, false).newDocument;
              });
            }
          }
        })
      )
      .subscribe();
    requestDeltasStream({
      streamType: 'TRANSPORT_DETAILS',
      subscribe: true,
      id: transportId,
    });
    return () => subscription.unsubscribe();
  }, [transportId]);

  if (!data) {
    return null;
  }

  return <TransportView data={data} />;
}

export default TransportDetails;
