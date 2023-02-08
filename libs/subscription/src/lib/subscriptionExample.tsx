import { DataUI } from '@example/dataui';
import { GraphqlProvider } from './graphqlClient';
import Groups from './groups';
import Geolocation from './geolocation';
import Grid from './grid';

function ToDo() {
  return <div>To Do</div>;
}

export function SubscriptionExample() {
  return (
    <GraphqlProvider>
      <DataUI
        groupsElement={<Groups />}
        gridElement={<Grid />}
        geolocationElement={<Geolocation />}
        detailsElement={<ToDo />}
      />
    </GraphqlProvider>
  );
}

export default SubscriptionExample;
