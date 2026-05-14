import { useState, useEffect } from 'react'
import { orderApi, restaurantApi } from '../services/api'
import type { OrderResponse, DeliveryStrategy, RestaurantResponse, MenuItemResponse, CreateOrderDto } from '../types'

const STATUS_COLORS: Record<string, string> = {
  Pending:        'bg-yellow-500/20 text-yellow-300',
  Confirmed:      'bg-blue-500/20 text-blue-300',
  Preparing:      'bg-orange-500/20 text-orange-300',
  OutForDelivery: 'bg-purple-500/20 text-purple-300',
  Delivered:      'bg-green-500/20 text-green-300',
  Cancelled:      'bg-red-500/20 text-red-300',
}

const NEXT_STATUS: Record<string, string> = {
  Pending:        'Confirmed',
  Confirmed:      'Preparing',
  Preparing:      'OutForDelivery',
  OutForDelivery: 'Delivered',
}

const STRATEGIES: DeliveryStrategy[] = ['Standard', 'Express', 'CityBased']

interface CartItem {
  menuItemId: number
  itemName: string
  quantity: number
  unitPrice: number
}

export default function OrdersPage() {
  const [orders, setOrders]         = useState<OrderResponse[]>([])
  const [loading, setLoading]       = useState(false)
  const [strategy, setStrategy]     = useState<DeliveryStrategy>('Standard')
  const [customerId, setCustomerId] = useState(1)
  const [error, setError]           = useState('')
  const [showForm, setShowForm]     = useState(false)

  // Form state
  const [restaurants, setRestaurants]   = useState<RestaurantResponse[]>([])
  const [menu, setMenu]                 = useState<MenuItemResponse[]>([])
  const [selectedRest, setSelectedRest] = useState<number>(0)
  const [cart, setCart]                 = useState<CartItem[]>([])
  const [address, setAddress]           = useState('')
  const [payment, setPayment]           = useState('Cash')
  const [notes, setNotes]               = useState('')
  const [saving, setSaving]             = useState(false)

  useEffect(() => { fetchOrders() }, [customerId])

  async function fetchOrders() {
    setLoading(true)
    setError('')
    try { setOrders(await orderApi.getByCustomer(customerId)) }
    catch { setError('Could not load orders.') }
    finally { setLoading(false) }
  }

  async function openForm() {
    setShowForm(true)
    setCart([])
    setAddress('')
    setPayment('Cash')
    setNotes('')
    setSelectedRest(0)
    setMenu([])
    try { setRestaurants(await restaurantApi.getAll()) }
    catch { setError('Could not load restaurants.') }
  }

  async function handleSelectRestaurant(id: number) {
    setSelectedRest(id)
    setCart([])
    try { setMenu(await restaurantApi.getMenu(id)) }
    catch { setMenu([]) }
  }

  function addToCart(item: MenuItemResponse) {
    setCart(prev => {
      const existing = prev.find(c => c.menuItemId === item.id)
      if (existing)
        return prev.map(c => c.menuItemId === item.id ? { ...c, quantity: c.quantity + 1 } : c)
      return [...prev, { menuItemId: item.id, itemName: item.name, quantity: 1, unitPrice: item.finalPrice }]
    })
  }

  function removeFromCart(menuItemId: number) {
    setCart(prev => {
      const existing = prev.find(c => c.menuItemId === menuItemId)
      if (existing && existing.quantity > 1)
        return prev.map(c => c.menuItemId === menuItemId ? { ...c, quantity: c.quantity - 1 } : c)
      return prev.filter(c => c.menuItemId !== menuItemId)
    })
  }

  async function handleCreateOrder() {
    if (!address || cart.length === 0 || !selectedRest) return
    setSaving(true)
    try {
      const dto: CreateOrderDto = {
        customerId,
        restaurantId: selectedRest,
        deliveryAddress: address,
        paymentMethod: payment,
        notes,
        isExpress: strategy === 'Express',
        items: cart.map(c => ({
          menuItemId: c.menuItemId,
          itemName:   c.itemName,
          quantity:   c.quantity,
          unitPrice:  c.unitPrice,
        }))
      }
      await orderApi.create(dto)
      setShowForm(false)
      fetchOrders()
    } catch { setError('Could not create order.') }
    finally { setSaving(false) }
  }

  async function handleSetStrategy(s: DeliveryStrategy) {
    try { await orderApi.setStrategy(s); setStrategy(s) }
    catch { }
  }

  async function handleUpdateStatus(id: number, status: string) {
    try { await orderApi.updateStatus(id, status); fetchOrders() }
    catch { setError('Could not update status.') }
  }

  async function handleCancel(id: number) {
    try { await orderApi.cancel(id); fetchOrders() }
    catch { setError('Could not cancel order.') }
  }

  const cartTotal = cart.reduce((sum, c) => sum + c.unitPrice * c.quantity, 0)

  return (
    <div className="p-6 space-y-6">

      {/* Header */}
      <div className="flex flex-wrap items-center justify-between gap-4">
        <h1 className="text-2xl font-bold text-white">Orders</h1>
        <div className="flex items-center gap-3">
          <span className="text-sm text-gray-400">Strategy:</span>
          {STRATEGIES.map(s => (
            <button key={s} onClick={() => handleSetStrategy(s)}
              className={`px-3 py-1 rounded text-sm font-medium transition-colors ${
                strategy === s ? 'bg-orange-500 text-white' : 'bg-white/10 text-gray-300 hover:bg-white/20'
              }`}>{s}</button>
          ))}
        </div>
      </div>

      {/* Customer + Add */}
      <div className="flex items-center gap-3">
        <label className="text-sm text-gray-400">Customer ID:</label>
        <input type="number" min={1} value={customerId}
          onChange={e => setCustomerId(Number(e.target.value))}
          className="w-20 px-2 py-1 rounded bg-white/10 text-white text-sm border border-white/20 focus:outline-none focus:border-orange-500"
        />
        <button onClick={fetchOrders}
          className="px-3 py-1 bg-white/10 hover:bg-white/20 text-gray-300 rounded text-sm">
          Load
        </button>
        <button onClick={openForm}
          className="ml-auto px-4 py-2 bg-orange-500 hover:bg-orange-600 text-white rounded-lg text-sm font-medium transition-colors">
          + New Order
        </button>
      </div>

      {error && <p className="text-red-400 text-sm">{error}</p>}

      {/* Create Order Modal */}
      {showForm && (
        <div className="fixed inset-0 bg-black/70 flex items-start justify-center z-50 overflow-y-auto py-8">
          <div className="bg-[#1a1a2e] border border-white/10 rounded-2xl p-6 w-full max-w-2xl space-y-4">
            <h2 className="text-white font-semibold text-lg">New Order</h2>

            {/* Restaurant select */}
            <div>
              <label className="text-xs text-gray-400 block mb-1">Restaurant</label>
              <select
                value={selectedRest}
                onChange={e => handleSelectRestaurant(Number(e.target.value))}
                className="w-full px-3 py-2 rounded-lg bg-white/10 text-white text-sm border border-white/20 focus:outline-none focus:border-orange-500"
              >
                <option value={0}>Select a restaurant...</option>
                {restaurants.map(r => (
                  <option key={r.id} value={r.id}>{r.name} — {r.city}</option>
                ))}
              </select>
            </div>

            {/* Menu items */}
            {menu.length > 0 && (
              <div>
                <label className="text-xs text-gray-400 block mb-1">Menu</label>
                <div className="space-y-1 max-h-48 overflow-y-auto">
                  {menu.filter(i => i.isAvailable).map(item => {
                    const inCart = cart.find(c => c.menuItemId === item.id)
                    return (
                      <div key={item.id} className="flex items-center justify-between bg-white/5 rounded-lg px-3 py-2">
                        <span className="text-white text-sm">{item.name}</span>
                        <div className="flex items-center gap-2">
                          <span className="text-orange-400 text-sm">{item.finalPrice.toFixed(2)} RON</span>
                          {inCart ? (
                            <div className="flex items-center gap-1">
                              <button onClick={() => removeFromCart(item.id)}
                                className="w-6 h-6 bg-white/10 hover:bg-white/20 text-white rounded text-xs">−</button>
                              <span className="text-white text-sm w-4 text-center">{inCart.quantity}</span>
                              <button onClick={() => addToCart(item)}
                                className="w-6 h-6 bg-orange-500 hover:bg-orange-600 text-white rounded text-xs">+</button>
                            </div>
                          ) : (
                            <button onClick={() => addToCart(item)}
                              className="px-2 py-1 bg-orange-500 hover:bg-orange-600 text-white rounded text-xs">
                              Add
                            </button>
                          )}
                        </div>
                      </div>
                    )
                  })}
                </div>
              </div>
            )}

            {/* Cart summary */}
            {cart.length > 0 && (
              <div className="bg-white/5 rounded-lg p-3 space-y-1">
                <p className="text-xs text-gray-400 mb-2">Cart</p>
                {cart.map(c => (
                  <div key={c.menuItemId} className="flex justify-between text-sm">
                    <span className="text-gray-300">{c.itemName} × {c.quantity}</span>
                    <span className="text-gray-400">{(c.unitPrice * c.quantity).toFixed(2)} RON</span>
                  </div>
                ))}
                <div className="flex justify-between text-sm font-medium pt-1 border-t border-white/10">
                  <span className="text-white">Total items</span>
                  <span className="text-orange-400">{cartTotal.toFixed(2)} RON</span>
                </div>
              </div>
            )}

            {/* Address */}
            <div>
              <label className="text-xs text-gray-400 block mb-1">Delivery Address</label>
              <input type="text" value={address} onChange={e => setAddress(e.target.value)}
                placeholder="Str. Ștefan cel Mare 10, Chișinău"
                className="w-full px-3 py-2 rounded-lg bg-white/10 text-white text-sm border border-white/20 focus:outline-none focus:border-orange-500 placeholder-gray-600"
              />
            </div>

            {/* Payment */}
            <div>
              <label className="text-xs text-gray-400 block mb-1">Payment Method</label>
              <div className="flex gap-2">
                {['Cash', 'Card', 'PayPal'].map(p => (
                  <button key={p} onClick={() => setPayment(p)}
                    className={`px-3 py-1 rounded text-sm transition-colors ${
                      payment === p ? 'bg-orange-500 text-white' : 'bg-white/10 text-gray-300 hover:bg-white/20'
                    }`}>{p}</button>
                ))}
              </div>
            </div>

            {/* Notes */}
            <div>
              <label className="text-xs text-gray-400 block mb-1">Notes (optional)</label>
              <input type="text" value={notes} onChange={e => setNotes(e.target.value)}
                placeholder="Fără ceapă, vă rog..."
                className="w-full px-3 py-2 rounded-lg bg-white/10 text-white text-sm border border-white/20 focus:outline-none focus:border-orange-500 placeholder-gray-600"
              />
            </div>

            <div className="flex gap-3 pt-2">
              <button onClick={handleCreateOrder}
                disabled={saving || cart.length === 0 || !address || !selectedRest}
                className="flex-1 py-2 bg-orange-500 hover:bg-orange-600 disabled:opacity-40 text-white rounded-lg text-sm font-medium transition-colors">
                {saving ? 'Placing...' : `Place Order${cart.length > 0 ? ` — ${cartTotal.toFixed(2)} RON` : ''}`}
              </button>
              <button onClick={() => setShowForm(false)}
                className="flex-1 py-2 bg-white/10 hover:bg-white/20 text-gray-300 rounded-lg text-sm transition-colors">
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Orders list */}
      {loading ? (
        <p className="text-gray-400 animate-pulse">Loading...</p>
      ) : orders.length === 0 ? (
        <div className="text-center py-16 text-gray-500">
          <p className="text-4xl mb-3">🛍️</p>
          <p>No orders for customer #{customerId}. Add one above.</p>
        </div>
      ) : (
        <div className="space-y-4">
          {orders.map(order => (
            <div key={order.id} className="bg-white/5 border border-white/10 rounded-xl p-5 space-y-3 hover:border-white/20 transition-colors">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <span className="text-white font-semibold">Order #{order.id}</span>
                  <span className={`px-2 py-0.5 rounded-full text-xs font-medium ${STATUS_COLORS[order.status] ?? ''}`}>
                    {order.status}
                  </span>
                </div>
                <span className="text-orange-400 font-bold text-lg">{order.totalPrice.toFixed(2)} RON</span>
              </div>

              <div className="grid grid-cols-2 gap-1 text-sm text-gray-400">
                <span>📍 {order.deliveryAddress}</span>
                <span>💳 {order.paymentMethod}</span>
                {order.notes && <span className="col-span-2">📝 {order.notes}</span>}
              </div>

              <div className="border-t border-white/10 pt-3 space-y-1">
                {order.items.map((item, i) => (
                  <div key={i} className="flex justify-between text-sm">
                    <span className="text-gray-300">{item.itemName} × {item.quantity}</span>
                    <span className="text-gray-400">{item.totalPrice.toFixed(2)} RON</span>
                  </div>
                ))}
              </div>

              <div className="flex items-center gap-2 pt-1">
                {NEXT_STATUS[order.status] && (
                  <button onClick={() => handleUpdateStatus(order.id, NEXT_STATUS[order.status])}
                    className="px-3 py-1 bg-blue-500/20 hover:bg-blue-500/40 text-blue-300 rounded text-xs transition-colors">
                    → {NEXT_STATUS[order.status]}
                  </button>
                )}
                {!['Delivered', 'Cancelled', 'OutForDelivery'].includes(order.status) && (
                  <button onClick={() => handleCancel(order.id)}
                    className="px-3 py-1 bg-red-500/20 hover:bg-red-500/40 text-red-300 rounded text-xs transition-colors">
                    Cancel
                  </button>
                )}
                <span className="ml-auto text-xs text-gray-500">
                  {new Date(order.createdAt).toLocaleString()}
                </span>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
