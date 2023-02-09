import { GridData, GridDataType, isOlderThanSeconds } from '@example/dataui';
import { useQueryAndSubscription } from './useQueryAndSubscription';

const sharedDataQuery = `
    transportId
    status
    endTimestampUTC
    vehicle {
      vehicleId
      vehicleType
      location {
        latitude
        longitude
        address
      }
    }
    driver {
      driverId
      status
    }
`;

const dataQuery = `
{
  activeTransports {
${sharedDataQuery}
  }
}
`;

const subscribeQuery = `
subscription {
  transportUpdated {
${sharedDataQuery}
  }
}
`;

interface SharedData {
  transportId: string;
  status: string;
  endTimestampUTC: string;
  vehicle: {
    vehicleId: string;
    vehicleType: 'Truck' | 'Van';
    location: {
      latitude?: number;
      longitude?: number;
      address?: string;
    };
  };
  driver: {
    driverId: string;
    status: string;
  };
}

interface SubscriptionData {
  transportUpdated: SharedData;
}

interface QueryData {
  activeTransports: SharedData[];
}

interface ResultData {
  transports: SharedData[];
}

export function Grid() {
  const [result] = useQueryAndSubscription<
    SubscriptionData,
    QueryData,
    ResultData
  >({
    query: dataQuery,
    subscription: subscribeQuery,
    onQuery: (data) => {
      return {
        transports: data.activeTransports,
      };
    },
    onSubscribe: (data, current) => {
      if (!current) {
        current = {
          transports: [],
        };
      }
      return {
        ...current,
        transports: current.transports.reduce(
          (list, transport) => {
            if (transport.transportId !== data.transportUpdated.transportId) {
              if (!shouldPurgeTransport(transport)) {
                list.push(transport);
              }
            }
            return list;
          },
          [data.transportUpdated]
        ),
      };
    },
  });

  if (!result) {
    return null;
  }

  const { transports } = result;
  const data = transports
    .map((t) => {
      const item: GridDataType = {
        transportId: t.transportId,
        transportStatus: t.status,
        vehicleId: t.vehicle?.vehicleId ?? '?',
        vehicleType: t.vehicle?.vehicleType,
        latitude: t.vehicle?.location?.latitude,
        longitude: t.vehicle?.location?.longitude,
        address: t.vehicle?.location?.address,
        driverId: t.driver?.driverId ?? '?',
        driverStatus: t.driver?.status,
      };
      return item;
    })
    .sort((a, b) => a.transportId.localeCompare(b.transportId));

  return <GridData data={data} />;
}

export default Grid;

function shouldPurgeTransport(transport: SharedData) {
  return (
    transport.status === 'FINISHED' &&
    isOlderThanSeconds(transport.endTimestampUTC, 10)
  );
}
