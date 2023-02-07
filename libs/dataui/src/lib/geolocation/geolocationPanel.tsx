import { ReactNode, useEffect, useState } from 'react';
import { fetchCoordinates } from './fetchCoordinates';
import styles from './geolocationPanel.module.scss';

interface Props {
  element?: ReactNode;
}

export function GeolocationPanel(props: Props) {
  const { element } = props;
  const [viewBox, setViewBox] = useState<string>();
  const [center, setCenter] = useState({ x: 0.0, y: 0.0 });

  useEffect(() => {
    fetchCoordinates()
      .then((result) => {
        const {
          data: {
            configuration: {
              area: {
                topLeft: { latitude: top, longitude: left },
                bottomRight: { latitude: bottom, longitude: right },
                center: { latitude: y, longitude: x },
              },
            },
          },
        } = result;
        setViewBox([left, top, right - left, bottom - top].join(' '));
        setCenter({ x, y });
      })
      .catch((err) => {
        console.error(err);
      });
  }, []);

  if (!viewBox) {
    return null;
  }

  return (
    <div className={styles['panel']}>
      <svg viewBox={viewBox}>
        <circle cx={center.x} cy={center.y} r="0.002" fill="orange" />
        {element}
      </svg>
    </div>
  );
}

export default GeolocationPanel;
