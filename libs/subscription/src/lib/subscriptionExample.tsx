import { DataUI } from '@example/dataui';
import { GraphqlProvider } from './graphqlClient';
import Groups from './groups';

function ToDo() {
  return <div>To Do</div>;
}

export function SubscriptionExample() {
  return (
    <GraphqlProvider>
      <DataUI
        groupsElement={<Groups />}
        gridElement={<ToDo />}
        geolocationElement={<ToDo />}
        detailsElement={<ToDo />}
      />
    </GraphqlProvider>
  );
}

export default SubscriptionExample;
