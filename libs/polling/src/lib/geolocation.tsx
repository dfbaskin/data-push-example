import { GeolocationPanel, GeolocationVehicle } from '@example/dataui';
import { useQuery } from 'urql';
import { usePolling } from './usePolling';

const mapQuery = `
{
  activeTransports {
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
  }
}
`;

interface Data {
  activeTransports: {
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
  }[];
}

const defaultData: Data = {
  activeTransports: [],
};

export function Groups() {
  const [{ data }, reexecuteQuery] = useQuery<Data>({
    query: mapQuery,
    requestPolicy: 'network-only',
  });
  usePolling(() => {
    reexecuteQuery();
  });

  const { activeTransports } = data ?? defaultData;

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

export default Groups;
