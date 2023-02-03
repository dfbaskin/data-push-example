import { ReactNode } from "react";
import styles from "./geolocation.module.scss";

interface Props {
  element?: ReactNode
}

export function Geolocation(props: Props) {
  const { element } = props;
  return (
    <div className={styles['geolocation']}>
      {element}
    </div>
  );
}

export default Geolocation;
