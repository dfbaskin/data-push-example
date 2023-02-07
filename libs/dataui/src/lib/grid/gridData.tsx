import { AgGridReact } from 'ag-grid-react';
import classNames from 'classnames';
import styles from "./gridData.module.scss";

const columnDefs = [
  { field: 'transportId' },
  { field: 'transportStatus' },
  { field: 'vehicleId' },
  { field: 'vehicleType' },
  { field: 'latitude' },
  { field: 'longitude' },
  { field: 'address' },
  { field: 'driverId' },
  { field: 'driverStatus' },
];

interface Props {
  data: {
    transportId: string;
    transportStatus: string;
    vehicleId: string;
    vehicleType?: 'Truck' | 'Van';
    latitude?: number;
    longitude?: number;
    address?: string;
    driverId: string;
    driverStatus?: string;
  }[];
}

export function GridData(props: Props) {
  const { data } = props;
  const gridClassName = classNames(
    'ag-theme-alpine',
    styles['gridTheme']
  );
  return (
    <div className={gridClassName}>
      <AgGridReact
        rowData={data}
        columnDefs={columnDefs}
        getRowId={(params) => params.data.transportId}
      />
    </div>
  );
}

export default GridData;
