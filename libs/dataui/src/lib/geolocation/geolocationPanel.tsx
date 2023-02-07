import { ReactNode } from 'react';

interface Props {
  element?: ReactNode;
}

export function GeolocationPanel(props: Props) {
  const { element } = props;
  return (
    <div>
      <div>Map</div>
      <div>{element}</div>
    </div>
  );
}

export default GeolocationPanel;
