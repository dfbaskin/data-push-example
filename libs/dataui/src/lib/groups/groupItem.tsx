import Truck from '../truck';
import Van from '../van';
import styles from './groupItem.module.scss';

type VehicleTypes = 'Truck' | 'Van';

interface Props {
  driverId: string;
  driverName: string;
  vehicleId: string;
  vehicleType: VehicleTypes | undefined;
}

export function GroupItem(props: Props) {
  const {
    driverId,
    driverName,
    vehicleId,
    vehicleType
  } = props;

  const getIcon = () => {
    switch (vehicleType) {
      case 'Truck':
        return <Truck />;
      case 'Van':
        return <Van />;
    }
    return <div>&nbsp;</div>;
  };

  return (
    <div className={styles['groupItem']}>
      <span>{getIcon()}</span>
      <span>{driverId}/{vehicleId}</span>
      <span>({driverName})</span>
    </div>
  );
}

export default GroupItem;
