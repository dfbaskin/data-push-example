import { ReactNode } from 'react';
import {
  useDataUiStore,
  DataUiState,
  DetailsDisplay,
} from '../store/dataUiStore';

const getDetailsDisplay = (state: DataUiState) => ({
  display: state.display,
});

interface Props {
  element?: (display: DetailsDisplay) => ReactNode;
}

export function DetailsData(props: Props) {
  const { element } = props;
  const { display } = useDataUiStore(getDetailsDisplay);
  if (display.view === 'none' || !element) {
    return null;
  }
  return <>{element(display)}</>;
}

export default DetailsData;
