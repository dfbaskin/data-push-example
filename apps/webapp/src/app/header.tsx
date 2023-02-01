import { NavLink } from 'react-router-dom';
import styles from './header.module.scss';

export function Header() {
  const navLinkName = styles['navlink'];
  const isActiveClassName = ({ isActive }: { isActive: boolean }) =>
    isActive ? `${navLinkName} active` : navLinkName;
  return (
    <div role="navigation" className={styles['header']}>
      <div>
        <h1>Push Example</h1>
      </div>
      <div>
        <ul>
          <li>
            <NavLink to="/" className={isActiveClassName}>
              Home
            </NavLink>
          </li>
          <li>
            <NavLink to="/polling" className={isActiveClassName}>
              Polling
            </NavLink>
          </li>
          <li>
            <a href="/graphql">GraphQL</a>
          </li>
        </ul>
      </div>
    </div>
  );
}

export default Header;
