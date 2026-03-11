import { NavLink, useNavigate } from 'react-router-dom'
import {
  LayoutDashboard, UtensilsCrossed, ShoppingBag,
  Users, Bell, LogOut, ChefHat, Menu, X
} from 'lucide-react'
import { useState } from 'react'
import type { UserResponse } from '../../types'

interface Props {
  user: UserResponse | null
  onLogout: () => void
  children: React.ReactNode
}

const navItems = [
  { to: '/',              icon: LayoutDashboard, label: 'Dashboard'    },
  { to: '/restaurants',   icon: UtensilsCrossed, label: 'Restaurante'  },
  { to: '/orders',        icon: ShoppingBag,     label: 'Comenzi'      },
  { to: '/users',         icon: Users,           label: 'Utilizatori'  },
  { to: '/notifications', icon: Bell,            label: 'Notificări'   },
]

export default function Layout({ user, onLogout, children }: Props) {
  const [open, setOpen] = useState(false)
  const navigate = useNavigate()

  const handleLogout = () => { onLogout(); navigate('/login') }

  return (
    <div className="flex h-screen overflow-hidden bg-[#0f0f13]">

      {/* Mobile overlay */}
      {open && (
        <div className="fixed inset-0 bg-black/60 z-20 lg:hidden" onClick={() => setOpen(false)} />
      )}

      {/* Sidebar */}
      <aside className={`
        fixed lg:static inset-y-0 left-0 z-30
        w-64 flex flex-col
        bg-[#13131a] border-r border-white/5
        transform transition-transform duration-300
        ${open ? 'translate-x-0' : '-translate-x-full lg:translate-x-0'}
      `}>
        {/* Logo */}
        <div className="flex items-center gap-3 px-6 py-5 border-b border-white/5">
          <div className="w-9 h-9 rounded-xl bg-brand-500 flex items-center justify-center">
            <ChefHat size={20} className="text-white" />
          </div>
          <div>
            <p className="font-display font-bold text-white text-sm leading-none">FoodDelivery</p>
            <p className="text-white/40 text-xs mt-0.5">Platform</p>
          </div>
        </div>

        {/* Nav */}
        <nav className="flex-1 px-3 py-4 space-y-1">
          {navItems.map(({ to, icon: Icon, label }) => (
            <NavLink
              key={to} to={to}
              onClick={() => setOpen(false)}
              className={({ isActive }) => `
                flex items-center gap-3 px-3 py-2.5 rounded-xl
                text-sm font-medium transition-all duration-200
                ${isActive
                  ? 'bg-brand-500/15 text-brand-400 border border-brand-500/20'
                  : 'text-white/50 hover:text-white hover:bg-white/5'}
              `}
            >
              <Icon size={18} />
              {label}
            </NavLink>
          ))}
        </nav>

        {/* User info + logout */}
        <div className="p-4 border-t border-white/5">
          {user ? (
            <div className="flex items-center gap-3">
              <div className="w-8 h-8 rounded-full bg-brand-500/20 border border-brand-500/30
                              flex items-center justify-center text-brand-400 font-bold text-sm">
                {user.name.charAt(0).toUpperCase()}
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-white text-sm font-medium truncate">{user.name}</p>
                <p className="text-white/40 text-xs">{user.role}</p>
              </div>
              <button onClick={handleLogout}
                className="p-1.5 rounded-lg text-white/30 hover:text-red-400 hover:bg-red-500/10 transition-all">
                <LogOut size={15} />
              </button>
            </div>
          ) : (
            <button onClick={() => navigate('/login')}
              className="w-full btn-primary text-sm py-2">
              Autentificare
            </button>
          )}
        </div>
      </aside>

      {/* Main */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Top bar mobile */}
        <header className="lg:hidden flex items-center justify-between px-4 py-3
                           border-b border-white/5 bg-[#13131a]">
          <button onClick={() => setOpen(true)} className="text-white/60 hover:text-white">
            <Menu size={22} />
          </button>
          <span className="font-display font-bold text-white text-sm">FoodDelivery</span>
          <div className="w-8" />
        </header>

        {/* Page content */}
        <main className="flex-1 overflow-y-auto">
          <div className="animate-fade-in">
            {children}
          </div>
        </main>
      </div>
    </div>
  )
}
