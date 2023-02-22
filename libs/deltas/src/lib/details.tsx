import { DetailsData } from '@example/dataui';
import DriverDetails from './driverDetails';
import TransportDetails from './transportDetails';
import VehicleDetails from './vehicleDetails';

export function Details() {
  return (
    <DetailsData
      element={(display) => {
        if (display.view === 'transport') {
          return <TransportDetails transportId={display.transportId} />;
        } else if (display.view === 'driver') {
          return <DriverDetails driverId={display.driverId} />;
        } else if (display.view === 'vehicle') {
          return <VehicleDetails vehicleId={display.vehicleId} />;
        }
        return null;
      }}
    />
  );
}

export default Details;
