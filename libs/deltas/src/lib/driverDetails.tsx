import { DriverView, DriverViewData } from '@example/dataui';
import { useQuery } from 'urql';
import { usePolling } from './usePolling';

const driverQuery = `
query driver($id: String!) {
  driver(driverId: $id) {
    driverId
    name
    groupAssignment
    status
    history {
      timestampUTC
      message
    }
  }
}
`;

interface Data {
  driver: DriverViewData;
}

interface Props {
  driverId: string;
}

export function DriverDetails(props: Props) {
  const { driverId } = props;
  const [{ data }, reexecuteQuery] = useQuery<Data>({
    query: driverQuery,
    requestPolicy: 'network-only',
    variables: {
      id: driverId,
    },
  });
  usePolling(() => {
    reexecuteQuery();
  });

  if (!data) {
    return null;
  }

  return <DriverView data={data.driver} />;
}

export default DriverDetails;
