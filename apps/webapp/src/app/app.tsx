import styles from './app.module.scss';

import Header from './header';
import AppRoutes from './appRoutes';

export function App() {
  return (
    <div className={styles["layout"]}>
      <Header />
      <AppRoutes />
    </div>
  );
}

export default App;
