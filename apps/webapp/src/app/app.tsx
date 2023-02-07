import styles from './app.module.scss';

import Header from './header';
import AppRoutes from './appRoutes';

import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-alpine.css';

export function App() {
  return (
    <div className={styles['layout']}>
      <Header />
      <AppRoutes />
    </div>
  );
}

export default App;
