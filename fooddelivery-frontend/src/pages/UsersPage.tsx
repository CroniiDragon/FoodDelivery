import { useEffect, useState } from 'react'
import { Plus, User, Bike, Trash2 } from 'lucide-react'
import toast from 'react-hot-toast'
import { userApi } from '../services/api'
import { LoadingSpinner, EmptyState, Modal, RoleBadge } from '../components/ui'
import type { UserResponse, CreateCustomerDto, CreateCourierDto } from '../types'

type Tab = 'list' | 'addCustomer' | 'addCourier'

export default function UsersPage() {
  const [users, setUsers]     = useState<UserResponse[]>([])
  const [loading, setLoading] = useState(true)
  const [tab, setTab]         = useState<Tab>('list')
  const [cForm, setCForm]     = useState<CreateCustomerDto>({
    name: '', email: '', phone: '', password: '', deliveryAddress: '', city: ''
  })
  const [curForm, setCurForm] = useState<CreateCourierDto>({
    name: '', email: '', phone: '', password: '', vehicleType: ''
  })

  useEffect(() => { load() }, [])

  const load = async () => {
    try { setUsers(await userApi.getAll()) }
    catch { toast.error('Nu s-au putut încărca utilizatorii.') }
    finally { setLoading(false) }
  }

  const handleAddCustomer = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      const u = await userApi.createCustomer(cForm)
      setUsers(p => [...p, u])
      setTab('list')
      toast.success('Client înregistrat!')
    } catch { toast.error('Eroare la înregistrare.') }
  }

  const handleAddCourier = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      const u = await userApi.createCourier(curForm)
      setUsers(p => [...p, u])
      setTab('list')
      toast.success('Curier înregistrat!')
    } catch { toast.error('Eroare la înregistrare.') }
  }

  const handleDelete = async (id: number) => {
    try {
      await userApi.delete(id)
      setUsers(p => p.filter(u => u.id !== id))
      toast.success('Utilizator șters.')
    } catch { toast.error('Eroare la ștergere.') }
  }

  const customers = users.filter(u => u.role === 'Customer')
  const couriers  = users.filter(u => u.role === 'Courier')

  if (loading) return <LoadingSpinner text="Se încarcă utilizatorii..." />

  return (
    <div className="page-container">
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="section-title mb-0">Utilizatori</h1>
          <p className="text-white/40 text-sm mt-1">{customers.length} clienți · {couriers.length} curieri</p>
        </div>
        <div className="flex gap-2">
          <button onClick={() => setTab('addCustomer')} className="btn-primary flex items-center gap-2 text-sm">
            <User size={15} /> Client Nou
          </button>
          <button onClick={() => setTab('addCourier')} className="btn-secondary flex items-center gap-2 text-sm">
            <Bike size={15} /> Curier Nou
          </button>
        </div>
      </div>

      {/* Tabs */}
      {tab !== 'list' && (
        <Modal
          title={tab === 'addCustomer' ? 'Înregistrare Client' : 'Înregistrare Curier'}
          onClose={() => setTab('list')}
        >
          {tab === 'addCustomer' ? (
            <form onSubmit={handleAddCustomer} className="space-y-4">
              {([
                ['name','Nume complet'],['email','Email'],['phone','Telefon'],
                ['password','Parolă'],['deliveryAddress','Adresă livrare'],['city','Oraș'],
              ] as [keyof CreateCustomerDto, string][]).map(([k,lbl]) => (
                <div key={k}>
                  <label className="block text-white/60 text-sm mb-1.5">{lbl}</label>
                  <input type={k === 'password' ? 'password' : k === 'email' ? 'email' : 'text'}
                    required className="input"
                    onChange={e => setCForm(p => ({ ...p, [k]: e.target.value }))} />
                </div>
              ))}
              <div className="flex gap-3 pt-2">
                <button type="button" onClick={() => setTab('list')} className="btn-secondary flex-1">Anulează</button>
                <button type="submit" className="btn-primary flex-1">Înregistrează</button>
              </div>
            </form>
          ) : (
            <form onSubmit={handleAddCourier} className="space-y-4">
              {([
                ['name','Nume complet'],['email','Email'],['phone','Telefon'],['password','Parolă'],
              ] as [keyof CreateCourierDto, string][]).map(([k,lbl]) => (
                <div key={k}>
                  <label className="block text-white/60 text-sm mb-1.5">{lbl}</label>
                  <input type={k === 'password' ? 'password' : k === 'email' ? 'email' : 'text'}
                    required className="input"
                    onChange={e => setCurForm(p => ({ ...p, [k]: e.target.value }))} />
                </div>
              ))}
              <div>
                <label className="block text-white/60 text-sm mb-1.5">Tip vehicul</label>
                <select className="input" onChange={e => setCurForm(p => ({ ...p, vehicleType: e.target.value }))}>
                  <option value="Bicicletă">Bicicletă</option>
                  <option value="Scuter">Scuter</option>
                  <option value="Mașină">Mașină</option>
                </select>
              </div>
              <div className="flex gap-3 pt-2">
                <button type="button" onClick={() => setTab('list')} className="btn-secondary flex-1">Anulează</button>
                <button type="submit" className="btn-primary flex-1">Înregistrează</button>
              </div>
            </form>
          )}
        </Modal>
      )}

      {/* User list */}
      {users.length === 0
        ? <EmptyState title="Niciun utilizator" sub="Adaugă primul client sau curier." />
        : <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
            {users.map(u => (
              <div key={u.id} className="card p-4 animate-slide-up hover:border-white/20 transition-all">
                <div className="flex items-start justify-between mb-3">
                  <div className="flex items-center gap-3">
                    <div className="w-10 h-10 rounded-full bg-brand-500/15 border border-brand-500/25
                                    flex items-center justify-center text-brand-400 font-bold">
                      {u.name.charAt(0).toUpperCase()}
                    </div>
                    <div>
                      <p className="text-white font-semibold text-sm">{u.name}</p>
                      <RoleBadge role={u.role} />
                    </div>
                  </div>
                  <button onClick={() => handleDelete(u.id)}
                    className="text-white/20 hover:text-red-400 transition-colors p-1">
                    <Trash2 size={15} />
                  </button>
                </div>
                <div className="space-y-1 text-xs text-white/40">
                  <p>{u.email}</p>
                  <p>{u.phone}</p>
                </div>
              </div>
            ))}
          </div>
      }
    </div>
  )
}
