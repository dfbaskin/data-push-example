import { ReactNode } from 'react';
import styles from "./detailItem.module.scss";

interface Props {
  title: string;
  children?: ReactNode;
}

export function DetailItem(props: Props) {
  const { title, children } = props;
  return (
    <div className={styles["field"]}>
      <div>{title}</div>
      <div>{children}</div>
    </div>
  );
}
