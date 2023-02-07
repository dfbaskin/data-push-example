import { GridData } from '@example/dataui';
import { useQuery } from 'urql';
import { usePolling } from './usePolling';

const gridQuery = `
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
      status
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
      status: string;
    };
  }[];
}

interface GridData {
  transportId: string;
  transportStatus: string;
  vehicleId: string;
  vehicleType?: 'Truck' | 'Van';
  latitude?: number;
  longitude?: number;
  address?: string;
  driverId: string;
  driverStatus?: string;
}

export function Grid() {
  const [{ data }, reexecuteQuery] = useQuery<Data>({
    query: gridQuery,
    requestPolicy: 'network-only',
  });
  usePolling(() => {
    reexecuteQuery();
  });

  const gridData = (
    data ?? {
      activeTransports: [],
    }
  ).activeTransports
    .map((t) => {
      const item: GridData = {
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
