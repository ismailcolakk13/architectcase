import { NavLink } from 'react-router-dom';

const links = [
  { section: 'Bireysel', items: [
    { to: '/portal', icon: '🏠', label: 'Müşteri Portali' },
  ]},
  { section: 'Yönetim Paneli', items: [
    { to: '/customers', icon: '👤', label: 'Müşteriler' },
    { to: '/loans',     icon: '💳', label: 'Krediler'   },
  ]},
];

export default function Sidebar() {
  return (
    <aside className="sidebar">
      <div className="sidebar-logo">
        <h1><span>Digital</span>Loan</h1>
        <p>Kredi Yönetim Sistemi</p>
      </div>
      <nav className="sidebar-nav">
        {links.map(({ section, items }) => (
          <div key={section} className="nav-section">
            <div className="nav-section-label">{section}</div>
            {items.map(({ to, icon, label }) => (
              <NavLink
                key={to}
                to={to}
                className={({ isActive }) => `nav-link${isActive ? ' active' : ''}`}
              >
                <span className="nav-icon">{icon}</span>
                {label}
              </NavLink>
            ))}
          </div>
        ))}
      </nav>
    </aside>
  );
}
