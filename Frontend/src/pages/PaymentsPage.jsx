import { useState, useEffect } from 'react';
import { getPayments } from '../api/client';
import { formatMoney, formatDate } from '../components/helpers.jsx';

export default function PaymentsPage() {
  const [payments, setPayments] = useState([]);
  const [loading, setLoading]   = useState(true);
  const [error, setError]       = useState('');

  useEffect(() => {
    getPayments()
      .then(setPayments)
      .catch(e => setError(e.message))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div className="page">
      <div className="page-header">
        <div>
          <div className="page-title">Ödemeler</div>
          <div className="page-subtitle">Tüm taksit ödeme kayıtları</div>
        </div>
        <div className="card" style={{padding:'12px 20px',display:'flex',gap:24}}>
          <div>
            <div style={{fontSize:11,color:'var(--text-dim)',fontWeight:600,textTransform:'uppercase'}}>Toplam Ödeme</div>
            <div style={{fontSize:18,fontWeight:700,color:'var(--green)'}}>{payments.length}</div>
          </div>
          <div>
            <div style={{fontSize:11,color:'var(--text-dim)',fontWeight:600,textTransform:'uppercase'}}>Toplam Tutar</div>
            <div style={{fontSize:18,fontWeight:700,color:'var(--text-h)'}}>{formatMoney(payments.reduce((s,p)=>s+p.amount,0))}</div>
          </div>
        </div>
      </div>

      {error && <div className="alert alert-error" style={{marginBottom:16}}>{error}</div>}

      {loading ? (
        <div className="spinner" />
      ) : payments.length === 0 ? (
        <div className="empty-state"><div className="icon">💸</div><p>Henüz ödeme kaydı yok</p><p style={{fontSize:12,marginTop:8}}>Ödeme yapmak için Krediler → Detay sayfasına gidin</p></div>
      ) : (
        <div className="table-wrap">
          <div className="table-header">
            <h2>Ödeme Geçmişi</h2>
            <span className="badge badge-green">{payments.length} ödeme</span>
          </div>
          <table>
            <thead>
              <tr>
                <th>#</th>
                <th>Taksit ID</th>
                <th>Tutar</th>
                <th>Ödeme Tarihi</th>
                <th>Durum</th>
              </tr>
            </thead>
            <tbody>
              {payments.map(p => (
                <tr key={p.id}>
                  <td className="td-dim">{p.id}</td>
                  <td className="td-dim">Taksit #{p.installmentId}</td>
                  <td className="td-money">{formatMoney(p.amount)}</td>
                  <td className="td-dim">{formatDate(p.paymentDate)}</td>
                  <td><span className="badge badge-green">Tamamlandı</span></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
