import { ReactNode } from "react";
import styles from "./grid.module.scss";

interface Props {
  element?: ReactNode
}

export function Grid(props: Props) {
  const { element } = props;
  return (
    <div className={styles['grid']}>
      {element}
    </div>
  );
}

export default Grid;
