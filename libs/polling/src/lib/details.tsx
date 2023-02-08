import { DetailsData } from '@example/dataui';
import TransportDetails from './transportDetails';

export function Details() {
  return (
    <DetailsData
      element={(display) => {
        if (display.view === 'transport') {
          return <TransportDetails transportId={display.transportId} />;
        }
        return null;
      }}
    />
  );
}

export default Details;
