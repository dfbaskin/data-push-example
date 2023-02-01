import styles from './dataui.module.scss';
import Details from './details';
import Geolocation from './geolocation';
import Grid from './grid';
import Groups from './groups';

export function DataUI() {
  return (
    <div className={styles['container']}>
      <Groups />
      <Grid />
      <Geolocation />
      <Details />
    </div>
  );
}

export default DataUI;
