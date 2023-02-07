interface Props {
  vehicleType: 'Truck' | 'Van';
  latitude?: number;
  longitude?: number;
}

export function GeolocationVehicle(props: Props) {
  const { vehicleType, latitude, longitude } = props;
  return <circle cx={longitude} cy={latitude} r="0.003" fill="red" />;
}

export default GeolocationVehicle;
