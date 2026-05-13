const BASE = '/api';

async function req(path, options = {}) {
  const res = await fetch(`${BASE}${path}`, {
    headers: { 'Content-Type': 'application/json', ...options.headers },
    ...options,
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({ message: 'Sunucu hatası' }));
    throw new Error(err.message || err.Message || 'İşlem başarısız');
  }
  const text = await res.text();
  return text ? JSON.parse(text) : null;
}

// ── Customers ─────────────────────────────────────────────
export const getCustomers    = ()      => req('/customers');
export const getCustomer     = (id)    => req(`/customers/${id}`);
export const getCustomerSum  = (id)    => req(`/customers/${id}/summary`);
export const getCustomerByIdentity = (idNo) => req(`/customers/by-identity/${idNo}`);
export const createCustomer  = (body)  => req('/customers', { method: 'POST', body: JSON.stringify(body) });
export const updateCustomer  = (id, b) => req(`/customers/${id}`, { method: 'PUT',  body: JSON.stringify(b) });
export const deleteCustomer  = (id)    => req(`/customers/${id}`, { method: 'DELETE' });

// ── Loans ─────────────────────────────────────────────────
export const getLoans           = ()     => req('/loans');
export const getLoan            = (id)   => req(`/loans/${id}`);
export const getLoansByCustomer = (cid)  => req(`/loans?customerId=${cid}`);
export const createLoan         = (body) => req('/loans', { method: 'POST', body: JSON.stringify(body) });
export const updateLoanStatus   = (id, status) => req(`/loans/${id}`, { method: 'PUT', body: JSON.stringify({ status }) });

// ── Installments ──────────────────────────────────────────
export const getInstallmentsByLoan     = (loanId)     => req(`/installments/by-loan/${loanId}`);
export const getInstallmentsByCustomer = (customerId) => req(`/installments/by-customer/${customerId}`);

// ── Payments ──────────────────────────────────────────────
export const getPayments    = ()     => req('/payments');
export const createPayment  = (body) => req('/payments', { method: 'POST', body: JSON.stringify(body) });
