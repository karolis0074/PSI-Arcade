import { useAuth } from '../auth.jsx';
import { initials } from '../format.js';
import { IconCalendar, IconPerson, IconPin, IconPlus } from './Icons.jsx';

const TABS = [
  { key: 'games', href: '#/games', label: 'Žaidimai', icon: IconCalendar },
  { key: 'fields', href: '#/fields', label: 'Aikštelės', icon: IconPin },
  { key: 'create', href: '#/create', label: 'Naujas', icon: IconPlus },
  { key: 'profile', href: '#/profile', label: 'Profilis', icon: IconPerson },
];

export default function Layout({ active, children }) {
  const { user } = useAuth();

  return (
    <div className="app">
      <header className="header">
        <div className="header-inner">
          <a href="#/games" className="brand">
            <span className="brand-mark" aria-hidden>
              <svg viewBox="0 0 24 24" width="16" height="16" fill="none" stroke="currentColor" strokeWidth="2">
                <circle cx="12" cy="12" r="8.5" />
                <path d="m12 7.5 3.2 2.3-1.2 3.7h-4l-1.2-3.7Z" fill="currentColor" stroke="none" />
              </svg>
            </span>
            PAVADINIMAS
          </a>
          <nav className="nav">
            {TABS.filter((t) => t.key !== 'profile').map((t) => (
              <a key={t.key} href={t.href} className={active === t.key ? 'active' : ''}>
                {t.key === 'create' ? 'Naujas žaidimas' : t.label}
              </a>
            ))}
          </nav>
          <div className="header-end">
            {user ? (
              <a href="#/profile" className={`account-chip${active === 'profile' ? ' active' : ''}`}>
                <span className="avatar avatar-sm">{initials(user.displayName)}</span>
                <span className="account-name">{user.displayName}</span>
              </a>
            ) : (
              <a href="#/login" className="btn btn-primary btn-sm">
                Prisijungti
              </a>
            )}
          </div>
        </div>
      </header>

      <main className="main">{children}</main>

      <nav className="tabbar" aria-label="Pagrindinė navigacija">
        {TABS.map(({ key, href, label, icon: Icon }) => (
          <a key={key} href={href} className={active === key ? 'active' : ''}>
            <Icon />
            <span>{label}</span>
          </a>
        ))}
      </nav>
    </div>
  );
}
