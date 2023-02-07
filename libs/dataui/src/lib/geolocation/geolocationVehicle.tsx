import classNames from 'classnames';
import styles from './geolocationVehicle.module.scss';

interface Props {
  vehicleType: 'Truck' | 'Van';
  latitude?: number;
  longitude?: number;
}

export function GeolocationVehicle(props: Props) {
  const { vehicleType, latitude, longitude } = props;
  if (latitude === null || longitude === null) {
    return null;
  }

  const groupClass = classNames(styles['group'], vehicleType?.toLowerCase());

  const transform = `translate(${longitude}, ${latitude})`;
  return (
    <g className={groupClass} transform={transform}>
      <circle cx={0} cy={0} r="0.003" />;
    </g>
  );
}

export default GeolocationVehicle;
