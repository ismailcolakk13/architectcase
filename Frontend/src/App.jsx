import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Sidebar from './components/Sidebar';
import CustomersPage from './pages/CustomersPage';
import CustomerDetailPage from './pages/CustomerDetailPage';
import LoansPage from './pages/LoansPage';
import LoanDetailPage from './pages/LoanDetailPage';
import CustomerPortalPage from './pages/CustomerPortalPage';

export default function App() {
  return (
    <BrowserRouter>
      <div className="layout">
        <Sidebar />
        <main className="main">
          <Routes>
            <Route path="/"                    element={<Navigate to="/portal" replace />} />
            <Route path="/portal"              element={<CustomerPortalPage />} />
            <Route path="/customers"           element={<CustomersPage />} />
            <Route path="/customers/:id"       element={<CustomerDetailPage />} />
            <Route path="/loans"               element={<LoansPage />} />
            <Route path="/loans/:id"           element={<LoanDetailPage />} />
            <Route path="*"                    element={<Navigate to="/portal" replace />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}
