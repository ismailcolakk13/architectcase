import { useState, useEffect } from 'react';
import { getCustomerByIdentity, getCustomerSum, getInstallmentsByLoan, createLoan, createPayment } from '../api/client';
import { StatusBadge, formatMoney, formatDate, loanTypeLabel } from '../components/helpers.jsx';

const loanTypes = [
  { value: 1, label: 'İhtiyaç Kredisi', interestRate: 18.0, termInMonths: 12 },
  { value: 2, label: 'Eğitim Kredisi', interestRate: 12.0, termInMonths: 12 },
  { value: 3, label: 'Taşıt Kredisi', interestRate: 24.0, termInMonths: 12 },
  { value: 4, label: 'Konut Kredisi', interestRate: 30.0, termInMonths: 12 },
  { value: 5, label: 'İşletme Kredisi', interestRate: 21.0, termInMonths: 6 },
];

const loanEmpty = { type: 1, principalAmount: '', interestRate: 18.0, termInMonths: 12 };

export default function CustomerPortalPage() {
  const [idNoInput, setIdNoInput] = useState('');
  const [customer, setCustomer] = useState(null);
  const [summary, setSummary] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  // Selected loan details inside portal
  const [selectedLoan, setSelectedLoan] = useState(null);
  const [installments, setInstallments] = useState([]);
  const [instLoading, setInstLoading] = useState(false);

  // Modals
  const [showLoanModal, setShowLoanModal] = useState(false);
  const [loanForm, setLoanForm] = useState(loanEmpty);
  const [savingLoan, setSavingLoan] = useState(false);

  const [payingInst, setPayingInst] = useState(null);
  const [payingSaving, setPayingSaving] = useState(false);

  // Check local storage for persistent login session
  useEffect(() => {
    const savedIdNo = localStorage.getItem('portal_id_no');
    if (savedIdNo) {
      setIdNoInput(savedIdNo);
      login(savedIdNo);
    }
  }, []);

  const login = async (idNoToUse) => {
    const targetIdNo = idNoToUse || idNoInput;
    if (!targetIdNo || targetIdNo.length !== 11) {
      setError('Lütfen 11 haneli T.C. Kimlik numaranızı giriniz.');
      return;
    }
    setLoading(true);
    setError('');
    setSuccess('');
    try {
      const custObj = await getCustomerByIdentity(targetIdNo);
      const sumObj = await getCustomerSum(custObj.id);
      setCustomer(custObj);
      setSummary(sumObj);
      localStorage.setItem('portal_id_no', targetIdNo);
      // Automatically select the first active loan if present
      if (custObj.loans && custObj.loans.length > 0) {
        const activeLoan = custObj.loans.find(l => l.status === 1) || custObj.loans[0];
        await loadLoanInstallments(activeLoan.id, activeLoan);
      } else {
        setSelectedLoan(null);
        setInstallments([]);
      }
    } catch (e) {
      setError(e.message || 'Giriş yapılamadı. T.C. Kimlik numaranızı kontrol ediniz.');
      setCustomer(null);
      setSummary(null);
      localStorage.removeItem('portal_id_no');
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    setCustomer(null);
    setSummary(null);
    setSelectedLoan(null);
    setInstallments([]);
    setIdNoInput('');
    setError('');
    setSuccess('');
    localStorage.removeItem('portal_id_no');
  };

  const loadLoanInstallments = async (loanId, loanObj) => {
    setInstLoading(true);
    setSelectedLoan(loanObj);
    try {
      const data = await getInstallmentsByLoan(loanId);
      setInstallments(data);
    } catch (e) {
      setError('Taksit planı yüklenemedi: ' + e.message);
    } finally {
      setInstLoading(false);
    }
  };

  const handleLoanSubmit = async (e) => {
    e.preventDefault();
    setSavingLoan(true);
    setError('');
    setSuccess('');
    try {
      await createLoan({
        customerId: customer.id,
        type: Number(loanForm.type),
        principalAmount: parseFloat(loanForm.principalAmount),
        interestRate: parseFloat(loanForm.interestRate),
        termInMonths: parseInt(loanForm.termInMonths),
      });
      setSuccess('Kredi başvurunuz başarıyla alınmış ve onaylanmıştır!');
      setShowLoanModal(false);
      setLoanForm(loanEmpty);
      // Reload customer details
      await login(customer.identityNumber);
    } catch (e) {
      setError(e.message);
    } finally {
      setSavingLoan(false);
    }
  };

  const handlePaySubmit = async (e) => {
    e.preventDefault();
    setPayingSaving(true);
    setError('');
    setSuccess('');
    try {
      await createPayment({
        installmentId: payingInst.id,
        amount: payingInst.amount
      });
      setSuccess(`Taksit #${payingInst.installmentNumber} ödemesi başarıyla gerçekleştirildi!`);
      setPayingInst(null);
      // Reload current loan installments and customer summary
      await login(customer.identityNumber);
      if (selectedLoan) {
        await loadLoanInstallments(selectedLoan.id, selectedLoan);
      }
    } catch (e) {
      setError(e.message);
    } finally {
      setPayingSaving(false);
    }
  };

  // Render Login Screen if not logged in
  if (!customer) {
    return (
      <div className="page" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: 'calc(100svh - 64px)' }}>
        <div className="card" style={{ maxWidth: 420, width: '100%', padding: '40px 32px', textAlign: 'center', position: 'relative', overflow: 'hidden' }}>
          {/* Subtle gradient background decoration */}
          <div style={{ position: 'absolute', top: -50, left: -50, width: 150, height: 150, background: 'var(--blue)', opacity: 0.15, filter: 'blur(50px)', borderRadius: '50%' }} />
          <div style={{ position: 'absolute', bottom: -50, right: -50, width: 150, height: 150, background: 'var(--purple)', opacity: 0.15, filter: 'blur(50px)', borderRadius: '50%' }} />

          <div style={{ fontSize: 40, marginBottom: 16 }}>🛡️</div>
          <h2 style={{ fontSize: 22, fontWeight: 700, color: 'var(--text-h)', marginBottom: 8 }}>Müşteri Portali Girişi</h2>
          <p style={{ fontSize: 13, color: 'var(--text-dim)', marginBottom: 28 }}>
            Kredilerinizi, taksit planlarınızı ve ödemelerinizi görüntülemek için giriş yapın.
          </p>

          {error && <div className="alert alert-error" style={{ marginBottom: 20, textAlign: 'left' }}>{error}</div>}

          <form onSubmit={(e) => { e.preventDefault(); login(); }} className="form">
            <div className="form-group" style={{ textAlign: 'left' }}>
              <label>T.C. Kimlik Numarası</label>
              <input
                type="text"
                maxLength={11}
                placeholder="11 haneli T.C. Kimlik No"
                value={idNoInput}
                onChange={(e) => setIdNoInput(e.target.value.replace(/\D/g, ''))}
                required
                style={{ fontSize: 15, letterSpacing: '1px', textAlign: 'center', padding: '12px' }}
              />
            </div>

            <button type="submit" id="btn-portal-login" className="btn btn-primary" style={{ width: '100%', justifyContent: 'center', padding: '12px', marginTop: 8 }} disabled={loading}>
              {loading ? 'Giriş Yapılıyor...' : 'Giriş Yap →'}
            </button>
          </form>

          <div style={{ marginTop: 24, fontSize: 11, color: 'var(--text-dim)', borderTop: '1px solid var(--border)', paddingTop: 16 }}>
            💡 <span style={{ color: 'var(--text)' }}>Test hesapları:</span> Ahmet Yılmaz (12345678901), Fatma Demir (23456789012), Mehmet Kaya (34567890123)
          </div>
        </div>
      </div>
    );
  }

  // Logged in Dashboard View
  return (
    <div className="page">
      {/* Top Bar with User Info */}
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', background: 'var(--bg-card)', border: '1px solid var(--border)', padding: '16px 24px', borderRadius: 'var(--radius)', marginBottom: 28 }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 16 }}>
          <div style={{ width: 48, height: 48, borderRadius: '50%', background: 'linear-gradient(135deg, var(--blue), var(--purple))', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 20, color: '#fff', fontWeight: 700 }}>
            {customer.firstName[0]}{customer.lastName[0]}
          </div>
          <div>
            <div style={{ fontSize: 18, fontWeight: 700, color: 'var(--text-h)', display: 'flex', alignItems: 'center', gap: 12 }}>
              Hoş Geldiniz, {customer.firstName} {customer.lastName}
              <span style={{ fontSize: 12, padding: '2px 8px', background: 'var(--bg-hover)', border: '1px solid var(--border)', borderRadius: 12, color: 'var(--blue)' }}>
                🎯 Kredi Skoru: <strong>{summary?.creditScore ?? customer.creditScore ?? 1200}</strong>
              </span>
              <span style={{ fontSize: 12, padding: '2px 8px', background: 'rgba(16, 185, 129, 0.1)', border: '1px solid rgba(16, 185, 129, 0.2)', borderRadius: 12, color: 'var(--green)' }}>
                💰 Bakiye: <strong>{formatMoney(customer.balance)}</strong>
              </span>
            </div>
            <div style={{ fontSize: 12, color: 'var(--text-dim)', marginTop: 2 }}>TC: {customer.identityNumber} · {customer.email}</div>
          </div>
        </div>
        <div style={{ display: 'flex', gap: 12 }}>
          <button id="btn-portal-new-loan" className="btn btn-primary" onClick={() => setShowLoanModal(true)}>
            💳 Yeni Kredi Başvurusu
          </button>
          <button id="btn-portal-logout" className="btn btn-ghost" onClick={logout}>
            Çıkış Yap
          </button>
        </div>
      </div>

      {error && <div className="alert alert-error" style={{ marginBottom: 20 }}>{error}</div>}
      {success && <div className="alert alert-success" style={{ marginBottom: 20 }}>{success}</div>}

      {/* Overview Metrics Cards */}
      {summary && (
        <div className="stats-grid" style={{ gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))' }}>
          <div className="card" style={{ borderLeft: '4px solid var(--red)' }}>
            <div className="card-title">Toplam Kredi Borcu</div>
            <div className="card-value red">{formatMoney(summary.totalLoanDebt)}</div>
            <div style={{ fontSize: 11, color: 'var(--text-dim)', marginTop: 4 }}>Tüm aktif kredilerin toplam tutarı</div>
          </div>
          <div className="card" style={{ borderLeft: '4px solid var(--amber)' }}>
            <div className="card-title">Kalan Anapara Bakiyesi</div>
            <div className="card-value amber">{formatMoney(summary.remainingPrincipal)}</div>
            <div style={{ fontSize: 11, color: 'var(--text-dim)', marginTop: 4 }}>Ödenmemiş taksitlerin anapara karşılığı</div>
          </div>
          <div className="card" style={{ borderLeft: `4px solid ${summary.delayedInstallmentCount > 0 ? 'var(--red)' : 'var(--green)'}` }}>
            <div className="card-title">Gecikmiş Taksit Sayısı</div>
            <div className={`card-value ${summary.delayedInstallmentCount > 0 ? 'red' : 'green'}`}>{summary.delayedInstallmentCount}</div>
            <div style={{ fontSize: 11, color: 'var(--text-dim)', marginTop: 4 }}>
              {summary.delayedInstallmentCount > 0 ? '⚠️ Son ödeme tarihi geçmiş taksitler' : '🎉 Tüm ödemeleriniz düzenli'}
            </div>
          </div>
        </div>
      )}

      {/* Main Content Split: Left side Loans List, Right side Selected Loan Installments */}
      <div style={{ display: 'grid', gridTemplateColumns: '320px 1fr', gap: 24, alignItems: 'start' }}>
        {/* Loans Sidebar List */}
        <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
          <div style={{ fontSize: 13, fontWeight: 600, color: 'var(--text-h)', textTransform: 'uppercase', letterSpacing: '0.5px', paddingLeft: 4 }}>
            Kredilerim ({customer.loans?.length || 0})
          </div>

          {!customer.loans || customer.loans.length === 0 ? (
            <div className="card empty-state" style={{ padding: '30px 16px' }}>
              <p style={{ fontSize: 13 }}>Aktif krediniz bulunmuyor.</p>
            </div>
          ) : (
            customer.loans.map(loan => {
              const isSelected = selectedLoan?.id === loan.id;
              return (
                <div
                  key={loan.id}
                  onClick={() => loadLoanInstallments(loan.id, loan)}
                  className="card"
                  style={{
                    padding: '16px',
                    cursor: 'pointer',
                    transition: 'all 0.2s',
                    borderColor: isSelected ? 'var(--blue)' : 'var(--border)',
                    background: isSelected ? 'var(--bg-hover)' : 'var(--bg-card)',
                    position: 'relative'
                  }}
                >
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 8 }}>
                    <span style={{ fontSize: 14, fontWeight: 700, color: 'var(--text-h)' }}>
                      {loanTypeLabel[loan.type]} Kredisi <span style={{ color: 'var(--blue)', fontSize: 13 }}>#{loan.id}</span>
                    </span>
                    <StatusBadge status={loan.status} type="loan" />
                  </div>
                  <div style={{ fontSize: 18, fontWeight: 700, color: 'var(--text-h)', marginBottom: 2 }}>
                    {formatMoney(loan.principalAmount)}
                  </div>
                  <div style={{ fontSize: 12, color: 'var(--blue)', fontWeight: 600, marginBottom: 8 }}>
                    Toplam: {formatMoney(loan.totalAmount)}
                  </div>
                  <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: 12, color: 'var(--text-dim)' }}>
                    <span>Vade: {loan.termInMonths} Ay</span>
                    <span>Faiz: %{loan.interestRate}</span>
                  </div>
                </div>
              );
            })
          )}
        </div>

        {/* Selected Loan Installment Plan View */}
        <div className="table-wrap">
          {selectedLoan ? (
            <>
              <div className="table-header" style={{ background: 'var(--bg-hover)', borderBottom: '1px solid var(--border)' }}>
                <div>
                  <h2 style={{ fontSize: 16, color: 'var(--text-h)' }}>{loanTypeLabel[selectedLoan.type]} Kredisi Detayı</h2>
                  <div style={{ fontSize: 12, color: 'var(--text-dim)', marginTop: 2 }}>
                    Kredi ID: #{selectedLoan.id} · Başlangıç: {formatDate(selectedLoan.startDate)}
                  </div>
                </div>
                <div style={{ textAlign: 'right' }}>
                  <div style={{ marginBottom: 4 }}>
                    <span style={{ fontSize: 11, color: 'var(--text-dim)', marginRight: 8 }}>Toplam Geri Ödeme:</span>
                    <span style={{ fontSize: 14, fontWeight: 700, color: 'var(--blue)' }}>{formatMoney(selectedLoan.totalAmount)}</span>
                  </div>
                  <span style={{ fontSize: 12, color: 'var(--text-dim)', display: 'block' }}>Kredi Durumu</span>
                  <StatusBadge status={selectedLoan.status} type="loan" />
                </div>
              </div>

              {instLoading ? (
                <div className="spinner" style={{ margin: '80px auto' }} />
              ) : installments.length === 0 ? (
                <div className="empty-state"><p>Taksit planı bulunamadı.</p></div>
              ) : (
                <table>
                  <thead>
                    <tr>
                      <th>Taksit</th>
                      <th>Tutar</th>
                      <th>Son Ödeme Tarihi</th>
                      <th>Durum</th>
                      <th style={{ textAlign: 'right' }}>İşlem</th>
                    </tr>
                  </thead>
                  <tbody>
                    {installments.map(inst => {
                      const isOverdue = inst.status === 3;
                      const isUnpaid = inst.status === 2;
                      const isPaid = inst.status === 1;

                      return (
                        <tr key={inst.id} style={{ background: isOverdue ? 'rgba(239, 68, 68, 0.04)' : undefined }}>
                          <td className="td-bold">#{inst.installmentNumber}</td>
                          <td className="td-money">{formatMoney(inst.amount)}</td>
                          <td className="td-dim">{formatDate(inst.dueDate)}</td>
                          <td>
                            <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                              <StatusBadge status={inst.status} />
                              {isPaid && inst.payment?.paymentDate && (
                                <span style={{ fontSize: 11, color: 'var(--text-dim)' }}>
                                  ({formatDate(inst.payment.paymentDate)})
                                </span>
                              )}
                            </div>
                          </td>
                          <td style={{ textAlign: 'right' }}>
                            {isPaid ? (
                              <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'flex-end' }}>
                                <span style={{ fontSize: 12, color: 'var(--green)', fontWeight: 600 }}>✓ Ödendi</span>
                                {inst.payment?.paymentDate && (
                                  <span style={{ fontSize: 10, color: 'var(--text-dim)' }}>{formatDate(inst.payment.paymentDate)}</span>
                                )}
                              </div>
                            ) : selectedLoan.status === 1 ? (
                              <button
                                id={`btn-portal-pay-${inst.id}`}
                                className={`btn btn-sm ${isOverdue ? 'btn-danger' : 'btn-primary'}`}
                                onClick={() => setPayingInst(inst)}
                              >
                                {isOverdue ? 'Hemen Öde' : 'Öde'}
                              </button>
                            ) : (
                              <span style={{ fontSize: 12, color: 'var(--text-dim)' }}>Kredi Kapalı</span>
                            )}
                          </td>
                        </tr>
                      );
                    })}
                  </tbody>
                </table>
              )}
            </>
          ) : (
            <div className="empty-state" style={{ padding: '80px 20px' }}>
              <div className="icon">👈</div>
              <p style={{ fontSize: 15, color: 'var(--text-h)', fontWeight: 500, marginBottom: 4 }}>Kredi Seçiniz</p>
              <p style={{ fontSize: 13 }}>Taksit planını ve ödeme durumunu görmek için sol taraftan bir kredi seçin.</p>
            </div>
          )}
        </div>
      </div>

      {/* New Loan Application Modal */}
      {showLoanModal && (
        <div className="modal-overlay" onClick={e => e.target === e.currentTarget && setShowLoanModal(false)}>
          <div className="modal">
            <div className="modal-header">
              <h2>Yeni Kredi Başvurusu</h2>
              <button className="btn btn-ghost btn-sm btn-icon" onClick={() => setShowLoanModal(false)}>✕</button>
            </div>
            <form onSubmit={handleLoanSubmit}>
              <div className="modal-body">
                <p style={{ fontSize: 13, color: 'var(--text-dim)', marginBottom: 16 }}>
                  İhtiyaçlarınıza uygun kredi türünü, tutarını ve vadesini seçerek anında başvuru yapabilirsiniz.
                </p>

                <div className="form">
                  <div className="form-group">
                    <label>Kredi Türü</label>
                    <select
                      name="type"
                      value={loanForm.type}
                      onChange={e => {
                        const val = Number(e.target.value);
                        const config = loanTypes.find(t => t.value === val);
                        setLoanForm({
                          ...loanForm,
                          type: val,
                          interestRate: config ? config.interestRate : 18.0,
                          termInMonths: config ? config.termInMonths : 12
                        });
                      }}
                    >
                      {loanTypes.map(t => <option key={t.value} value={t.value}>{t.label} (Faiz: %{t.interestRate}, Vade: {t.termInMonths} Ay)</option>)}
                    </select>
                  </div>

                  <div className="form-row">
                    <div className="form-group">
                      <label>Kredi Tutarı (₺)</label>
                      <input
                        type="number"
                        placeholder="Örn: 50000"
                        value={loanForm.principalAmount}
                        onChange={e => setLoanForm({ ...loanForm, principalAmount: e.target.value })}
                        required
                        min="1000"
                        step="500"
                      />
                    </div>
                    <div className="form-group">
                      <label style={{ display: 'flex', justifyContent: 'space-between' }}>
                        <span>Yıllık Faiz Oranı (%)</span>
                        <span style={{ fontSize: 10, color: 'var(--blue)', fontWeight: 600 }}>Kategori Standardı</span>
                      </label>
                      <input
                        type="number"
                        value={loanForm.interestRate}
                        readOnly
                        style={{ background: 'var(--bg-input)', opacity: 0.85, cursor: 'not-allowed' }}
                      />
                    </div>
                  </div>

                  <div className="form-group">
                    <label style={{ display: 'flex', justifyContent: 'space-between' }}>
                      <span>Vade (Ay)</span>
                      <span style={{ fontSize: 10, color: 'var(--blue)', fontWeight: 600 }}>Sabit Vade</span>
                    </label>
                    <input
                      type="number"
                      value={loanForm.termInMonths}
                      readOnly
                      style={{ background: 'var(--bg-input)', opacity: 0.85, cursor: 'not-allowed' }}
                    />
                  </div>
                </div>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn-ghost" onClick={() => setShowLoanModal(false)}>İptal</button>
                <button type="submit" id="btn-submit-portal-loan" className="btn btn-primary" disabled={savingLoan}>
                  {savingLoan ? 'İşleniyor...' : 'Başvuruyu Tamamla'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* Payment Confirmation Modal */}
      {payingInst && (
        <div className="modal-overlay" onClick={e => e.target === e.currentTarget && setPayingInst(null)}>
          <div className="modal" style={{ maxWidth: 440 }}>
            <div className="modal-header">
              <h2>Taksit Ödeme Onayı</h2>
              <button className="btn btn-ghost btn-sm btn-icon" onClick={() => setPayingInst(null)}>✕</button>
            </div>
            <form onSubmit={handlePaySubmit}>
              <div className="modal-body" style={{ textAlign: 'center' }}>
                <div style={{ fontSize: 40, marginBottom: 12 }}>💳</div>
                <div style={{ fontSize: 14, color: 'var(--text-dim)', marginBottom: 4 }}>Ödenecek Taksit Tutarı</div>
                <div style={{ fontSize: 32, fontWeight: 700, color: 'var(--green)', marginBottom: 20 }}>
                  {formatMoney(payingInst.amount)}
                </div>

                <div style={{ background: 'var(--bg-input)', padding: '12px', borderRadius: 'var(--radius-sm)', textAlign: 'left', fontSize: 13, display: 'flex', flexDirection: 'column', gap: 6 }}>
                  <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                    <span style={{ color: 'var(--text-dim)' }}>Taksit No:</span>
                    <span style={{ color: 'var(--text-h)', fontWeight: 600 }}>#{payingInst.installmentNumber}</span>
                  </div>
                  <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                    <span style={{ color: 'var(--text-dim)' }}>Son Ödeme Tarihi:</span>
                    <span style={{ color: 'var(--text-h)', fontWeight: 600 }}>{formatDate(payingInst.dueDate)}</span>
                  </div>
                  <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                    <span style={{ color: 'var(--text-dim)' }}>Kredi Türü:</span>
                    <span style={{ color: 'var(--text-h)', fontWeight: 600 }}>{loanTypeLabel[selectedLoan?.type]}</span>
                  </div>
                </div>
              </div>
              <div className="modal-footer">
                <button type="button" className="btn btn-ghost" onClick={() => setPayingInst(null)}>Vazgeç</button>
                <button type="submit" id="btn-confirm-portal-pay" className="btn btn-primary" style={{ background: 'var(--green)' }} disabled={payingSaving}>
                  {payingSaving ? 'Ödeme Alınıyor...' : 'Ödemeyi Onayla'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
