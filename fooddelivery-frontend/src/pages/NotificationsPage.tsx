import { useEffect, useState } from 'react'
import { Bell, Send, Mail, MessageSquare, Smartphone } from 'lucide-react'
import toast from 'react-hot-toast'
import { notificationApi, userApi } from '../services/api'
import { LoadingSpinner, EmptyState } from '../components/ui'
import type { NotificationResponse, UserResponse } from '../types'

export default function NotificationsPage() {
  const [notifications, setNotifications] = useState<NotificationResponse[]>([])
  const [users, setUsers]                 = useState<UserResponse[]>([])
  const [loading, setLoading]             = useState(false)
  const [recipientId, setRecipientId]     = useState<string>('')
  const [channel, setChannel]             = useState('Email')
  const [message, setMessage]             = useState('')
  const [sending, setSending]             = useState(false)

  useEffect(() => {
    userApi.getAll().then(setUsers).catch(() => {})
  }, [])

  const loadNotifications = async (uid: string) => {
    if (!uid) return
    setLoading(true)
    try { setNotifications(await notificationApi.getByRecipient(Number(uid))) }
    catch { toast.error('Eroare la încărcare.') }
    finally { setLoading(false) }
  }

  const handleSend = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!recipientId) { toast.error('Selectează un destinatar.'); return }
    const user = users.find(u => u.id === Number(recipientId))
    setSending(true)
    try {
      const n = await notificationApi.send({
        recipientId: Number(recipientId),
        recipientType: user?.role ?? 'Customer',
        channel,
        message,
      })
      setNotifications(p => [n, ...p])
      setMessage('')
      toast.success('Notificare trimisă!')
    } catch { toast.error('Eroare la trimitere.') }
    finally { setSending(false) }
  }

  const channelIcon = (ch: string) => {
    if (ch === 'Email') return <Mail size={14} className="text-blue-400" />
    if (ch === 'SMS')   return <MessageSquare size={14} className="text-green-400" />
    return <Smartphone size={14} className="text-brand-400" />
  }

  return (
    <div className="page-container">
      <div className="mb-6">
        <h1 className="section-title mb-0">Notificări</h1>
        <p className="text-white/40 text-sm mt-1">Trimite notificări utilizatorilor platformei</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">

        {/* Send form */}
        <div className="card p-6">
          <h2 className="font-display font-bold text-white mb-5 flex items-center gap-2">
            <Send size={18} className="text-brand-400" /> Trimite Notificare
          </h2>
          <form onSubmit={handleSend} className="space-y-4">
            <div>
              <label className="block text-white/60 text-sm mb-1.5">Destinatar</label>
              <select className="input" required value={recipientId}
                onChange={e => { setRecipientId(e.target.value); loadNotifications(e.target.value) }}>
                <option value="">— selectează utilizator —</option>
                {users.map(u => (
                  <option key={u.id} value={u.id}>{u.name} ({u.role})</option>
                ))}
              </select>
            </div>
            <div>
              <label className="block text-white/60 text-sm mb-1.5">Canal</label>
              <div className="grid grid-cols-3 gap-2">
                {['Email','SMS','Push'].map(ch => (
                  <button key={ch} type="button"
                    onClick={() => setChannel(ch)}
                    className={`py-2.5 rounded-xl border text-sm font-medium transition-all
                      ${channel === ch
                        ? 'bg-brand-500/20 border-brand-500/40 text-brand-400'
                        : 'bg-white/3 border-white/10 text-white/50 hover:text-white/70'}`}>
                    {ch}
                  </button>
                ))}
              </div>
            </div>
            <div>
              <label className="block text-white/60 text-sm mb-1.5">Mesaj</label>
              <textarea rows={4} required className="input resize-none" value={message}
                placeholder="Scrie mesajul notificării..."
                onChange={e => setMessage(e.target.value)} />
            </div>
            <button type="submit" disabled={sending} className="btn-primary w-full flex items-center justify-center gap-2">
              <Send size={15} /> {sending ? 'Se trimite...' : 'Trimite Notificare'}
            </button>
          </form>
        </div>

        {/* Notifications history */}
        <div className="card p-6">
          <h2 className="font-display font-bold text-white mb-5 flex items-center gap-2">
            <Bell size={18} className="text-brand-400" /> Istoric Notificări
          </h2>
          {loading ? <LoadingSpinner text="Se încarcă..." />
            : notifications.length === 0
            ? <EmptyState title="Nicio notificare" sub="Selectează un utilizator sau trimite prima notificare." />
            : <div className="space-y-3 max-h-96 overflow-y-auto pr-1">
                {notifications.map(n => (
                  <div key={n.id} className="bg-white/3 border border-white/5 rounded-xl p-3">
                    <div className="flex items-center justify-between mb-1.5">
                      <div className="flex items-center gap-2">
                        {channelIcon(n.channel)}
                        <span className="text-white/60 text-xs font-medium">{n.channel}</span>
                      </div>
                      <div className="flex items-center gap-2">
                        <span className={`w-2 h-2 rounded-full ${n.isSent ? 'bg-green-400' : 'bg-red-400'}`} />
                        <span className="text-white/30 text-xs">
                          {new Date(n.createdAt).toLocaleString('ro-RO', { hour: '2-digit', minute: '2-digit', day: '2-digit', month: '2-digit' })}
                        </span>
                      </div>
                    </div>
                    <p className="text-white/80 text-sm">{n.message}</p>
                  </div>
                ))}
              </div>
          }
        </div>
      </div>
    </div>
  )
}
