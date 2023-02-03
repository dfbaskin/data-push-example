import { ReactNode } from 'react';
import Details from './details';
import Geolocation from './geolocation';
import Grid from './grid';
import Groups from './groups';
import styles from './dataui.module.scss';

interface Props {
  groupsElement?: ReactNode;
  gridElement?: ReactNode;
  geolocationElement?: ReactNode;
  detailsElement?: ReactNode;
}

export function DataUI(props: Props) {
  const { groupsElement, gridElement, geolocationElement, detailsElement } =
    props;
  return (
    <div className={styles['container']}>
      <Groups element={groupsElement} />
      <Grid element={gridElement} />
      <Geolocation element={geolocationElement} />
      <Details element={detailsElement} />
    </div>
  );
}

export default DataUI;
