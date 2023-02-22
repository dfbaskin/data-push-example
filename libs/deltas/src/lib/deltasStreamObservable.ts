import { webSocket } from 'rxjs/webSocket';
import { Observable } from 'rxjs';
import { Operation } from 'fast-json-patch';

export type DeltasStreamRequest =
  | {
      streamType: 'GROUPS' | 'GEOLOCATION' | 'TRANSPORTS';
      subscribe: boolean;
    }
  | {
      streamType: 'TRANSPORT_DETAILS' | 'VEHICLE_DETAILS' | 'DRIVER_DETAILS';
      subscribe: boolean;
      id: string;
    };

export type DeltasUpdateInitial = {
  initial: unknown;
};
export type DeltasUpdatePatches = {
  patches: Operation[];
};
export type DeltasUpdateStream =
  | ({
      streamType: 'GROUPS' | 'GEOLOCATION' | 'TRANSPORTS';
    } & DeltasUpdateInitial)
  | ({
      streamType: 'GROUPS' | 'GEOLOCATION' | 'TRANSPORTS';
    } & DeltasUpdatePatches)
  | ({
      streamType: 'TRANSPORT_DETAILS' | 'VEHICLE_DETAILS' | 'DRIVER_DETAILS';
      id: string;
    } & DeltasUpdateInitial)
  | ({
      streamType: 'TRANSPORT_DETAILS' | 'VEHICLE_DETAILS' | 'DRIVER_DETAILS';
      id: string;
    } & DeltasUpdatePatches);

export function isInitialData(
  data: DeltasUpdateInitial | DeltasUpdatePatches
): data is DeltasUpdateInitial {
  return (data as DeltasUpdateInitial).initial !== undefined;
}

export function isPatchesData(
  data: DeltasUpdateInitial | DeltasUpdatePatches
): data is DeltasUpdatePatches {
  return (data as DeltasUpdatePatches).patches !== undefined;
}

// const wsUrl = new URL('/deltas-stream', globalThis.window.location.href);
// wsUrl.protocol = 'ws:';
const wsUrl = new URL('ws://localhost:4202/deltas-stream');

const subject = webSocket(wsUrl.toString());

export function requestDeltasStream(request: DeltasStreamRequest) {
  subject.next(request);
}

export function deltasStreamObservable() {
  return subject.asObservable() as Observable<DeltasUpdateStream>;
}
