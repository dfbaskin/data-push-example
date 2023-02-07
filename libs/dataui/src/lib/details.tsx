import { ReactNode } from 'react';
import styles from './details.module.scss';

interface Props {
  element?: ReactNode;
}

export function Details(props: Props) {
  const { element } = props;
  return <div className={styles['details']}>{element}</div>;
}

export default Details;
