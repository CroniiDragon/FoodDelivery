import { useEffect, useState } from 'react'
import { Plus, ShoppingBag, MapPin, CreditCard } from 'lucide-react'
import toast from 'react-hot-toast'
import { orderApi, restaurantApi, userApi } from '../services/api'
import { LoadingSpinner, EmptyState, Modal, OrderStatusBadge } from '../components/ui'
import type { OrderResponse, RestaurantResponse, MenuItemResponse, UserResponse } from '../types'

export default function OrdersPage() {
  const [orders, setOrders]           = useState<OrderResponse[]>([])
  const [loading, setLoading]         = useState(false)
  const [showNew, setShowNew]         = useState(false)
  const [customerId, setCustomerId]   = useState<string>('')
  const [customers, setCustomers]     = useState<UserResponse[]>([])
  const [restaurants, setRestaurants] = useState<RestaurantResponse[]>([])
  const [selRest, setSelRest]         = useState<string>('')
  const [menu, setMenu]               = useState<MenuItemResponse[]>([])
  const [cart, setCart]               = useState<Record<number,number>>({})
  const [address, setAddress]         = useState('')
  const [payment, setPayment]         = useState('Card')

  useEffect(() => {
    restaurantApi.getAll().then(setRestaurants).catch(() => {})
    userApi.getAll().then(u => setCustomers(u.filter(x => x.role === 'Customer'))).catch(() => {})
  }, [])

  useEffect(() => {
    if (!selRest) return
    restaurantApi.getMenu(Number(selRest)).then(setMenu).catch(() => {})
    setCart({})
  }, [selRest])

  const loadOrders = async (cid: string) => {
    if (!cid) return
    setLoading(true)
    try { setOrders(await orderApi.getByCustomer(Number(cid))) }
    catch { toast.error('Eroare la încărcarea comenzilor.') }
    finally { setLoading(false) }
  }

  const addToCart = (itemId: number) =>
    setCart(p => ({ ...p, [itemId]: (p[itemId] ?? 0) + 1 }))

  const removeFromCart = (itemId: number) =>
    setCart(p => { const n = { ...p }; if (n[itemId] > 1) n[itemId]--; else delete n[itemId]; return n })

  const handlePlaceOrder = async (e: React.FormEvent) => {
    e.preventDefault()
    const items = Object.entries(cart).map(([id, qty]) => {
      const item = menu.find(m => m.id === Number(id))!
      return { menuItemId: Number(id), itemName: item.name, quantity: qty, unitPrice: item.finalPrice }
    })
    if (items.length === 0) { toast.error('Adaugă cel puțin un produs.'); return }
    try {
      const order = await orderApi.create({
        customerId: Number(customerId), restaurantId: Number(selRest),
        deliveryAddress: address, paymentMethod: payment, items
      })
      setOrders(p => [order, ...p])
      setShowNew(false)
      setCart({})
      toast.success(`Comanda #${order.id} plasată cu succes!`)
    } catch { toast.error('Eroare la plasarea comenzii.') }
  }

  const handleCancel = async (id: number) => {
    try {
      await orderApi.cancel(id)
      setOrders(p => p.map(o => o.id === id ? { ...o, status: 'Cancelled' } : o))
      toast.success('Comanda anulată.')
    } catch { toast.error('Comanda nu poate fi anulată.') }
  }

  const cartTotal = Object.entries(cart).reduce((sum, [id, qty]) => {
    const item = menu.find(m => m.id === Number(id))
    return sum + (item ? item.finalPrice * qty : 0)
  }, 0)

  return (
    <div className="page-container">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="section-title mb-0">Comenzi</h1>
          <p className="text-white/40 text-sm mt-1">Plasează și urmărește comenzile</p>
        </div>
        <button onClick={() => setShowNew(true)} className="btn-primary flex items-center gap-2">
          <Plus size={16} /> Comandă Nouă
        </button>
      </div>

      {/* Filter by customer */}
      <div className="card p-4 mb-6 flex items-center gap-4">
        <label className="text-white/60 text-sm whitespace-nowrap">Comenzile clientului:</label>
        <select className="input max-w-xs" value={customerId}
          onChange={e => { setCustomerId(e.target.value); loadOrders(e.target.value) }}>
          <option value="">— selectează client —</option>
          {customers.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
        </select>
      </div>

      {/* Orders list */}
      {loading ? <LoadingSpinner /> : orders.length === 0
        ? <EmptyState title="Nicio comandă" sub="Selectează un client sau plasează o comandă nouă." />
        : <div className="space-y-4">
            {orders.map(o => (
              <div key={o.id} className="card p-5 hover:border-white/20 transition-all animate-slide-up">
                <div className="flex items-start justify-between mb-3">
                  <div>
                    <div className="flex items-center gap-3">
                      <ShoppingBag size={18} className="text-brand-400" />
                      <span className="font-display font-bold text-white">Comanda #{o.id}</span>
                      <OrderStatusBadge status={o.status} />
                    </div>
                    <div className="flex items-center gap-4 mt-2 text-white/40 text-xs">
                      <span className="flex items-center gap-1"><MapPin size={11} />{o.deliveryAddress}</span>
                      <span className="flex items-center gap-1"><CreditCard size={11} />{o.paymentMethod}</span>
                    </div>
                  </div>
                  <div className="text-right">
                    <p className="text-brand-400 font-bold font-display">{o.totalPrice.toFixed(2)} LEI</p>
                    <p className="text-white/30 text-xs mt-1">{new Date(o.createdAt).toLocaleDateString('ro-RO')}</p>
                  </div>
                </div>
                <div className="border-t border-white/5 pt-3 mt-3">
                  <div className="space-y-1">
                    {o.items.map((item, i) => (
                      <div key={i} className="flex justify-between text-sm">
                        <span className="text-white/60">{item.quantity}x {item.itemName}</span>
                        <span className="text-white/40">{item.totalPrice.toFixed(2)} LEI</span>
                      </div>
                    ))}
                  </div>
                  {(o.status === 'Pending' || o.status === 'Confirmed') && (
                    <button onClick={() => handleCancel(o.id)}
                      className="btn-danger text-xs py-1.5 px-3 mt-3">
                      Anulează
                    </button>
                  )}
                </div>
              </div>
            ))}
          </div>
      }

      {/* Modal comandă nouă */}
      {showNew && (
        <Modal title="Comandă Nouă" onClose={() => setShowNew(false)}>
          <form onSubmit={handlePlaceOrder} className="space-y-4 max-h-[75vh] overflow-y-auto pr-1">
            <div>
              <label className="block text-white/60 text-sm mb-1.5">Client</label>
              <select className="input" required value={customerId}
                onChange={e => setCustomerId(e.target.value)}>
                <option value="">— selectează —</option>
                {customers.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
              </select>
            </div>
            <div>
              <label className="block text-white/60 text-sm mb-1.5">Restaurant</label>
              <select className="input" required value={selRest}
                onChange={e => setSelRest(e.target.value)}>
                <option value="">— selectează —</option>
                {restaurants.map(r => <option key={r.id} value={r.id}>{r.name}</option>)}
              </select>
            </div>

            {/* Meniu + coș */}
            {menu.length > 0 && (
              <div>
                <label className="block text-white/60 text-sm mb-2">Produse</label>
                <div className="space-y-2 max-h-48 overflow-y-auto">
                  {menu.filter(m => m.isAvailable).map(item => (
                    <div key={item.id} className="flex items-center justify-between
                                                   bg-white/3 rounded-xl px-3 py-2 border border-white/5">
                      <div>
                        <p className="text-white text-sm">{item.name}</p>
                        <p className="text-brand-400 text-xs">{item.finalPrice.toFixed(2)} LEI</p>
                      </div>
                      <div className="flex items-center gap-2">
                        <button type="button" onClick={() => removeFromCart(item.id)}
                          className="w-6 h-6 rounded-full bg-white/10 text-white text-sm flex items-center justify-center hover:bg-white/20">−</button>
                        <span className="text-white text-sm w-4 text-center">{cart[item.id] ?? 0}</span>
                        <button type="button" onClick={() => addToCart(item.id)}
                          className="w-6 h-6 rounded-full bg-brand-500 text-white text-sm flex items-center justify-center hover:bg-brand-600">+</button>
                      </div>
                    </div>
                  ))}
                </div>
                {Object.keys(cart).length > 0 && (
                  <div className="bg-brand-500/10 border border-brand-500/20 rounded-xl px-3 py-2 mt-2">
                    <p className="text-brand-400 text-sm font-semibold">Total coș: {cartTotal.toFixed(2)} LEI + 10 LEI livrare</p>
                  </div>
                )}
              </div>
            )}

            <div>
              <label className="block text-white/60 text-sm mb-1.5">Adresă livrare</label>
              <input type="text" required className="input" value={address}
                onChange={e => setAddress(e.target.value)} placeholder="Str. Exemplu 10, Cluj" />
            </div>
            <div>
              <label className="block text-white/60 text-sm mb-1.5">Metodă plată</label>
              <select className="input" value={payment} onChange={e => setPayment(e.target.value)}>
                <option>Card</option>
                <option>Numerar</option>
                <option>Online</option>
              </select>
            </div>
            <div className="flex gap-3 pt-2">
              <button type="button" onClick={() => setShowNew(false)} className="btn-secondary flex-1">Anulează</button>
              <button type="submit" className="btn-primary flex-1">Plasează Comanda</button>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}
