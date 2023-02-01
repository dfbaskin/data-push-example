import TestPing from "./testPing";
import styles from "./home.module.scss";

export function Home() {
  return (
    <div className={styles['home']}>
      <div>Examples of pushing data to browser.</div>
      <div>
        <TestPing />
      </div>
    </div>
  );
}

export default Home;
