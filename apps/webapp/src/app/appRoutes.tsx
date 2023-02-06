import { PollingExample } from '@example/polling';
import { SubscriptionExample } from '@example/subscription';
import { Route, Routes } from 'react-router-dom';
import Home from './home';

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/polling" element={<PollingExample />} />
      <Route path="/subscription" element={<SubscriptionExample />} />
    </Routes>
  );
}

export default AppRoutes;
