import type { LucideIcon } from 'lucide-react'
import { Loader2, PackageSearch } from 'lucide-react'

// ── StatCard ─────────────────────────────────────────────────────
interface StatCardProps {
  label: string
  value: string | number
  icon: LucideIcon
  color: string
  sub?: string
}
export function StatCard({ label, value, icon: Icon, color, sub }: StatCardProps) {
  return (
    <div className="card p-5 animate-slide-up">
      <div className="flex items-start justify-between">
        <div>
          <p className="text-white/50 text-sm font-medium">{label}</p>
          <p className="font-display text-3xl font-bold text-white mt-1">{value}</p>
          {sub && <p className="text-white/30 text-xs mt-1">{sub}</p>}
        </div>
        <div className={`w-11 h-11 rounded-xl flex items-center justify-center ${color}`}>
          <Icon size={22} className="text-white" />
        </div>
      </div>
    </div>
  )
}

// ── OrderStatusBadge ─────────────────────────────────────────────
const statusConfig: Record<string, { label: string; cls: string }> = {
  Pending:        { label: 'În așteptare',  cls: 'bg-yellow-500/15 text-yellow-400 border border-yellow-500/25' },
  Confirmed:      { label: 'Confirmată',    cls: 'bg-blue-500/15 text-blue-400 border border-blue-500/25' },
  Preparing:      { label: 'Se prepară',    cls: 'bg-purple-500/15 text-purple-400 border border-purple-500/25' },
  OutForDelivery: { label: 'La livrare',    cls: 'bg-brand-500/15 text-brand-400 border border-brand-500/25' },
  Delivered:      { label: 'Livrat',        cls: 'bg-green-500/15 text-green-400 border border-green-500/25' },
  Cancelled:      { label: 'Anulat',        cls: 'bg-red-500/15 text-red-400 border border-red-500/25' },
}
export function OrderStatusBadge({ status }: { status: string }) {
  const cfg = statusConfig[status] ?? { label: status, cls: 'bg-white/10 text-white/60' }
  return <span className={`badge ${cfg.cls}`}>{cfg.label}</span>
}

// ── RoleBadge ────────────────────────────────────────────────────
export function RoleBadge({ role }: { role: string }) {
  return (
    <span className={`badge ${role === 'Customer'
      ? 'bg-brand-500/15 text-brand-400 border border-brand-500/25'
      : 'bg-cyan-500/15 text-cyan-400 border border-cyan-500/25'}`}>
      {role === 'Customer' ? 'Client' : 'Curier'}
    </span>
  )
}

// ── LoadingSpinner ───────────────────────────────────────────────
export function LoadingSpinner({ text = 'Se încarcă...' }: { text?: string }) {
  return (
    <div className="flex flex-col items-center justify-center py-20 gap-3">
      <Loader2 size={32} className="text-brand-500 animate-spin" />
      <p className="text-white/40 text-sm">{text}</p>
    </div>
  )
}

// ── EmptyState ───────────────────────────────────────────────────
export function EmptyState({ title, sub }: { title: string; sub?: string }) {
  return (
    <div className="flex flex-col items-center justify-center py-20 gap-3">
      <PackageSearch size={40} className="text-white/20" />
      <p className="text-white/50 font-medium">{title}</p>
      {sub && <p className="text-white/30 text-sm">{sub}</p>}
    </div>
  )
}

// ── Modal ────────────────────────────────────────────────────────
export function Modal({ title, onClose, children }: {
  title: string; onClose: () => void; children: React.ReactNode
}) {
  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div className="absolute inset-0 bg-black/70 backdrop-blur-sm" onClick={onClose} />
      <div className="relative card w-full max-w-lg p-6 animate-slide-up shadow-2xl">
        <div className="flex items-center justify-between mb-5">
          <h3 className="font-display text-lg font-bold text-white">{title}</h3>
          <button onClick={onClose} className="text-white/40 hover:text-white transition-colors text-xl leading-none">✕</button>
        </div>
        {children}
      </div>
    </div>
  )
}
