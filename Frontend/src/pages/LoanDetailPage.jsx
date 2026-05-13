import { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { getLoan, getInstallmentsByLoan, createPayment, updateLoanStatus } from '../api/client';
import { StatusBadge, formatMoney, formatDate, loanTypeLabel, loanStatusLabel } from '../components/helpers.jsx';

export default function LoanDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [loan, setLoan]               = useState(null);
  const [installments, setInstallments] = useState([]);
  const [loading, setLoading]         = useState(true);
  const [error, setError]             = useState('');
  const [success, setSuccess]         = useState('');
  const [paying, setPaying]           = useState(null); // installment being paid
  const [payAmount, setPayAmount]     = useState('');
  const [saving, setSaving]           = useState(false);

  const load = async () => {
    try {
      const [l, insts] = await Promise.all([getLoan(id), getInstallmentsByLoan(id)]);
      setLoan(l);
      setInstallments(insts);
    } catch(e) { setError(e.message); }
    finally { setLoading(false); }
  };

  useEffect(() => { load(); }, [id]);

  const openPay = (inst) => {
    setPaying(inst);
    setPayAmount(inst.amount.toFixed(2));
    setError('');
  };

  const handlePay = async e => {
    e.preventDefault();
    setSaving(true); setError(''); setSuccess('');
    try {
      await createPayment({ installmentId: paying.id, amount: parseFloat(payAmount) });
      setSuccess(`Taksit #${paying.installmentNumber} başarıyla ödendi!`);
      setPaying(null);
      await load();
    } catch(e) { setError(e.message); }
    finally { setSaving(false); }
  };

  const handleClose = async () => {
    if (!confirm('Krediyi manuel olarak kapatmak istiyor musunuz?')) return;
    try {
      await updateLoanStatus(id, 2);
      setSuccess('Kredi kapatıldı.');
      await load();
    } catch(e) { setError(e.message); }
  };

  if (loading) return <div className="page"><div className="spinner" /></div>;
  if (!loan)   return <div className="page"><div className="alert alert-error">Kredi bulunamadı.</div></div>;

  const paid   = installments.filter(i => i.status === 1);
  const unpaid = installments.filter(i => i.status !== 1);

  return (
    <div className="page">
      <div className="breadcrumb">
        <Link to="/loans">Krediler</Link>
        <span>›</span>
        <span>Kredi #{loan.id}</span>
      </div>

      {error   && <div className="alert alert-error"   style={{marginBottom:16}}>{error}</div>}
      {success && <div className="alert alert-success" style={{marginBottom:16}}>{success}</div>}

      <div className="page-header">
        <div>
          <div className="page-title">{loanTypeLabel[loan.type]} Kredisi #{loan.id}</div>
          <div className="page-subtitle">
            <span style={{cursor:'pointer',color:'var(--blue)', fontWeight: 600}} onClick={() => navigate(`/customers/${loan.customerId}`)}>
              {loan.customer ? `${loan.customer.firstName} ${loan.customer.lastName}` : `Müşteri #${loan.customerId}`}
            </span>
            {' · '} Başlangıç: {formatDate(loan.startDate)}
          </div>
        </div>
        {loan.status === 1 && (
          <button id="btn-close-loan" className="btn btn-danger" onClick={handleClose}>Krediyi Kapat</button>
        )}
      </div>

      {/* Kredi bilgileri */}
      <div className="card" style={{marginBottom:24}}>
        <div className="detail-grid">
          <div className="detail-item"><span className="detail-label">Ana Para</span><span className="detail-value">{formatMoney(loan.principalAmount)}</span></div>
          <div className="detail-item"><span className="detail-label">Toplam Geri Ödeme</span><span className="detail-value" style={{color:'var(--blue)', fontWeight: 700}}>{formatMoney(loan.totalAmount)}</span></div>
          <div className="detail-item"><span className="detail-label">Vade</span><span className="detail-value">{loan.termInMonths} Ay</span></div>
          <div className="detail-item"><span className="detail-label">Durum</span><span className="detail-value"><StatusBadge status={loan.status} type="loan" /></span></div>
          <div className="detail-item"><span className="detail-label">Ödenen</span><span className="detail-value" style={{color:'var(--green)'}}>{paid.length} / {installments.length}</span></div>
          <div className="detail-item"><span className="detail-label">Kalan</span><span className="detail-value" style={{color:'var(--amber)'}}>{formatMoney(unpaid.reduce((s,i) => s+i.amount, 0))}</span></div>
        </div>
      </div>

      {/* Taksit tablosu */}
      <div className="table-wrap">
        <div className="table-header">
          <h2>Taksit Planı</h2>
          <span className="badge badge-blue">{installments.length} taksit</span>
        </div>
        <table>
          <thead>
            <tr><th>No</th><th>Tutar</th><th>Son Ödeme</th><th>Durum</th><th>İşlem</th></tr>
          </thead>
          <tbody>
            {installments.map(i => (
              <tr key={i.id}>
                <td className="td-dim">#{i.installmentNumber}</td>
                <td className="td-money">{formatMoney(i.amount)}</td>
                <td className="td-dim">{formatDate(i.dueDate)}</td>
                <td>
                  <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                    <StatusBadge status={i.status} />
                    {i.status === 1 && i.payment?.paymentDate && (
                      <span style={{ fontSize: 11, color: 'var(--text-dim)' }}>
                        ({formatDate(i.payment.paymentDate)})
                      </span>
                    )}
                  </div>
                </td>
                <td>
                  {i.status !== 1 && loan.status === 1 && (
                    <button id={`btn-pay-${i.id}`} className="btn btn-primary btn-sm" onClick={() => openPay(i)}>
                      Öde
                    </button>
                  )}
                  {i.payment && (
                    <span className="td-dim" style={{fontSize:12}}>
                      {formatDate(i.payment.paymentDate)}
                    </span>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Ödeme Modalı */}
      {paying && (
        <div className="modal-overlay" onClick={e => e.target===e.currentTarget && setPaying(null)}>
          <div className="modal">
            <div className="modal-header">
              <h2>Taksit #{paying.installmentNumber} Öde</h2>
              <button className="btn btn-ghost btn-sm btn-icon" onClick={() => setPaying(null)}>✕</button>
            </div>
            <form onSubmit={handlePay}>
              <div className="modal-body">
                {error && <div className="alert alert-error" style={{marginBottom:16}}>{error}</div>}
                <div className="form">
                  <div className="form-group">
                    <label>Son Ödeme Tarihi</label>
                    <input readOnly value={formatDate(paying.dueDate)} />
                  </div>
                  <div className="form-group">
                    <label>Ödeme Tutarı (₺)</label>
                    <input type="number" value={payAmount} onChange={e => setPayAmount(e.target.value)}
                           step="0.01" required readOnly />
                    <span style={{fontSize:12,color:'var(--text-dim)'}}>Taksit tutarı değiştirilemez</span>
                  </div>
                </div>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn-ghost" onClick={() => setPaying(null)}>İptal</button>
                <button type="submit" id="btn-confirm-pay" className="btn btn-primary" disabled={saving}>
                  {saving ? 'İşleniyor…' : `${formatMoney(parseFloat(payAmount))} Öde`}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
