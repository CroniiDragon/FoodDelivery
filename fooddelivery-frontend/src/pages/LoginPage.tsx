import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { ChefHat, Mail, Lock, Eye, EyeOff } from 'lucide-react'
import toast from 'react-hot-toast'
import { userApi } from '../services/api'
import type { UserResponse } from '../types'

interface Props { onLogin: (user: UserResponse) => void }

export default function LoginPage({ onLogin }: Props) {
  const [email, setEmail]       = useState('')
  const [password, setPassword] = useState('')
  const [showPw, setShowPw]     = useState(false)
  const [loading, setLoading]   = useState(false)
  const navigate = useNavigate()

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setLoading(true)
    try {
      const user = await userApi.login({ email, password })
      onLogin(user)
      toast.success(`Bun venit, ${user.name}!`)
      navigate('/')
    } catch {
      toast.error('Email sau parolă incorectă.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-[#0f0f13] p-4">
      {/* Background glow */}
      <div className="absolute inset-0 overflow-hidden pointer-events-none">
        <div className="absolute top-1/3 left-1/2 -translate-x-1/2 -translate-y-1/2
                        w-96 h-96 bg-brand-500/10 rounded-full blur-3xl" />
      </div>

      <div className="relative w-full max-w-md animate-slide-up">
        {/* Logo */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16
                          rounded-2xl bg-brand-500 mb-4 shadow-lg shadow-brand-500/30">
            <ChefHat size={32} className="text-white" />
          </div>
          <h1 className="font-display text-3xl font-bold text-white">FoodDelivery</h1>
          <p className="text-white/40 mt-2">Autentifică-te în platformă</p>
        </div>

        {/* Form */}
        <div className="card p-8">
          <form onSubmit={handleSubmit} className="space-y-5">
            <div>
              <label className="block text-white/60 text-sm font-medium mb-2">Email</label>
              <div className="relative">
                <Mail size={16} className="absolute left-3.5 top-1/2 -translate-y-1/2 text-white/30" />
                <input type="email" value={email} onChange={e => setEmail(e.target.value)}
                  className="input pl-10" placeholder="andrei@example.com" required />
              </div>
            </div>

            <div>
              <label className="block text-white/60 text-sm font-medium mb-2">Parolă</label>
              <div className="relative">
                <Lock size={16} className="absolute left-3.5 top-1/2 -translate-y-1/2 text-white/30" />
                <input type={showPw ? 'text' : 'password'} value={password}
                  onChange={e => setPassword(e.target.value)}
                  className="input pl-10 pr-10" placeholder="••••••••" required />
                <button type="button" onClick={() => setShowPw(!showPw)}
                  className="absolute right-3.5 top-1/2 -translate-y-1/2 text-white/30 hover:text-white/60">
                  {showPw ? <EyeOff size={16} /> : <Eye size={16} />}
                </button>
              </div>
            </div>

            <button type="submit" disabled={loading} className="btn-primary w-full mt-2">
              {loading ? 'Se autentifică...' : 'Autentificare'}
            </button>
          </form>
        </div>

        <p className="text-center text-white/30 text-sm mt-4">
          Nu ai cont?{' '}
          <a href="/users" className="text-brand-400 hover:text-brand-300 transition-colors">
            Înregistrează-te
          </a>
        </p>
      </div>
    </div>
  )
}
