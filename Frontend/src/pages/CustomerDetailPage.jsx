import { useState, useEffect } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { getCustomer, getCustomerSum, getInstallmentsByCustomer, createLoan } from '../api/client';
import { StatusBadge, formatMoney, formatDate, loanTypeLabel } from '../components/helpers.jsx';

const loanTypes = [
  { value: 1, label: 'İhtiyaç Kredisi', interestRate: 18.0, termInMonths: 12 },
  { value: 2, label: 'Eğitim Kredisi', interestRate: 12.0, termInMonths: 12 },
  { value: 3, label: 'Taşıt Kredisi', interestRate: 24.0, termInMonths: 12 },
  { value: 4, label: 'Konut Kredisi', interestRate: 30.0, termInMonths: 12 },
  { value: 5, label: 'İşletme Kredisi', interestRate: 21.0, termInMonths: 6 },
];

const loanEmpty = { type: 1, principalAmount: '', interestRate: 18.0, termInMonths: 12 };

export default function CustomerDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [customer, setCustomer]         = useState(null);
  const [summary,  setSummary]          = useState(null);
  const [installments, setInstallments] = useState([]);
  const [loading, setLoading]           = useState(true);
  const [tab, setTab]                   = useState('summary'); // 'summary' | 'installments'
  const [modal, setModal]               = useState(false);
  const [form, setForm]                 = useState(loanEmpty);
  const [saving, setSaving]             = useState(false);
  const [error, setError]               = useState('');
  const [success, setSuccess]           = useState('');

  const load = async () => {
    try {
      const [c, s] = await Promise.all([getCustomer(id), getCustomerSum(id)]);
      setCustomer(c);
      setSummary(s);
    } catch (e) { setError(e.message); }
    finally { setLoading(false); }
  };

  const loadInstallments = async () => {
    try { setInstallments(await getInstallmentsByCustomer(id)); }
    catch (e) { setError(e.message); }
  };

  useEffect(() => { load(); }, [id]);

  const handleTab = async (t) => {
    setTab(t);
    if (t === 'installments' && installments.length === 0) await loadInstallments();
  };

  const handleChange = e => setForm(f => ({ ...f, [e.target.name]: e.target.value }));

  const handleLoanSubmit = async e => {
    e.preventDefault();
    setSaving(true); setError(''); setSuccess('');
    try {
      await createLoan({
        customerId: Number(id),
        type: Number(form.type),
        principalAmount: parseFloat(form.principalAmount),
        interestRate: parseFloat(form.interestRate),
        termInMonths: parseInt(form.termInMonths),
      });
      setSuccess('Kredi başarıyla oluşturuldu!');
      setModal(false);
      setForm(loanEmpty);
      await load();
    } catch (e) { setError(e.message); }
    finally { setSaving(false); }
  };

  if (loading) return <div className="page"><div className="spinner" /></div>;
  if (!customer) return <div className="page"><div className="alert alert-error">Müşteri bulunamadı.</div></div>;

  return (
    <div className="page">
      <div className="breadcrumb">
        <Link to="/customers">Müşteriler</Link>
        <span>›</span>
        <span>{customer.firstName} {customer.lastName}</span>
      </div>

      {error   && <div className="alert alert-error"   style={{marginBottom:16}}>{error}</div>}
      {success && <div className="alert alert-success" style={{marginBottom:16}}>{success}</div>}

      {/* Header */}
      <div className="page-header">
        <div>
          <div className="page-title" style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
            {customer.firstName} {customer.lastName}
            <span style={{ fontSize: 12, padding: '2px 8px', background: 'var(--bg-hover)', border: '1px solid var(--border)', borderRadius: 12, color: 'var(--blue)', fontWeight: 500 }}>
              🎯 Kredi Skoru: <strong>{summary?.creditScore ?? customer.creditScore ?? 1200}</strong>
            </span>
          </div>
          <div className="page-subtitle">TC: {customer.identityNumber} · {customer.email}</div>
        </div>
        <button id="btn-new-loan" className="btn btn-primary" onClick={() => setModal(true)}>
          + Yeni Kredi
        </button>
      </div>

      {/* Müşteri detayları */}
      <div className="card" style={{marginBottom:24}}>
        <div className="detail-grid">
          <div className="detail-item"><span className="detail-label">Telefon</span><span className="detail-value">{customer.phone || '—'}</span></div>
          <div className="detail-item"><span className="detail-label">Adres</span><span className="detail-value">{customer.address || '—'}</span></div>
          <div className="detail-item"><span className="detail-label">Toplam Kredi</span><span className="detail-value">{customer.loans?.length ?? 0}</span></div>
        </div>
      </div>

      {/* Sekmeler */}
      <div style={{display:'flex',gap:8,marginBottom:20}}>
        {[['summary','📊 Özet'],['installments','📋 Taksitler']].map(([key,label]) => (
          <button key={key}
            className={`btn ${tab===key?'btn-primary':'btn-ghost'}`}
            onClick={() => handleTab(key)}>
            {label}
          </button>
        ))}
      </div>

      {tab === 'summary' && summary && (
        <>
          <div className="stats-grid">
            <div className="card"><div className="card-title">Toplam Borç</div><div className="card-value red">{formatMoney(summary.totalLoanDebt)}</div></div>
            <div className="card"><div className="card-title">Kalan Anapara</div><div className="card-value amber">{formatMoney(summary.remainingPrincipal)}</div></div>
            <div className="card"><div className="card-title">Gecikmiş Taksit</div><div className={`card-value ${summary.delayedInstallmentCount>0?'red':'green'}`}>{summary.delayedInstallmentCount}</div></div>
          </div>

          <div style={{display:'grid',gridTemplateColumns:'1fr 1fr',gap:20}}>
            <InstallmentTable title="✅ Ödenen Taksitler" data={summary.paidInstallments} />
            <InstallmentTable title="⏳ Ödenmeyen Taksitler" data={summary.unpaidInstallments} />
          </div>
        </>
      )}

      {tab === 'installments' && (
        <AllInstallmentsTable installments={installments} navigate={navigate} />
      )}

      {/* Kredi Oluşturma Modalı */}
      {modal && (
        <div className="modal-overlay" onClick={e => e.target===e.currentTarget && setModal(false)}>
          <div className="modal">
            <div className="modal-header">
              <h2>Yeni Kredi Başvurusu</h2>
              <button className="btn btn-ghost btn-sm btn-icon" onClick={() => setModal(false)}>✕</button>
            </div>
            <form onSubmit={handleLoanSubmit}>
              <div className="modal-body">
                {error && <div className="alert alert-error" style={{marginBottom:16}}>{error}</div>}
                <div className="form">
                  <div className="form-group">
                    <label>Kredi Türü</label>
                    <select
                      name="type"
                      value={form.type}
                      onChange={e => {
                        const val = Number(e.target.value);
                        const config = loanTypes.find(t => t.value === val);
                        setForm(f => ({
                          ...f,
                          type: val,
                          interestRate: config ? config.interestRate : 18.0,
                          termInMonths: config ? config.termInMonths : 12
                        }));
                      }}
                    >
                      {loanTypes.map(t => <option key={t.value} value={t.value}>{t.label} (Faiz: %{t.interestRate}, Vade: {t.termInMonths} Ay)</option>)}
                    </select>
                  </div>
                  <div className="form-row">
                    <div className="form-group">
                      <label>Ana Para (₺)</label>
                      <input type="number" name="principalAmount" value={form.principalAmount} onChange={handleChange} required min="1000" step="500" />
                    </div>
                    <div className="form-group">
                      <label style={{ display: 'flex', justifyContent: 'space-between' }}>
                        <span>Yıllık Faiz Oranı (%)</span>
                        <span style={{ fontSize: 10, color: 'var(--blue)', fontWeight: 600 }}>Kategori Standardı</span>
                      </label>
                      <input type="number" name="interestRate" value={form.interestRate} readOnly style={{ background: 'var(--bg-input)', opacity: 0.85, cursor: 'not-allowed' }} />
                    </div>
                  </div>
                  <div className="form-group">
                    <label style={{ display: 'flex', justifyContent: 'space-between' }}>
                      <span>Vade (Ay)</span>
                      <span style={{ fontSize: 10, color: 'var(--blue)', fontWeight: 600 }}>Sabit Vade</span>
                    </label>
                    <input type="number" name="termInMonths" value={form.termInMonths} readOnly style={{ background: 'var(--bg-input)', opacity: 0.85, cursor: 'not-allowed' }} />
                  </div>
                </div>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn-ghost" onClick={() => setModal(false)}>İptal</button>
                <button type="submit" id="btn-submit-loan" className="btn btn-primary" disabled={saving}>
                  {saving ? 'Başvuruluyor…' : 'Kredi Başvurusu Yap'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

function InstallmentTable({ title, data }) {
  if (!data?.length) return (
    <div className="table-wrap">
      <div className="table-header"><h2>{title}</h2></div>
      <div className="empty-state" style={{padding:'30px'}}><p style={{fontSize:13}}>Kayıt yok</p></div>
    </div>
  );
  return (
    <div className="table-wrap">
      <div className="table-header">
        <h2>{title}</h2>
        <span className="badge badge-blue">{data.length}</span>
      </div>
      <table>
        <thead><tr><th>No</th><th>Tutar</th><th>Vade</th><th>Durum</th></tr></thead>
        <tbody>
          {data.map(i => (
            <tr key={i.id}>
              <td className="td-dim">#{i.installmentNumber}</td>
              <td className="td-money">{formatMoney(i.amount)}</td>
              <td className="td-dim">{formatDate(i.dueDate)}</td>
              <td>
                <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                  <span className={`badge ${i.status==='Ödendi'?'badge-green':i.status==='Gecikmiş'?'badge-red':'badge-amber'}`}>
                    {i.status}
                  </span>
                  {i.paymentDate && (
                    <span style={{ fontSize: 11, color: 'var(--text-dim)' }}>
                      ({formatDate(i.paymentDate)})
                    </span>
                  )}
                </div>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

function AllInstallmentsTable({ installments, navigate }) {
  if (!installments.length) return (
    <div className="empty-state"><div className="icon">📋</div><p>Taksit bulunamadı</p></div>
  );
  return (
    <div className="table-wrap">
      <div className="table-header">
        <h2>Tüm Taksitler (Yönetim Görünümü)</h2>
        <span className="badge badge-blue">{installments.length} taksit</span>
      </div>
      <table>
        <thead>
          <tr><th>Kredi #</th><th>Taksit No</th><th>Tutar</th><th>Vade Tarihi</th><th>Durum</th></tr>
        </thead>
        <tbody>
          {installments.map(i => (
            <tr key={i.id}>
              <td><span style={{cursor:'pointer',color:'var(--blue)'}} onClick={() => navigate(`/loans/${i.loanId}`)}>#{i.loanId}</span></td>
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
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
