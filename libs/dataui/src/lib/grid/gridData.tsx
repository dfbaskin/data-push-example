import { CellClickedEvent, ValueFormatterParams } from 'ag-grid-community';
import { AgGridReact } from 'ag-grid-react';
import classNames from 'classnames';
import { useDataUiStore } from '../store/dataUiStore';
import styles from './gridData.module.scss';

const formatter = new Intl.NumberFormat(undefined, {
  maximumFractionDigits: 5,
});

function formatLatLong(params: ValueFormatterParams) {
  return formatter.format(params.value);
}

const columnDefs = [
  { field: 'transportId' },
  { field: 'transportStatus' },
  { field: 'vehicleId' },
  { field: 'vehicleType' },
  { field: 'latitude', valueFormatter: formatLatLong },
  { field: 'longitude', valueFormatter: formatLatLong },
  { field: 'address' },
  { field: 'driverId' },
  { field: 'driverStatus' },
];

export interface GridDataType {
  transportId: string;
  transportStatus: string;
  vehicleId: string;
  vehicleType?: 'Truck' | 'Van';
  latitude?: number;
  longitude?: number;
  address?: string;
  driverId: string;
  driverStatus?: string;
}

interface Props {
  data: GridDataType[];
}

export function GridData(props: Props) {
  const { data } = props;
  const { setDetailsDisplay } = useDataUiStore();

  const onCellClicked = (evt: CellClickedEvent) => {
    const fieldName = evt.colDef.field;
    const fieldValue = evt.value;
    switch (fieldName) {
      case 'transportId':
        setDetailsDisplay({
          view: 'transport',
          transportId: fieldValue,
        });
        break;
      case 'driverId':
        setDetailsDisplay({
          view: 'driver',
          driverId: fieldValue,
        });
        break;
      case 'vehicleId':
        setDetailsDisplay({
          view: 'vehicle',
          vehicleId: fieldValue,
        });
        break;
    }
  };

  const gridClassName = classNames('ag-theme-alpine', styles['gridTheme']);
  return (
    <div className={gridClassName}>
      <AgGridReact
        rowData={data}
        columnDefs={columnDefs}
        getRowId={(params) => params.data.transportId}
        onCellClicked={onCellClicked}
      />
    </div>
  );
}

export default GridData;
