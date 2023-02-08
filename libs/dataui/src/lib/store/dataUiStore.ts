import { create } from 'zustand';

export type DetailsDisplay =
  | {
      view: 'none';
    }
  | {
      view: 'transport';
      transportId: string;
    }
  | {
      view: 'driver';
      driverId: string;
    }
  | {
      view: 'vehicle';
      vehicleId: string;
    };

export interface DataUiState {
  display: DetailsDisplay;
  setDetailsDisplay: (display: DetailsDisplay) => void;
}

export const useDataUiStore = create<DataUiState>()((set) => ({
  display: { view: 'none' },
  setDetailsDisplay: (display: DetailsDisplay) => {
    set({
      display,
    });
  },
}));
