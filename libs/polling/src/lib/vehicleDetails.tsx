import { VehicleView, VehicleViewData } from '@example/dataui';
import { useQuery } from 'urql';
import { usePolling } from './usePolling';

const vehicleQuery = `
query vehicle($id: String!) {
  vehicle(vehicleId: $id) {
    vehicleId
    vehicleType
    status
    location {
      latitude
      longitude
      address
    }
    history {
      timestampUTC
      message
    }
  }
}
`;

interface Data {
  vehicle: VehicleViewData;
}

interface Props {
  vehicleId: string;
}

export function VehicleDetails(props: Props) {
  const { vehicleId } = props;
  const [{ data }, reexecuteQuery] = useQuery<Data>({
    query: vehicleQuery,
    requestPolicy: 'network-only',
    variables: {
      id: vehicleId,
    },
  });
  usePolling(() => {
    reexecuteQuery();
  });

  if (!data) {
    return null;
  }

  return <VehicleView data={data.vehicle} />;
}

export default VehicleDetails;
