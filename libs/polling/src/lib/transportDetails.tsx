import { TransportView } from '@example/dataui';
import { useQuery } from 'urql';
import { usePolling } from './usePolling';

const transportQuery = `
query transport($id: String!) {
  transport(transportId: $id) {
    transportId
    status
    beginTimestampUTC
    endTimestampUTC
    manifest {
      createdTimestampUTC
      items {
        itemId
        quantity
        description
      }
    }
    driver {
      driverId
      name
      groupAssignment
      status
    }
    vehicle {
      vehicleId
      vehicleType
      status
      location {
        latitude
        longitude
        address
      }
    }
    history {
      timestampUTC
      message
    }
  }
}
`;

interface Data {
  transport: {
    transportId: string;
    status: string;
    beginTimestampUTC: string;
    endTimestampUTC: string;
    manifest: {
      createdTimestampUTC: string;
      items: {
        itemId: string;
        quantity: number;
        description: string;
      }[];
    };
    driver: {
      driverId: string;
      name: string;
      groupAssignment?: string;
      status: string;
    };
    vehicle: {
      vehicleId: string;
      vehicleType: string;
      status: string;
      location: {
        latitude: number;
        longitude: number;
        address: string;
      };
    };
    history: {
      timestampUTC: string;
      message: string;
    }[];
  };
}

interface Props {
  transportId: string;
}

export function TransportDetails(props: Props) {
  const { transportId } = props;
  const [{ data }, reexecuteQuery] = useQuery<Data>({
    query: transportQuery,
    requestPolicy: 'network-only',
    variables: {
      id: transportId,
    },
  });
  usePolling(() => {
    reexecuteQuery();
  });

  if (!data) {
    return null;
  }

  return <TransportView transportId={transportId} />;
}

export default TransportDetails;
