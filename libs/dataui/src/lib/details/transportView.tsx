import { useMemo } from 'react';
import { DetailItem } from './detailItem';
import { HistoryItem } from './historyItem';
import { Location } from './location';
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
    location?: {
      latitude?: number;
      longitude?: number;
      address?: string;
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
    data: {
      transportId,
      status,
      beginTimestampUTC,
      endTimestampUTC,
      manifest,
      driver,
      vehicle,
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
      <h2>Transport ({transportId})</h2>
      <div>
        <DetailItem title="ID:">{transportId}</DetailItem>
        <DetailItem title="Status:">{status}</DetailItem>
        <DetailItem title="Begin:">{beginTimestampUTC}</DetailItem>
        <DetailItem title="End:">{endTimestampUTC}</DetailItem>
      </div>
      <h2>Manifest</h2>
      <div>
        <DetailItem title="Created:">{manifest.createdTimestampUTC}</DetailItem>
        <table>
          <thead>
            <tr>
              <th>Item Id</th>
              <th>Qty</th>
              <th>Description</th>
            </tr>
          </thead>
          <tbody>
            {manifest.items.map((item) => (
              <tr key={item.itemId}>
                <td>{item.itemId}</td>
                <td>{item.quantity}</td>
                <td>{item.description}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      <h2>Driver</h2>
      <div>
        <DetailItem title="ID:">{driver.driverId}</DetailItem>
        <DetailItem title="Name:">{driver.name}</DetailItem>
        <DetailItem title="Group:">{driver.groupAssignment}</DetailItem>
        <DetailItem title="Status:">{driver.status}</DetailItem>
      </div>
      <h2>Vehicle</h2>
      <div>
        <DetailItem title="ID:">{vehicle.vehicleId}</DetailItem>
        <DetailItem title="Type:">{vehicle.vehicleType}</DetailItem>
        <DetailItem title="Status:">{vehicle.status}</DetailItem>
        <DetailItem title="Location:">
          <Location location={vehicle.location} />
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

export default TransportView;
