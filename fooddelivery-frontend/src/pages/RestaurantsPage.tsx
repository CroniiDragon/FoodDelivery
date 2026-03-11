import { useEffect, useState } from 'react'
import { Plus, MapPin, Phone, Utensils, ChevronRight, ToggleLeft, ToggleRight } from 'lucide-react'
import toast from 'react-hot-toast'
import { restaurantApi } from '../services/api'
import { LoadingSpinner, EmptyState, Modal } from '../components/ui'
import type { RestaurantResponse, MenuItemResponse, CreateRestaurantDto, CreateMenuItemDto } from '../types'

export default function RestaurantsPage() {
  const [restaurants, setRestaurants]   = useState<RestaurantResponse[]>([])
  const [selected, setSelected]         = useState<RestaurantResponse | null>(null)
  const [menu, setMenu]                 = useState<MenuItemResponse[]>([])
  const [loading, setLoading]           = useState(true)
  const [showAddR, setShowAddR]         = useState(false)
  const [showAddM, setShowAddM]         = useState(false)

  const [rForm, setRForm] = useState<CreateRestaurantDto>({
    name: '', address: '', city: '', cuisine: '', phoneNumber: ''
  })
  const [mForm, setMForm] = useState<Partial<CreateMenuItemDto>>({
    itemType: 'Food', category: 'Main'
  })

  useEffect(() => { load() }, [])

  const load = async () => {
    try {
      const data = await restaurantApi.getAll()
      setRestaurants(data)
    } catch { toast.error('Nu s-au putut încărca restaurantele.') }
    finally { setLoading(false) }
  }

  const selectRestaurant = async (r: RestaurantResponse) => {
    setSelected(r)
    try {
      const m = await restaurantApi.getMenu(r.id)
      setMenu(m)
    } catch { setMenu([]) }
  }

  const handleAddRestaurant = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      const r = await restaurantApi.create(rForm)
      setRestaurants(prev => [...prev, r])
      setShowAddR(false)
      setRForm({ name: '', address: '', city: '', cuisine: '', phoneNumber: '' })
      toast.success('Restaurant adăugat!')
    } catch { toast.error('Eroare la adăugare.') }
  }

  const handleAddMenuItem = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!selected) return
    try {
      const item = await restaurantApi.addMenuItem({ ...mForm, restaurantId: selected.id } as CreateMenuItemDto)
      setMenu(prev => [...prev, item])
      setShowAddM(false)
      setMForm({ itemType: 'Food', category: 'Main' })
      toast.success('Produs adăugat în meniu!')
    } catch { toast.error('Eroare la adăugare.') }
  }

  const toggleItem = async (itemId: number) => {
    try {
      await restaurantApi.toggleAvailability(itemId)
      setMenu(prev => prev.map(m => m.id === itemId ? { ...m, isAvailable: !m.isAvailable } : m))
    } catch { toast.error('Eroare.') }
  }

  if (loading) return <LoadingSpinner text="Se încarcă restaurantele..." />

  return (
    <div className="page-container">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="section-title mb-0">Restaurante</h1>
          <p className="text-white/40 text-sm mt-1">{restaurants.length} restaurante înregistrate</p>
        </div>
        <button onClick={() => setShowAddR(true)} className="btn-primary flex items-center gap-2">
          <Plus size={16} /> Adaugă Restaurant
        </button>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Lista restaurante */}
        <div className="space-y-3">
          {restaurants.length === 0
            ? <EmptyState title="Niciun restaurant" sub="Apasă + pentru a adăuga primul restaurant." />
            : restaurants.map(r => (
              <button key={r.id} onClick={() => selectRestaurant(r)}
                className={`w-full text-left card p-4 hover:border-brand-500/30 transition-all duration-200
                  ${selected?.id === r.id ? 'border-brand-500/40 bg-brand-500/5' : ''}`}>
                <div className="flex items-start justify-between">
                  <div>
                    <div className="flex items-center gap-2 mb-1">
                      <p className="font-semibold text-white">{r.name}</p>
                      <span className={`w-2 h-2 rounded-full ${r.isOpen ? 'bg-green-400' : 'bg-red-400'}`} />
                    </div>
                    <div className="flex items-center gap-1 text-white/40 text-sm">
                      <MapPin size={12} />
                      <span>{r.city}</span>
                    </div>
                    <p className="text-brand-400/70 text-xs mt-1">{r.cuisine}</p>
                  </div>
                  <div className="flex items-center gap-2">
                    <span className="text-white/30 text-xs">{r.menuItemCount} produse</span>
                    <ChevronRight size={16} className="text-white/20" />
                  </div>
                </div>
              </button>
            ))}
        </div>

        {/* Meniu restaurant selectat */}
        <div className="card p-5">
          {!selected ? (
            <div className="flex flex-col items-center justify-center h-48 text-white/20">
              <Utensils size={32} />
              <p className="mt-3 text-sm">Selectează un restaurant pentru a vedea meniul</p>
            </div>
          ) : (
            <>
              <div className="flex items-center justify-between mb-4">
                <h3 className="font-display font-bold text-white">{selected.name} — Meniu</h3>
                <button onClick={() => setShowAddM(true)}
                  className="btn-secondary flex items-center gap-1.5 text-xs py-1.5 px-3">
                  <Plus size={14} /> Produs
                </button>
              </div>
              {menu.length === 0
                ? <EmptyState title="Meniu gol" sub="Adaugă primul produs." />
                : <div className="space-y-2 max-h-96 overflow-y-auto pr-1">
                    {menu.map(item => (
                      <div key={item.id} className="flex items-center justify-between
                                                     bg-white/3 rounded-xl px-3 py-2.5 border border-white/5">
                        <div>
                          <p className="text-white text-sm font-medium">{item.name}</p>
                          <p className="text-white/40 text-xs">{item.category} · {item.itemType}</p>
                        </div>
                        <div className="flex items-center gap-3">
                          <span className="text-brand-400 font-semibold text-sm">
                            {item.finalPrice.toFixed(2)} LEI
                          </span>
                          <button onClick={() => toggleItem(item.id)} className="text-white/30 hover:text-white/70 transition-colors">
                            {item.isAvailable
                              ? <ToggleRight size={20} className="text-green-400" />
                              : <ToggleLeft size={20} />}
                          </button>
                        </div>
                      </div>
                    ))}
                  </div>
              }
            </>
          )}
        </div>
      </div>

      {/* Modal adaugă restaurant */}
      {showAddR && (
        <Modal title="Restaurant Nou" onClose={() => setShowAddR(false)}>
          <form onSubmit={handleAddRestaurant} className="space-y-4">
            {([
              ['name','Nume restaurant','text'],
              ['address','Adresă','text'],
              ['city','Oraș','text'],
              ['cuisine','Tip bucătărie (ex: Italiană)','text'],
              ['phoneNumber','Telefon','tel'],
            ] as [keyof CreateRestaurantDto, string, string][]).map(([k,lbl,type]) => (
              <div key={k}>
                <label className="block text-white/60 text-sm mb-1.5">{lbl}</label>
                <input type={type} required className="input"
                  value={(rForm as any)[k]}
                  onChange={e => setRForm(p => ({ ...p, [k]: e.target.value }))} />
              </div>
            ))}
            <div className="flex gap-3 pt-2">
              <button type="button" onClick={() => setShowAddR(false)} className="btn-secondary flex-1">Anulează</button>
              <button type="submit" className="btn-primary flex-1">Adaugă</button>
            </div>
          </form>
        </Modal>
      )}

      {/* Modal adaugă produs meniu */}
      {showAddM && (
        <Modal title="Produs Nou în Meniu" onClose={() => setShowAddM(false)}>
          <form onSubmit={handleAddMenuItem} className="space-y-4">
            <div>
              <label className="block text-white/60 text-sm mb-1.5">Tip produs</label>
              <select className="input" value={mForm.itemType}
                onChange={e => setMForm(p => ({ ...p, itemType: e.target.value }))}>
                <option value="Food">Mâncare</option>
                <option value="Drink">Băutură</option>
              </select>
            </div>
            {['name','description','category'].map(k => (
              <div key={k}>
                <label className="block text-white/60 text-sm mb-1.5 capitalize">{k}</label>
                <input type="text" required className="input"
                  onChange={e => setMForm(p => ({ ...p, [k]: e.target.value }))} />
              </div>
            ))}
            <div>
              <label className="block text-white/60 text-sm mb-1.5">Preț (LEI)</label>
              <input type="number" step="0.01" required className="input"
                onChange={e => setMForm(p => ({ ...p, basePrice: parseFloat(e.target.value) }))} />
            </div>
            <div className="flex gap-3 pt-2">
              <button type="button" onClick={() => setShowAddM(false)} className="btn-secondary flex-1">Anulează</button>
              <button type="submit" className="btn-primary flex-1">Adaugă</button>
            </div>
          </form>
        </Modal>
      )}
    </div>
  )
}
