import { useState } from 'react'
import { notificationApi } from '../services/api'
import type { NotificationResponse, SendNotificationDto } from '../types'

const CHANNELS = ['Email', 'SMS', 'Push']

export default function NotificationsPage() {
  const [notifications, setNotifications] = useState<NotificationResponse[]>([])
  const [loading, setLoading]             = useState(false)
  const [recipientId, setRecipientId]     = useState(1)
  const [form, setForm] = useState<SendNotificationDto>({
    recipientId: 1, recipientType: 'Customer', channel: 'Push', message: ''
  })
  const [sendMsg, setSendMsg] = useState('')
  const [error, setError]     = useState('')

  async function fetchNotifications() {
    setLoading(true)
    setError('')
    try {
      setNotifications(await notificationApi.getByRecipient(recipientId))
    } catch {
      setError('Could not load notifications.')
    } finally {
      setLoading(false)
    }
  }

  async function handleSend() {
    setSendMsg('')
    setError('')
    try {
      await notificationApi.send({ ...form, recipientId })
      setSendMsg('✓ Notification sent.')
      fetchNotifications()
    } catch (e: any) {
      setError(e?.response?.data?.errors?.[0] ?? 'Send failed — check Chain validation.')
    }
  }

  return (
    <div className="p-6 space-y-6">
      <h1 className="text-2xl font-bold text-white">Notifications</h1>

      {/* Send form */}
      <div className="bg-white/5 border border-white/10 rounded-xl p-5 space-y-4">
        <h2 className="text-white font-semibold">Send Notification</h2>
        <p className="text-xs text-gray-500">Passes through Chain of Responsibility before sending.</p>

        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className="text-xs text-gray-400 block mb-1">Recipient ID</label>
            <input
              type="number" min={1}
              value={recipientId}
              onChange={e => setRecipientId(Number(e.target.value))}
              className="w-full px-3 py-2 rounded bg-white/10 text-white text-sm border border-white/20 focus:outline-none focus:border-orange-500"
            />
          </div>
          <div>
            <label className="text-xs text-gray-400 block mb-1">Recipient Type</label>
            <select
              value={form.recipientType}
              onChange={e => setForm(f => ({ ...f, recipientType: e.target.value }))}
              className="w-full px-3 py-2 rounded bg-white/10 text-white text-sm border border-white/20 focus:outline-none focus:border-orange-500"
            >
              <option value="Customer">Customer</option>
              <option value="Courier">Courier</option>
            </select>
          </div>
        </div>

        <div>
          <label className="text-xs text-gray-400 block mb-1">Channel</label>
          <div className="flex gap-2">
            {CHANNELS.map(c => (
              <button
                key={c}
                onClick={() => setForm(f => ({ ...f, channel: c }))}
                className={`px-4 py-2 rounded text-sm font-medium transition-colors ${
                  form.channel === c
                    ? 'bg-orange-500 text-white'
                    : 'bg-white/10 text-gray-300 hover:bg-white/20'
                }`}
              >
                {c}
              </button>
            ))}
          </div>
        </div>

        <div>
          <label className="text-xs text-gray-400 block mb-1">Message</label>
          <textarea
            rows={3}
            value={form.message}
            onChange={e => setForm(f => ({ ...f, message: e.target.value }))}
            placeholder="Enter notification message..."
            className="w-full px-3 py-2 rounded bg-white/10 text-white text-sm border border-white/20 focus:outline-none focus:border-orange-500 resize-none"
          />
        </div>

        <button
          onClick={handleSend}
          disabled={!form.message.trim()}
          className="px-5 py-2 bg-orange-500 hover:bg-orange-600 disabled:opacity-40 text-white rounded text-sm font-medium transition-colors"
        >
          Send
        </button>

        {sendMsg && <p className="text-green-400 text-sm">{sendMsg}</p>}
        {error   && <p className="text-red-400 text-sm">{error}</p>}
      </div>

      {/* History */}
      <div className="space-y-3">
        <div className="flex items-center gap-3">
          <h2 className="text-white font-semibold">History for recipient #{recipientId}</h2>
          <button
            onClick={fetchNotifications}
            className="px-3 py-1 bg-white/10 hover:bg-white/20 text-gray-300 rounded text-xs"
          >
            Load
          </button>
        </div>

        {loading ? (
          <p className="text-gray-400 animate-pulse text-sm">Loading...</p>
        ) : notifications.length === 0 ? (
          <p className="text-gray-500 text-sm">No notifications found.</p>
        ) : (
          <div className="space-y-2">
            {notifications.map(n => (
              <div key={n.id} className="flex items-start gap-3 bg-white/5 border border-white/10 rounded-lg p-3">
                <span className={`mt-0.5 px-2 py-0.5 rounded text-xs font-medium shrink-0 ${
                  n.channel === 'Email' ? 'bg-blue-500/20 text-blue-300' :
                  n.channel === 'SMS'   ? 'bg-green-500/20 text-green-300' :
                                          'bg-purple-500/20 text-purple-300'
                }`}>
                  {n.channel}
                </span>
                <p className="text-sm text-gray-300 flex-1 break-all">{n.message}</p>
                <div className="text-right shrink-0">
                  <span className={`text-xs ${n.isSent ? 'text-green-400' : 'text-red-400'}`}>
                    {n.isSent ? '✓ Sent' : '✗ Failed'}
                  </span>
                  <p className="text-xs text-gray-500 mt-0.5">
                    {new Date(n.createdAt).toLocaleTimeString()}
                  </p>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
