import { TransportView, TransportViewData } from '@example/dataui';
import { useQueryAndSubscription } from './useQueryAndSubscription';

const sharedDataQuery = `
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
`;

const dataQuery = `
query transport($id: String!) {
  transport(transportId: $id) {
  ${sharedDataQuery}
  }
}
`;

const subscribeQuery = `
subscription transport($id: String!) {
  transportByIdUpdated(transportId: $id) {
${sharedDataQuery}
  }
}
`;

interface SubscriptionData {
  transportByIdUpdated: TransportViewData;
}

interface QueryData {
  transport?: TransportViewData;
}
interface Props {
  transportId: string;
}

export function TransportDetails(props: Props) {
  const { transportId } = props;

  const [result] = useQueryAndSubscription<
    SubscriptionData,
    QueryData
  >({
    query: dataQuery,
    subscription: subscribeQuery,
    variables: {
      id: transportId,
    },
    onQuery: (data) => {
      return {
        transport: data.transport,
      };
    },
    onSubscribe: (data, current) => {
      if (!current) {
        current = {};
      }
      return {
        transport: data.transportByIdUpdated
      };
    },
  });

  if (!result || !result.transport) {
    return null;
  }

  const { transport } = result;
  return <TransportView data={transport} />;
}

export default TransportDetails;
