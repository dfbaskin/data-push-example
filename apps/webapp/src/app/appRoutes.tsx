import { PollingExample } from '@example/polling';
import { Route, Routes } from 'react-router-dom';
import Home from './home';

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/polling" element={<PollingExample />} />
    </Routes>
  );
}

export default AppRoutes;
