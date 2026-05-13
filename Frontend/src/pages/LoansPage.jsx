import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getLoans } from '../api/client';
import { StatusBadge, formatMoney, formatDate, loanTypeLabel } from '../components/helpers.jsx';

export default function LoansPage() {
  const [loans, setLoans]     = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError]     = useState('');
  const [filter, setFilter]   = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    getLoans()
      .then(setLoans)
      .catch(e => setError(e.message))
      .finally(() => setLoading(false));
  }, []);

  const filtered = loans.filter(l =>
    !filter || l.customerId?.toString() === filter || loanTypeLabel[l.type]?.includes(filter)
  );

  return (
    <div className="page">
      <div className="page-header">
        <div>
          <div className="page-title">Krediler</div>
          <div className="page-subtitle">Sistemdeki tüm kredi kayıtları</div>
        </div>
        <input
          id="loan-filter"
          placeholder="Müşteri ID veya tür filtrele…"
          value={filter}
          onChange={e => setFilter(e.target.value)}
          style={{width:220}}
        />
      </div>

      {error && <div className="alert alert-error" style={{marginBottom:16}}>{error}</div>}

      {loading ? (
        <div className="spinner" />
      ) : filtered.length === 0 ? (
        <div className="empty-state"><div className="icon">💳</div><p>Kredi bulunamadı</p></div>
      ) : (
        <div className="table-wrap">
          <div className="table-header">
            <h2>Kredi Listesi</h2>
            <span className="badge badge-blue">{filtered.length} kayıt</span>
          </div>
          <table>
            <thead>
              <tr>
                <th>#</th>
                <th>Müşteri</th>
                <th>Tür</th>
                <th>Vade</th>
                <th>Ana Para</th>
                <th>Toplam Geri Ödeme</th>
                <th>Başlangıç</th>
                <th>Durum</th>
                <th>İşlem</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map(l => (
                <tr key={l.id}>
                  <td className="td-dim">{l.id}</td>
                  <td>
                    <span style={{cursor:'pointer',color:'var(--blue)', fontWeight: 600}}
                          onClick={() => navigate(`/customers/${l.customerId}`)}>
                      {l.customer ? `${l.customer.firstName} ${l.customer.lastName}` : `Müşteri #${l.customerId}`}
                    </span>
                  </td>
                  <td className="td-bold">{loanTypeLabel[l.type] ?? l.type}</td>
                  <td>{l.termInMonths} ay</td>
                  <td className="td-money">{formatMoney(l.principalAmount)}</td>
                  <td className="td-money" style={{color: 'var(--blue)', fontWeight: 600}}>{formatMoney(l.totalAmount)}</td>
                  <td className="td-dim">{formatDate(l.startDate)}</td>
                  <td><StatusBadge status={l.status} type="loan" /></td>
                  <td>
                    <button id={`btn-loan-detail-${l.id}`} className="btn btn-ghost btn-sm"
                            onClick={() => navigate(`/loans/${l.id}`)}>
                      Detay
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
