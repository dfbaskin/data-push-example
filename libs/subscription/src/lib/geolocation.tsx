import { GeolocationPanel, GeolocationVehicle } from '@example/dataui';
import { useQuery, useSubscription } from 'urql';

const transportQuery = `
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
    }
`;

const transportsQuery = `
{
  activeTransports {
${transportQuery}
  }
}
`;

const subscribeQuery = `
subscription {
  transportUpdated {
${transportQuery}
  }
}
`;

interface TransportData {
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
  };
}

interface SubscriptionData {
  transportUpdated: TransportData;
}

interface Data {
  activeTransports: TransportData[];
}

const defaultData: Data = {
  activeTransports: [],
};

export function Geolocation() {
  const [{ data: queryData, fetching }] = useQuery<Data>({
    query: transportsQuery,
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

  return (
    <GeolocationPanel
      element={activeTransports.map((t) => (
        <GeolocationVehicle
          key={t.transportId}
          vehicleType={t.vehicle?.vehicleType}
          latitude={t.vehicle?.location?.latitude}
          longitude={t.vehicle?.location?.longitude}
        />
      ))}
    />
  );
}

export default Geolocation;
