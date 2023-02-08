import { useMemo } from 'react';
import { DetailItem } from './detailItem';
import { HistoryItem } from './historyItem';
import { Location } from './location';
import styles from './vehicleView.module.scss';

export interface VehicleViewData {
  vehicleId: string;
  vehicleType: string;
  status: string;
  location?: {
    latitude?: number;
    longitude?: number;
    address?: string;
  };
  history: {
    timestampUTC: string;
    message: string;
  }[];
}

interface Props {
  data: VehicleViewData;
}

export function VehicleView(props: Props) {
  const {
    data: {
      vehicleId,
      vehicleType,
      status,
      location,
      history,
    },
  } = props;

  const sorted = useMemo(() => {
    return history.sort((a, b) => {
      return a.timestampUTC.localeCompare(b.timestampUTC);
    });
  }, [history]);

  return (
    <div className={styles['view']}>
      <h2>Vehicle</h2>
      <div>
        <DetailItem title="ID:">{vehicleId}</DetailItem>
        <DetailItem title="Type:">{vehicleType}</DetailItem>
        <DetailItem title="Status:">{status}</DetailItem>
        <DetailItem title="Location:">
          <Location location={location} />
        </DetailItem>
      </div>
      <h2>History</h2>
      <div>
        {sorted.map((item, idx) => (
          <HistoryItem key={idx} item={item} />
        ))}
      </div>
    </div>
  );
}

export default VehicleView;
