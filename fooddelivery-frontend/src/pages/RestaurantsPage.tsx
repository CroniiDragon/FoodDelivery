import { useState, useEffect } from 'react'
import { restaurantApi } from '../services/api'
import type { RestaurantResponse, MenuItemResponse, CreateRestaurantDto, CreateMenuItemDto } from '../types'

const EMPTY_RESTAURANT: CreateRestaurantDto = {
  name: '', address: '', city: '', cuisine: '', phoneNumber: ''
}

const EMPTY_MENU_ITEM: Omit<CreateMenuItemDto, 'restaurantId'> = {
  name: '', description: '', basePrice: 0, category: '',
  itemType: 'Food', calories: 0, isVegetarian: false,
  volumeInLiters: 0.5, isAlcoholic: false,
}

export default function RestaurantsPage() {
  const [restaurants, setRestaurants]   = useState<RestaurantResponse[]>([])
  const [menu, setMenu]                 = useState<MenuItemResponse[]>([])
  const [selectedId, setSelectedId]     = useState<number | null>(null)
  const [loading, setLoading]           = useState(true)
  const [menuLoading, setMenuLoading]   = useState(false)
  const [showRestForm, setShowRestForm] = useState(false)
  const [showMenuForm, setShowMenuForm] = useState(false)
  const [restForm, setRestForm]         = useState<CreateRestaurantDto>(EMPTY_RESTAURANT)
  const [menuForm, setMenuForm]         = useState<Omit<CreateMenuItemDto, 'restaurantId'>>(EMPTY_MENU_ITEM)
  const [saving, setSaving]             = useState(false)
  const [cloneMsg, setCloneMsg]         = useState('')
  const [error, setError]               = useState('')

  useEffect(() => { fetchRestaurants() }, [])

  async function fetchRestaurants() {
    setLoading(true)
    try { setRestaurants(await restaurantApi.getAll()) }
    catch { setError('Could not load restaurants.') }
    finally { setLoading(false) }
  }

  async function fetchMenu(id: number) {
    setMenuLoading(true)
    setError('')
    setSelectedId(id)
    try { setMenu(await restaurantApi.getMenu(id)) }
    catch { setError('Could not load menu.'); setMenu([]) }
    finally { setMenuLoading(false) }
  }

  async function handleCreateRestaurant() {
    if (!restForm.name || !restForm.city || !restForm.cuisine) return
    setSaving(true)
    try {
      await restaurantApi.create(restForm)
      setRestForm(EMPTY_RESTAURANT)
      setShowRestForm(false)
      fetchRestaurants()
    } catch { setError('Could not create restaurant.') }
    finally { setSaving(false) }
  }

  async function handleAddMenuItem() {
    if (!selectedId || !menuForm.name || !menuForm.category || menuForm.basePrice <= 0) return
    setSaving(true)
    try {
      await restaurantApi.addMenuItem({ ...menuForm, restaurantId: selectedId })
      setMenuForm(EMPTY_MENU_ITEM)
      setShowMenuForm(false)
      fetchMenu(selectedId)
    } catch { setError('Could not add menu item.') }
    finally { setSaving(false) }
  }

  async function handleClone(id: number, type: 'branch' | 'seasonal') {
    setCloneMsg('')
    try {
      const cloned = type === 'branch'
        ? await restaurantApi.cloneBranch(id)
        : await restaurantApi.cloneSeasonal(id)
      setCloneMsg(`✓ "${cloned.name}" created (ID: ${cloned.id})`)
      fetchRestaurants()
    } catch { setCloneMsg('Clone failed.') }
  }

  async function handleToggle(menuItemId: number) {
    await restaurantApi.toggleAvailability(menuItemId)
    if (selectedId) fetchMenu(selectedId)
  }

  const selected = restaurants.find(r => r.id === selectedId)

  return (
    <div className="p-6 space-y-6">

      {/* Header */}
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold text-white">Restaurants</h1>
        <button
          onClick={() => { setShowRestForm(true); setError('') }}
          className="px-4 py-2 bg-orange-500 hover:bg-orange-600 text-white rounded-lg text-sm font-medium transition-colors"
        >
          + Add Restaurant
        </button>
      </div>

      {/* Add Restaurant Modal */}
      {showRestForm && (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50">
          <div className="bg-[#1a1a2e] border border-white/10 rounded-2xl p-6 w-full max-w-md space-y-4">
            <h2 className="text-white font-semibold text-lg">New Restaurant</h2>
            {[
              { label: 'Name',    key: 'name',        placeholder: 'Pizza Roma' },
              { label: 'Address', key: 'address',     placeholder: 'Str. Ștefan cel Mare 10' },
              { label: 'City',    key: 'city',        placeholder: 'Chișinău' },
              { label: 'Cuisine', key: 'cuisine',     placeholder: 'Italian' },
              { label: 'Phone',   key: 'phoneNumber', placeholder: '+373 22 123 456' },
            ].map(({ label, key, placeholder }) => (
              <div key={key}>
                <label className="text-xs text-gray-400 block mb-1">{label}</label>
                <input
                  type="text"
                  placeholder={placeholder}
                  value={(restForm as any)[key]}
                  onChange={e => setRestForm(f => ({ ...f, [key]: e.target.value }))}
                  className="w-full px-3 py-2 rounded-lg bg-white/10 text-white text-sm border border-white/20 focus:outline-none focus:border-orange-500 placeholder-gray-600"
                />
              </div>
            ))}
            {error && <p className="text-red-400 text-sm">{error}</p>}
            <div className="flex gap-3 pt-2">
              <button
                onClick={handleCreateRestaurant}
                disabled={saving || !restForm.name || !restForm.city || !restForm.cuisine}
                className="flex-1 py-2 bg-orange-500 hover:bg-orange-600 disabled:opacity-40 text-white rounded-lg text-sm font-medium transition-colors"
              >
                {saving ? 'Saving...' : 'Create'}
              </button>
              <button
                onClick={() => { setShowRestForm(false); setRestForm(EMPTY_RESTAURANT); setError('') }}
                className="flex-1 py-2 bg-white/10 hover:bg-white/20 text-gray-300 rounded-lg text-sm transition-colors"
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Add Menu Item Modal */}
      {showMenuForm && selectedId && (
        <div className="fixed inset-0 bg-black/60 flex items-center justify-center z-50">
          <div className="bg-[#1a1a2e] border border-white/10 rounded-2xl p-6 w-full max-w-md space-y-4">
            <h2 className="text-white font-semibold text-lg">
              Add Menu Item — {selected?.name}
            </h2>

            {/* Type toggle */}
            <div>
              <label className="text-xs text-gray-400 block mb-1">Type</label>
              <div className="flex gap-2">
                {['Food', 'Drink'].map(t => (
                  <button key={t}
                    onClick={() => setMenuForm(f => ({ ...f, itemType: t }))}
                    className={`px-4 py-1.5 rounded text-sm font-medium transition-colors ${
                      menuForm.itemType === t
                        ? 'bg-orange-500 text-white'
                        : 'bg-white/10 text-gray-300 hover:bg-white/20'
                    }`}
                  >{t}</button>
                ))}
              </div>
            </div>

            {[
              { label: 'Name',        key: 'name',        type: 'text',   placeholder: 'Pizza Margherita' },
              { label: 'Description', key: 'description', type: 'text',   placeholder: 'Descriere scurtă' },
              { label: 'Category',    key: 'category',    type: 'text',   placeholder: 'Pizza / Paste / Băuturi' },
              { label: 'Base Price (RON)', key: 'basePrice', type: 'number', placeholder: '35' },
            ].map(({ label, key, type, placeholder }) => (
              <div key={key}>
                <label className="text-xs text-gray-400 block mb-1">{label}</label>
                <input
                  type={type}
                  placeholder={placeholder}
                  value={(menuForm as any)[key]}
                  onChange={e => setMenuForm(f => ({
                    ...f,
                    [key]: type === 'number' ? Number(e.target.value) : e.target.value
                  }))}
                  className="w-full px-3 py-2 rounded-lg bg-white/10 text-white text-sm border border-white/20 focus:outline-none focus:border-orange-500 placeholder-gray-600"
                />
              </div>
            ))}

            {/* Food-specific */}
            {menuForm.itemType === 'Food' && (
              <div className="space-y-3">
                <div>
                  <label className="text-xs text-gray-400 block mb-1">Calories</label>
                  <input type="number" value={menuForm.calories}
                    onChange={e => setMenuForm(f => ({ ...f, calories: Number(e.target.value) }))}
                    className="w-full px-3 py-2 rounded-lg bg-white/10 text-white text-sm border border-white/20 focus:outline-none focus:border-orange-500"
                  />
                </div>
                <div className="flex gap-4">
                  {[
                    { label: 'Vegetarian', key: 'isVegetarian' },
                    { label: 'Vegan',      key: 'isVegan' },
                  ].map(({ label, key }) => (
                    <label key={key} className="flex items-center gap-2 text-sm text-gray-300 cursor-pointer">
                      <input type="checkbox"
                        checked={(menuForm as any)[key]}
                        onChange={e => setMenuForm(f => ({ ...f, [key]: e.target.checked }))}
                        className="accent-orange-500"
                      />
                      {label}
                    </label>
                  ))}
                </div>
              </div>
            )}

            {/* Drink-specific */}
            {menuForm.itemType === 'Drink' && (
              <div className="space-y-3">
                <div>
                  <label className="text-xs text-gray-400 block mb-1">Volume (L)</label>
                  <input type="number" step="0.1" value={menuForm.volumeInLiters}
                    onChange={e => setMenuForm(f => ({ ...f, volumeInLiters: Number(e.target.value) }))}
                    className="w-full px-3 py-2 rounded-lg bg-white/10 text-white text-sm border border-white/20 focus:outline-none focus:border-orange-500"
                  />
                </div>
                <label className="flex items-center gap-2 text-sm text-gray-300 cursor-pointer">
                  <input type="checkbox"
                    checked={menuForm.isAlcoholic}
                    onChange={e => setMenuForm(f => ({ ...f, isAlcoholic: e.target.checked }))}
                    className="accent-orange-500"
                  />
                  Alcoholic (+10% price)
                </label>
              </div>
            )}

            {error && <p className="text-red-400 text-sm">{error}</p>}

            <div className="flex gap-3 pt-2">
              <button
                onClick={handleAddMenuItem}
                disabled={saving || !menuForm.name || !menuForm.category || menuForm.basePrice <= 0}
                className="flex-1 py-2 bg-orange-500 hover:bg-orange-600 disabled:opacity-40 text-white rounded-lg text-sm font-medium transition-colors"
              >
                {saving ? 'Saving...' : 'Add Item'}
              </button>
              <button
                onClick={() => { setShowMenuForm(false); setMenuForm(EMPTY_MENU_ITEM); setError('') }}
                className="flex-1 py-2 bg-white/10 hover:bg-white/20 text-gray-300 rounded-lg text-sm transition-colors"
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}

      {cloneMsg && (
        <div className="px-4 py-2 bg-green-500/20 border border-green-500/30 rounded-lg text-green-300 text-sm">
          {cloneMsg}
        </div>
      )}

      {loading ? (
        <p className="text-gray-400 animate-pulse">Loading...</p>
      ) : restaurants.length === 0 ? (
        <div className="text-center py-16 text-gray-500">
          <p className="text-4xl mb-3">🍽️</p>
          <p>No restaurants yet. Add one above.</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">

          {/* Restaurant list */}
          <div className="space-y-3">
            {restaurants.map(r => (
              <div
                key={r.id}
                onClick={() => fetchMenu(r.id)}
                className={`bg-white/5 border rounded-xl p-4 cursor-pointer transition-colors ${
                  selectedId === r.id
                    ? 'border-orange-500/60 bg-orange-500/5'
                    : 'border-white/10 hover:border-white/20'
                }`}
              >
                <div className="flex items-center justify-between">
                  <span className="text-white font-semibold">{r.name}</span>
                  <span className={`px-2 py-0.5 rounded-full text-xs font-medium ${
                    r.isOpen ? 'bg-green-500/20 text-green-300' : 'bg-red-500/20 text-red-300'
                  }`}>
                    {r.isOpen ? 'Open' : 'Closed'}
                  </span>
                </div>
                <p className="text-sm text-gray-400 mt-1">
                  {r.city} · {r.cuisine} · {r.menuItemCount} items
                </p>
                <div className="flex gap-2 mt-3" onClick={e => e.stopPropagation()}>
                  <button onClick={() => handleClone(r.id, 'branch')}
                    className="px-2 py-1 bg-blue-500/20 hover:bg-blue-500/40 text-blue-300 rounded text-xs transition-colors">
                    + Branch
                  </button>
                  <button onClick={() => handleClone(r.id, 'seasonal')}
                    className="px-2 py-1 bg-purple-500/20 hover:bg-purple-500/40 text-purple-300 rounded text-xs transition-colors">
                    + Seasonal
                  </button>
                </div>
              </div>
            ))}
          </div>

          {/* Menu panel */}
          <div className="bg-white/5 border border-white/10 rounded-xl p-4">
            {!selectedId ? (
              <p className="text-gray-500 text-sm">Select a restaurant to view its menu.</p>
            ) : (
              <>
                <div className="flex items-center justify-between mb-3">
                  <h2 className="text-white font-semibold">{selected?.name} — Menu</h2>
                  <div className="flex items-center gap-2">
                    {!selected?.isOpen && (
                      <span className="text-xs text-red-400 italic">Proxy: closed</span>
                    )}
                    <button
                      onClick={() => { setShowMenuForm(true); setError('') }}
                      className="px-2 py-1 bg-orange-500 hover:bg-orange-600 text-white rounded text-xs transition-colors"
                    >
                      + Add Item
                    </button>
                  </div>
                </div>

                {menuLoading ? (
                  <p className="text-gray-400 text-sm animate-pulse">Loading...</p>
                ) : menu.length === 0 ? (
                  <p className="text-gray-500 text-sm">
                    {selected?.isOpen
                      ? 'No items yet. Add one with "+ Add Item".'
                      : 'Restaurant is closed — menu blocked by Proxy.'}
                  </p>
                ) : (
                  <div className="space-y-2">
                    {menu.map(item => (
                      <div key={item.id} className="flex items-center justify-between text-sm">
                        <div>
                          <span className={item.isAvailable ? 'text-white' : 'text-gray-500 line-through'}>
                            {item.name}
                          </span>
                          <span className="ml-2 text-xs text-gray-500">{item.category}</span>
                        </div>
                        <div className="flex items-center gap-3">
                          <span className="text-orange-400">{item.finalPrice.toFixed(2)} RON</span>
                          <button
                            onClick={() => handleToggle(item.id)}
                            className="text-xs text-gray-400 hover:text-white transition-colors"
                          >
                            {item.isAvailable ? 'Disable' : 'Enable'}
                          </button>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </>
            )}
          </div>

        </div>
      )}
    </div>
  )
}
