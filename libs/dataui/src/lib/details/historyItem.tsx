import { formatDateText } from '../store/dateFormatter';
import styles from './historyItem.module.scss';

interface Props {
  item: {
    timestampUTC: string;
    message: string;
  };
}

export function HistoryItem(props: Props) {
  const { item } = props;
  return (
    <div className={styles['item']}>
      {formatDateText(item.timestampUTC)} - {item.message}
    </div>
  );
}
