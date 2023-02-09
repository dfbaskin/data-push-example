import { DriverView, DriverViewData } from '@example/dataui';
import { useQueryAndSubscription } from './useQueryAndSubscription';

const sharedDataQuery = `
    driverId
    name
    groupAssignment
    status
    history {
      timestampUTC
      message
    }
`;

const dataQuery = `
query driver($id: String!) {
  driver(driverId: $id) {
${sharedDataQuery}
  }
}
`;

const subscribeQuery = `
subscription driver($id: String!) {
  driverByIdUpdated(driverId: $id) {
${sharedDataQuery}
  }
}
`;

interface SubscriptionData {
  driverByIdUpdated: DriverViewData;
}

interface QueryData {
  driver?: DriverViewData;
}
interface Props {
  driverId: string;
}

export function DriverDetails(props: Props) {
  const { driverId } = props;

  const [result] = useQueryAndSubscription<
    SubscriptionData,
    QueryData
  >({
    query: dataQuery,
    subscription: subscribeQuery,
    variables: {
      id: driverId,
    },
    onQuery: (data) => {
      return {
        driver: data.driver,
      };
    },
    onSubscribe: (data, current) => {
      if (!current) {
        current = {};
      }
      return {
        driver: data.driverByIdUpdated
      };
    },
  });

  if (!result || !result.driver) {
    return null;
  }

  const { driver } = result;
  return <DriverView data={driver} />;
}

export default DriverDetails;
