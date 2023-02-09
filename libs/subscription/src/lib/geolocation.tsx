import { GeolocationPanel, GeolocationVehicle } from '@example/dataui';
import { useQueryAndSubscription } from './useQueryAndSubscription';

const sharedQuery = `
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

const dataQuery = `
{
  activeTransports {
${sharedQuery}
  }
}
`;

const subscribeQuery = `
subscription {
  transportUpdated {
${sharedQuery}
  }
}
`;

interface SharedData {
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
  transportUpdated: SharedData;
}

interface QueryData {
  activeTransports: SharedData[];
}

interface ResultData {
  transports: SharedData[];
}

export function Geolocation() {
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
              list.push(transport);
            }
            return list;
          },
          data.transportUpdated.status === 'FINISHED'
            ? []
            : [data.transportUpdated]
        ),
      };
    },
  });

  if (!result) {
    return null;
  }

  const { transports } = result;

  return (
    <GeolocationPanel
      element={transports.map((t) => (
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
