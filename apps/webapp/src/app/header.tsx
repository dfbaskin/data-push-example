import { Link } from 'react-router-dom';
import styles from "./header.module.scss";

export function Header() {
  return (
    <div role="navigation" className={styles["header"]}>
      <div>
        <h1>Push Example</h1>
      </div>
      <div>
      <ul>
        <li>
          <Link to="/">Home</Link>
        </li>
        <li>
          <Link to="/page-2">Page 2</Link>
        </li>
      </ul>
      </div>
    </div>
  );
}

export default Header;
