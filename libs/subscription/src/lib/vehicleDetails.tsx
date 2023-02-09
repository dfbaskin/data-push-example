import { VehicleView, VehicleViewData } from '@example/dataui';
import { useQueryAndSubscription } from './useQueryAndSubscription';

const sharedDataQuery = `
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
`;

const dataQuery = `
query vehicle($id: String!) {
  vehicle(vehicleId: $id) {
${sharedDataQuery}
  }
}
`;

const subscribeQuery = `
subscription vehicle($id: String!) {
  vehicleByIdUpdated(vehicleId: $id) {
${sharedDataQuery}
  }
}
`;

interface SubscriptionData {
  vehicleByIdUpdated: VehicleViewData;
}

interface QueryData {
  vehicle?: VehicleViewData;
}
interface Props {
  vehicleId: string;
}

export function VehicleDetails(props: Props) {
  const { vehicleId } = props;

  const [result] = useQueryAndSubscription<SubscriptionData, QueryData>({
    query: dataQuery,
    subscription: subscribeQuery,
    variables: {
      id: vehicleId,
    },
    onQuery: (data) => {
      return {
        vehicle: data.vehicle,
      };
    },
    onSubscribe: (data, current) => {
      if (!current) {
        current = {};
      }
      return {
        vehicle: data.vehicleByIdUpdated,
      };
    },
  });

  if (!result || !result.vehicle) {
    return null;
  }

  const { vehicle } = result;
  return <VehicleView data={vehicle} />;
}

export default VehicleDetails;
