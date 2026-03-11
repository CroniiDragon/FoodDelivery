// ── API Response wrapper (matches ApiResponse<T> from backend) ──
export interface ApiResponse<T> {
  success: boolean
  message: string
  data: T
  errors: string[]
}

// ── User types ───────────────────────────────────────────────────
export interface UserResponse {
  id: number
  name: string
  email: string
  phone: string
  role: 'Customer' | 'Courier'
  isActive: boolean
  createdAt: string
}

export interface CreateCustomerDto {
  name: string
  email: string
  phone: string
  password: string
  deliveryAddress: string
  city: string
}

export interface CreateCourierDto {
  name: string
  email: string
  phone: string
  password: string
  vehicleType: string
}

export interface LoginDto {
  email: string
  password: string
}

// ── Restaurant types ─────────────────────────────────────────────
export interface RestaurantResponse {
  id: number
  name: string
  address: string
  city: string
  cuisine: string
  isOpen: boolean
  menuItemCount: number
}

export interface CreateRestaurantDto {
  name: string
  address: string
  city: string
  cuisine: string
  phoneNumber: string
}

export interface MenuItemResponse {
  id: number
  name: string
  description: string
  finalPrice: number
  itemType: 'Food' | 'Drink'
  category: string
  isAvailable: boolean
  restaurantId: number
}

export interface CreateMenuItemDto {
  name: string
  description: string
  basePrice: number
  category: string
  itemType: string
  restaurantId: number
  calories?: number
  isVegetarian?: boolean
  volumeInLiters?: number
  isAlcoholic?: boolean
}

// ── Order types ──────────────────────────────────────────────────
export type OrderStatus =
  | 'Pending'
  | 'Confirmed'
  | 'Preparing'
  | 'OutForDelivery'
  | 'Delivered'
  | 'Cancelled'

export interface OrderItemResponse {
  itemName: string
  quantity: number
  unitPrice: number
  totalPrice: number
}

export interface OrderResponse {
  id: number
  customerId: number
  restaurantId: number
  status: OrderStatus
  totalPrice: number
  paymentMethod: string
  deliveryAddress: string
  createdAt: string
  items: OrderItemResponse[]
}

export interface CreateOrderItemDto {
  menuItemId: number
  itemName: string
  quantity: number
  unitPrice: number
}

export interface CreateOrderDto {
  customerId: number
  restaurantId: number
  deliveryAddress: string
  paymentMethod: string
  notes?: string
  items: CreateOrderItemDto[]
}

// ── Notification types ───────────────────────────────────────────
export interface NotificationResponse {
  id: number
  recipientId: number
  channel: string
  message: string
  isSent: boolean
  createdAt: string
}

export interface SendNotificationDto {
  recipientId: number
  recipientType: string
  channel: string
  message: string
}
