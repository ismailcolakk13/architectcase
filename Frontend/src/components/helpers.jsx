// Loan type & status helpers
export const loanTypeLabel = { 1:'İhtiyaç', 2:'Eğitim', 3:'Taşıt', 4:'Konut', 5:'İşletme' };
export const loanStatusLabel = { 0:'Onay Bekliyor', 1:'Aktif', 2:'Kapatıldı', 3:'Reddedildi' };
export const installmentStatusLabel = { 1:'Ödendi', 2:'Ödenmedi', 3:'Gecikmiş' };

export function StatusBadge({ status, type = 'installment' }) {
  if (type === 'loan') {
    if (status === 0) return <span className="badge badge-blue">Onay Bekliyor</span>;
    if (status === 1) return <span className="badge badge-green">Aktif</span>;
    if (status === 3) return <span className="badge badge-red">Reddedildi</span>;
    return <span className="badge badge-gray">Kapatıldı</span>;
  }
  if (status === 1) return <span className="badge badge-green">Ödendi</span>;
  if (status === 3) return <span className="badge badge-red">Gecikmiş</span>;
  return <span className="badge badge-amber">Ödenmedi</span>;
}

export function formatMoney(v) {
  return new Intl.NumberFormat('tr-TR', { style: 'currency', currency: 'TRY' }).format(v);
}

export function formatDate(d) {
  if (!d) return '—';
  return new Date(d).toLocaleDateString('tr-TR');
}
