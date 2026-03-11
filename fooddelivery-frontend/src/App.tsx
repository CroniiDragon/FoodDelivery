import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { Toaster } from 'react-hot-toast'
import { useAuth } from './hooks/useAuth'
import Layout from './components/layout/Layout'
import DashboardPage      from './pages/DashboardPage'
import RestaurantsPage    from './pages/RestaurantsPage'
import OrdersPage         from './pages/OrdersPage'
import UsersPage          from './pages/UsersPage'
import NotificationsPage  from './pages/NotificationsPage'
import LoginPage          from './pages/LoginPage'

export default function App() {
  const { user, login, logout } = useAuth()

  return (
    <BrowserRouter>
      <Toaster
        position="top-right"
        toastOptions={{
          style: { background: '#1e1e2a', color: '#f1f1f3', border: '1px solid rgba(255,255,255,0.1)' },
          success: { iconTheme: { primary: '#f97316', secondary: '#fff' } },
        }}
      />

      <Routes>
        {/* Login page - fara layout */}
        <Route path="/login" element={<LoginPage onLogin={login} />} />

        {/* Toate celelalte pagini au layout-ul cu sidebar */}
        <Route path="/*" element={
          <Layout user={user} onLogout={logout}>
            <Routes>
              <Route path="/"              element={<DashboardPage />} />
              <Route path="/restaurants"   element={<RestaurantsPage />} />
              <Route path="/orders"        element={<OrdersPage />} />
              <Route path="/users"         element={<UsersPage />} />
              <Route path="/notifications" element={<NotificationsPage />} />
              <Route path="*"              element={<Navigate to="/" replace />} />
            </Routes>
          </Layout>
        } />
      </Routes>
    </BrowserRouter>
  )
}
