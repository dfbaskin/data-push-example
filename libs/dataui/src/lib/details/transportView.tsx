import { DetailItem } from './detailItem';
import styles from './transportView.module.scss';

export interface TransportViewData {
  transportId: string;
  status: string;
  beginTimestampUTC: string;
  endTimestampUTC?: string;
  manifest: {
    createdTimestampUTC: string;
    items: {
      itemId: string;
      quantity: number;
      description: string;
    }[];
  };
  driver: {
    driverId: string;
    name: string;
    groupAssignment?: string;
    status: string;
  };
  vehicle: {
    vehicleId: string;
    vehicleType: string;
    status: string;
    location: {
      latitude: number;
      longitude: number;
      address: string;
    };
  };
  history: {
    timestampUTC: string;
    message: string;
  }[];
}

interface Props {
  data: TransportViewData;
}

export function TransportView(props: Props) {
  const {
    data: { transportId, status, beginTimestampUTC, endTimestampUTC },
  } = props;
  return (
    <div className={styles['view']}>
      <h2>Transport ({transportId})</h2>
      <div>
        <DetailItem title='ID:'>{transportId}</DetailItem>
        <DetailItem title='Status:'>{status}</DetailItem>
        <DetailItem title='Begin:'>{beginTimestampUTC}</DetailItem>
        <DetailItem title='End:'>{endTimestampUTC}</DetailItem>
      </div>
    </div>
  );
}

export default TransportView;
