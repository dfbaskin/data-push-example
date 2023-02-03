import { ReactNode } from "react";
import styles from "./groups.module.scss";

interface Props {
  element?: ReactNode
}

export function Groups(props: Props) {
  const { element } = props;
  return (
    <div className={styles['groups']}>
      {element}
    </div>
  );
}

export default Groups;
