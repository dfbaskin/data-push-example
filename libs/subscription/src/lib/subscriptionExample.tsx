import { DataUI } from '@example/dataui';
import { GraphqlProvider } from './graphqlClient';
import Groups from './groups';
import Geolocation from './geolocation';

function ToDo() {
  return <div>To Do</div>;
}

export function SubscriptionExample() {
  return (
    <GraphqlProvider>
      <DataUI
        groupsElement={<Groups />}
        gridElement={<ToDo />}
        geolocationElement={<Geolocation />}
        detailsElement={<ToDo />}
      />
    </GraphqlProvider>
  );
}

export default SubscriptionExample;
