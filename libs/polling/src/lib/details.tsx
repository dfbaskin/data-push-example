import { DetailsData } from '@example/dataui';
import DriverDetails from './driverDetails';
import TransportDetails from './transportDetails';

export function Details() {
  return (
    <DetailsData
      element={(display) => {
        if (display.view === 'transport') {
          return <TransportDetails transportId={display.transportId} />;
        }
        else if (display.view === 'driver') {
          return <DriverDetails driverId={display.driverId} />;
        }
        return null;
      }}
    />
  );
}

export default Details;
