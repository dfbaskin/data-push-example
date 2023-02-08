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
      {item.timestampUTC} - {item.message}
    </div>
  );
}
