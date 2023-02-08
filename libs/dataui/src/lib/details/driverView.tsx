import { useMemo } from 'react';
import { DetailItem } from './detailItem';
import { HistoryItem } from './historyItem';
import styles from './driverView.module.scss';

export interface DriverViewData {
  driverId: string;
  name: string;
  groupAssignment?: string;
  status: string;
  history: {
    timestampUTC: string;
    message: string;
  }[];
}

interface Props {
  data: DriverViewData;
}

export function DriverView(props: Props) {
  const {
    data: {
      driverId,
      name,
      groupAssignment,
      status,
      history
    },
  } = props;

  const sorted = useMemo(() => {
    return history.sort((a, b) => {
      return a.timestampUTC.localeCompare(b.timestampUTC);
    });
  }, [history]);

  return (
    <div className={styles['view']}>
      <h2>Driver ({driverId})</h2>
      <div>
        <DetailItem title="ID:">{driverId}</DetailItem>
        <DetailItem title="Name:">{name}</DetailItem>
        <DetailItem title="Group:">{groupAssignment}</DetailItem>
        <DetailItem title="Status:">{status}</DetailItem>
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

export default DriverView;
