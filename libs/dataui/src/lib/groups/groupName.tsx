import { ReactNode, useState } from 'react';
import classNames from 'classnames';
import styles from './groupName.module.scss';

interface Props {
  name: string;
  count: number;
  element?: ReactNode;
}

const icon = '➤';

export function GroupName(props: Props) {
  const { name, count, element } = props;
  const [expanded, setExpanded] = useState<boolean>(true);
  const toggleExpCol = (evt: React.MouseEvent<HTMLElement>) => {
    evt.preventDefault();
    setExpanded((v) => !v);
  };
  const iconClassName = classNames(styles['expColIcon'], { expanded });
  return (
    <>
      <div className={styles['groupName']}>
        <span className={iconClassName} onClick={toggleExpCol}>
          {icon}
        </span>
        <span>{name}</span>
        <span>({count})</span>
      </div>
      {expanded && <div className={styles['groupItem']}>{element}</div>}
    </>
  );
}

export default GroupName;
