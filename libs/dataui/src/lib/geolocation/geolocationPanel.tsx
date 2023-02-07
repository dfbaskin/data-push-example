import { ReactNode, useEffect, useState } from 'react';
import { TruckSegments } from '../truck';
import { VanSegments } from '../van';
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
        <symbol id="truck-icon" fill="brown" viewBox="0 0 16 16">
          <g transform="scale(0.09) translate(-116, -0)">
            <TruckSegments />
          </g>
        </symbol>
        <symbol id="van-icon" fill="blue" viewBox="0 0 32 32">
          <g transform="scale(0.1) translate(-208, -0)">
            <VanSegments />
          </g>
        </symbol>
        <circle cx={center.x} cy={center.y} r="0.002" fill="orange" />
        {/* <use href="#truck-icon" x={center.x} y={center.y} /> */}
        {/* <use href="#van-icon" x={center.x} y={center.y} /> */}
        {element}
      </svg>
    </div>
  );
}

export default GeolocationPanel;
