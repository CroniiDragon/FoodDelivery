import { useEffect, useState } from 'react'
import { UtensilsCrossed, ShoppingBag, Users, Bell, TrendingUp, Clock } from 'lucide-react'
import { StatCard, LoadingSpinner } from '../components/ui'
import { restaurantApi, orderApi, userApi, notificationApi } from '../services/api'

export default function DashboardPage() {
  const [stats, setStats] = useState({
    restaurants: 0, orders: 0, users: 0, notifications: 0
  })
  const [loading, setLoading] = useState(true)
  const [recentOrders, setRecentOrders] = useState<any[]>([])

  useEffect(() => {
    const load = async () => {
      try {
        const [restaurants, users, notifications] = await Promise.allSettled([
          restaurantApi.getAll(),
          userApi.getAll(),
          notificationApi.getByRecipient(1)
        ])
        setStats(s => ({
          ...s,
          restaurants: restaurants.status === 'fulfilled' ? restaurants.value.length : 0,
          users:       users.status === 'fulfilled'       ? users.value.length       : 0,
          notifications: notifications.status === 'fulfilled' ? notifications.value.length : 0,
          orders: 0,
        }))
      } catch (e) {
        console.error(e)
      } finally {
        setLoading(false)
      }
    }
    load()
  }, [])

  if (loading) return <LoadingSpinner text="Se încarcă dashboard-ul..." />

  return (
    <div className="page-container">
      {/* Header */}
      <div className="mb-8 animate-slide-up">
        <h1 className="font-display text-3xl font-bold text-white">Dashboard</h1>
        <p className="text-white/40 mt-1">Bun venit! Iată o privire de ansamblu a platformei.</p>
      </div>

      {/* Stats grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        <StatCard label="Restaurante"  value={stats.restaurants}  icon={UtensilsCrossed} color="bg-brand-500"      sub="active pe platformă" />
        <StatCard label="Comenzi"      value={stats.orders}       icon={ShoppingBag}     color="bg-blue-500"       sub="total plasate" />
        <StatCard label="Utilizatori"  value={stats.users}        icon={Users}           color="bg-purple-500"     sub="clienți + curieri" />
        <StatCard label="Notificări"   value={stats.notifications} icon={Bell}           color="bg-green-500"      sub="trimise astăzi" />
      </div>

      {/* Info cards */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Services status */}
        <div className="card p-6 animate-slide-up" style={{ animationDelay: '100ms' }}>
          <h2 className="font-display text-lg font-bold text-white mb-4 flex items-center gap-2">
            <TrendingUp size={20} className="text-brand-400" />
            Status Microservicii
          </h2>
          <div className="space-y-3">
            {[
              { name: 'OrderService',        port: '5001', color: 'bg-blue-500'   },
              { name: 'RestaurantService',    port: '5002', color: 'bg-brand-500'  },
              { name: 'UserService',          port: '5003', color: 'bg-purple-500' },
              { name: 'NotificationService',  port: '5004', color: 'bg-green-500'  },
            ].map(svc => (
              <div key={svc.name} className="flex items-center justify-between
                                             bg-white/3 rounded-xl px-4 py-3 border border-white/5">
                <div className="flex items-center gap-3">
                  <div className={`w-2 h-2 rounded-full ${svc.color} animate-pulse-slow`} />
                  <span className="text-white text-sm font-medium font-mono">{svc.name}</span>
                </div>
                <span className="text-white/30 text-xs font-mono">:{svc.port}</span>
              </div>
            ))}
          </div>
        </div>

        {/* Quick actions */}
        <div className="card p-6 animate-slide-up" style={{ animationDelay: '200ms' }}>
          <h2 className="font-display text-lg font-bold text-white mb-4 flex items-center gap-2">
            <Clock size={20} className="text-brand-400" />
            Acțiuni Rapide
          </h2>
          <div className="grid grid-cols-2 gap-3">
            {[
              { label: 'Adaugă Restaurant', href: '/restaurants', icon: UtensilsCrossed, color: 'text-brand-400' },
              { label: 'Comandă Nouă',      href: '/orders',       icon: ShoppingBag,    color: 'text-blue-400'  },
              { label: 'Utilizator Nou',    href: '/users',        icon: Users,          color: 'text-purple-400'},
              { label: 'Trimite Notificare',href: '/notifications',icon: Bell,           color: 'text-green-400' },
            ].map(action => (
              <a key={action.href} href={action.href}
                className="flex flex-col items-center gap-2 p-4 rounded-xl
                           bg-white/3 border border-white/5 hover:border-white/15
                           hover:bg-white/6 transition-all duration-200 group">
                <action.icon size={22} className={`${action.color} group-hover:scale-110 transition-transform`} />
                <span className="text-white/60 text-xs text-center font-medium">{action.label}</span>
              </a>
            ))}
          </div>
        </div>
      </div>
    </div>
  )
}
