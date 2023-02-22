import { DataUI } from '@example/dataui';
import { GraphqlProvider } from './graphqlClient';
import Groups from './groups';
import Geolocation from './geolocation';
import Grid from './grid';
import Details from './details';

export function DeltasExample() {
  return (
    <GraphqlProvider>
      <DataUI
        groupsElement={<Groups />}
        gridElement={<Grid />}
        geolocationElement={<Geolocation />}
        detailsElement={<Details />}
      />
    </GraphqlProvider>
  );
}

export default DeltasExample;
