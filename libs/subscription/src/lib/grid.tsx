import { GridData, GridDataType } from '@example/dataui';
import { useQuery, useSubscription } from 'urql';

const gridDataQuery = `
    transportId
    status
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

const gridQuery = `
{
  activeTransports {
${gridDataQuery}
  }
}
`;

const subscribeQuery = `
subscription {
  transportUpdated {
${gridDataQuery}
  }
}
`;

interface GridQueryData {
  transportId: string;
  status: string;
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
  transportUpdated: GridQueryData;
}

interface Data {
  activeTransports: GridQueryData[];
}

const defaultData: Data = {
  activeTransports: [],
};

export function Grid() {
  const [{ data: queryData, fetching }] = useQuery<Data>({
    query: gridQuery,
  });

  const [{ data: subscriptionData }] = useSubscription<SubscriptionData, Data>(
    {
      query: subscribeQuery,
      pause: fetching,
    },
    (
      current = {
        activeTransports: (queryData ?? defaultData).activeTransports,
      },
      data
    ) => {
      return {
        ...current,
        activeTransports: current.activeTransports.reduce(
          (list, transport) => {
            if (transport.transportId !== data.transportUpdated.transportId) {
              list.push(transport);
            }
            return list;
          },
          [data.transportUpdated]
        ),
      };
    }
  );

  const { activeTransports } = subscriptionData ?? queryData ?? defaultData;
  const gridData = activeTransports
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

  return <GridData data={gridData} />;
}

export default Grid;
