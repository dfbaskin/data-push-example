import { DataUI } from '@example/dataui';
import { Route, Routes, Link } from 'react-router-dom';
import Home from './home';

export function AppRoutes() {
  return (
      <Routes>
        <Route
          path="/"
          element={
            <Home />
          }
        />
        <Route
          path="/polling"
          element={
            <DataUI />
          }
        />
      </Routes>
  );
}

export default AppRoutes;
