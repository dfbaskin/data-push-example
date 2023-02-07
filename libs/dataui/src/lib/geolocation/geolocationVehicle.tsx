import classNames from 'classnames';
import styles from './geolocationVehicle.module.scss';

function isValid(value?: number): value is number {
  return value !== null && value !== undefined;
}

interface Props {
  vehicleType: 'Truck' | 'Van';
  latitude?: number;
  longitude?: number;
}

export function GeolocationVehicle(props: Props) {
  const { vehicleType, latitude, longitude } = props;
  if (!isValid(latitude) || !isValid(longitude)) {
    return null;
  }

  const getVehicle = () => {
    switch (vehicleType) {
      case 'Truck':
        return <use href="#truck-icon" x={0} y={0} />;
      case 'Van':
        return <use href="#van-icon" x={0} y={0} />;
    }
    return <circle cx={0} cy={0} r="0.003" />;
  };

  const groupClass = classNames(styles['group'], vehicleType?.toLowerCase());

  const transform = `translate(${longitude}, ${latitude})`;
  return (
    <g className={groupClass} transform={transform}>
      {getVehicle()}
    </g>
  );
}

export default GeolocationVehicle;
