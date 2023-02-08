import { TransportView, TransportViewData } from '@example/dataui';
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
  transport: TransportViewData;
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

  return <TransportView data={data.transport} />;
}

export default TransportDetails;
