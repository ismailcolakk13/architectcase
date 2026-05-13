import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getCustomers, createCustomer, updateCustomer, deleteCustomer } from '../api/client';

function Modal({ title, onClose, children }) {
  return (
    <div className="modal-overlay" onClick={e => e.target === e.currentTarget && onClose()}>
      <div className="modal">
        <div className="modal-header">
          <h2>{title}</h2>
          <button className="btn btn-ghost btn-sm btn-icon" onClick={onClose}>✕</button>
        </div>
        {children}
      </div>
    </div>
  );
}

const empty = { firstName:'', lastName:'', email:'', phone:'', address:'', identityNumber:'', creditScore: 1200 };

export default function CustomersPage() {
  const [customers, setCustomers] = useState([]);
  const [loading, setLoading]     = useState(true);
  const [error, setError]         = useState('');
  const [modal, setModal]         = useState(null); // 'create' | 'edit' | null
  const [editing, setEditing]     = useState(null);
  const [form, setForm]           = useState(empty);
  const [saving, setSaving]       = useState(false);
  const navigate = useNavigate();

  const load = async () => {
    try { setCustomers(await getCustomers()); }
    catch (e) { setError(e.message); }
    finally { setLoading(false); }
  };

  useEffect(() => { load(); }, []);

  const openCreate = () => { setForm(empty); setModal('create'); };
  const openEdit   = (c)  => {
    setEditing(c);
    setForm({ firstName:c.firstName, lastName:c.lastName, email:c.email, phone:c.phone, address:c.address, identityNumber:c.identityNumber, creditScore: c.creditScore ?? 1200 });
    setModal('edit');
  };
  const closeModal = () => { setModal(null); setEditing(null); setError(''); };

  const handleChange = e => setForm(f => ({ ...f, [e.target.name]: e.target.value }));

  const handleSubmit = async e => {
    e.preventDefault();
    setSaving(true);
    setError('');
    try {
      if (modal === 'create') {
        await createCustomer(form);
      } else {
        const { identityNumber: _, ...updateBody } = form;
        await updateCustomer(editing.id, updateBody);
      }
      await load();
      closeModal();
    } catch (e) { setError(e.message); }
    finally { setSaving(false); }
  };

  const handleDelete = async (id) => {
    if (!confirm('Müşteriyi silmek istediğinizden emin misiniz?')) return;
    try { await deleteCustomer(id); await load(); }
    catch (e) { alert(e.message); }
  };

  return (
    <div className="page">
      <div className="page-header">
        <div>
          <div className="page-title">Müşteriler</div>
          <div className="page-subtitle">Tüm bireysel müşterilerin listesi</div>
        </div>
        <button id="btn-create-customer" className="btn btn-primary" onClick={openCreate}>
          + Yeni Müşteri
        </button>
      </div>

      {error && !modal && <div className="alert alert-error" style={{marginBottom:16}}>{error}</div>}

      {loading ? (
        <div className="spinner" />
      ) : customers.length === 0 ? (
        <div className="empty-state"><div className="icon">👤</div><p>Henüz müşteri yok</p></div>
      ) : (
        <div className="table-wrap">
          <div className="table-header">
            <h2>Müşteri Listesi</h2>
            <span className="badge badge-blue">{customers.length} kayıt</span>
          </div>
          <table>
            <thead>
              <tr>
                <th>#</th>
                <th>Ad Soyad</th>
                <th>T.C. Kimlik</th>
                <th>E-posta</th>
                <th>Kredi Skoru</th>
                <th>İşlemler</th>
              </tr>
            </thead>
            <tbody>
              {customers.map(c => {
                const score = c.creditScore ?? 1200;
                return (
                  <tr key={c.id}>
                    <td className="td-dim">{c.id}</td>
                    <td className="td-bold"
                        style={{cursor:'pointer',color:'var(--blue)'}}
                        onClick={() => navigate(`/customers/${c.id}`)}>
                      {c.firstName} {c.lastName}
                    </td>
                    <td className="td-dim">{c.identityNumber}</td>
                    <td>{c.email}</td>
                    <td>
                      <span style={{ fontSize: 12, padding: '2px 8px', borderRadius: 12, fontWeight: 600, background: score < 1000 ? 'rgba(239, 68, 68, 0.1)' : score < 1400 ? 'rgba(245, 158, 11, 0.1)' : 'rgba(16, 185, 129, 0.1)', color: score < 1000 ? 'var(--red)' : score < 1400 ? 'var(--amber)' : 'var(--green)' }}>
                        {score}
                      </span>
                    </td>
                    <td>
                      <div className="action-row">
                        <button id={`btn-edit-customer-${c.id}`} className="btn btn-ghost btn-sm" onClick={() => openEdit(c)}>Düzenle</button>
                        <button id={`btn-delete-customer-${c.id}`} className="btn btn-danger btn-sm" onClick={() => handleDelete(c.id)}>Sil</button>
                      </div>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      )}

      {modal && (
        <Modal title={modal === 'create' ? 'Yeni Müşteri' : 'Müşteri Düzenle'} onClose={closeModal}>
          <form onSubmit={handleSubmit}>
            <div className="modal-body">
              {error && <div className="alert alert-error" style={{marginBottom:16}}>{error}</div>}
              <div className="form">
                <div className="form-row">
                  <div className="form-group">
                    <label>Ad</label>
                    <input name="firstName" value={form.firstName} onChange={handleChange} required />
                  </div>
                  <div className="form-group">
                    <label>Soyad</label>
                    <input name="lastName" value={form.lastName} onChange={handleChange} required />
                  </div>
                </div>
                <div className="form-group">
                  <label>E-posta</label>
                  <input type="email" name="email" value={form.email} onChange={handleChange} required />
                </div>
                <div className="form-row">
                  <div className="form-group">
                    <label>Telefon</label>
                    <input name="phone" value={form.phone} onChange={handleChange} required />
                  </div>
                  {modal === 'create' ? (
                    <div className="form-group">
                      <label>T.C. Kimlik No</label>
                      <input name="identityNumber" value={form.identityNumber} onChange={handleChange} required maxLength={11} />
                    </div>
                  ) : (
                    <div className="form-group">
                      <label>Kredi Skoru</label>
                      <input type="number" name="creditScore" value={form.creditScore} onChange={handleChange} required min="0" max="1900" />
                    </div>
                  )}
                </div>
                <div className="form-row">
                  <div className="form-group" style={{ flex: modal === 'create' ? 1 : undefined }}>
                    <label>Adres</label>
                    <input name="address" value={form.address} onChange={handleChange} />
                  </div>
                  {modal === 'create' && (
                    <div className="form-group">
                      <label>Kredi Skoru</label>
                      <input type="number" name="creditScore" value={form.creditScore} onChange={handleChange} required min="0" max="1900" />
                    </div>
                  )}
                </div>
              </div>
            </div>
            <div className="modal-footer">
              <button type="button" className="btn btn-ghost" onClick={closeModal}>İptal</button>
              <button type="submit" id="btn-save-customer" className="btn btn-primary" disabled={saving}>
                {saving ? 'Kaydediliyor…' : 'Kaydet'}
              </button>
            </div>
          </form>
        </Modal>
      )}
    </div>
  );
}
