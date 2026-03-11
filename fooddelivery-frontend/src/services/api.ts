import axios from 'axios'
import type {
  ApiResponse, UserResponse, CreateCustomerDto, CreateCourierDto, LoginDto,
  RestaurantResponse, CreateRestaurantDto, MenuItemResponse, CreateMenuItemDto,
  OrderResponse, CreateOrderDto,
  NotificationResponse, SendNotificationDto,
} from '../types'

// ── Base URLs pentru cele 4 microservicii ────────────────────────
const USER_API         = axios.create({ baseURL: 'http://localhost:5043/api' })
const RESTAURANT_API   = axios.create({ baseURL: 'http://localhost:5032/api' })
const ORDER_API        = axios.create({ baseURL: 'http://localhost:5224/api' })
const NOTIFICATION_API = axios.create({ baseURL: 'http://localhost:5071/api' })

// Helper: extrage data din ApiResponse<T>
const unwrap = <T>(res: { data: ApiResponse<T> }): T => res.data.data

// ════════════════════════════════════════════════════════════════
//  USER SERVICE  →  http://localhost:5043
// ════════════════════════════════════════════════════════════════
export const userApi = {
  getAll: () =>
    USER_API.get<ApiResponse<UserResponse[]>>('/users').then(unwrap),

  getById: (id: number) =>
    USER_API.get<ApiResponse<UserResponse>>(`/users/${id}`).then(unwrap),

  createCustomer: (dto: CreateCustomerDto) =>
    USER_API.post<ApiResponse<UserResponse>>('/users/customers', dto).then(unwrap),

  createCourier: (dto: CreateCourierDto) =>
    USER_API.post<ApiResponse<UserResponse>>('/users/couriers', dto).then(unwrap),

  login: (dto: LoginDto) =>
    USER_API.post<ApiResponse<UserResponse>>('/users/login', dto).then(unwrap),

  delete: (id: number) =>
    USER_API.delete<ApiResponse<boolean>>(`/users/${id}`).then(unwrap),
}

// ════════════════════════════════════════════════════════════════
//  RESTAURANT SERVICE  →  http://localhost:5032
// ════════════════════════════════════════════════════════════════
export const restaurantApi = {
  getAll: () =>
    RESTAURANT_API.get<ApiResponse<RestaurantResponse[]>>('/restaurants').then(unwrap),

  getById: (id: number) =>
    RESTAURANT_API.get<ApiResponse<RestaurantResponse>>(`/restaurants/${id}`).then(unwrap),

  getOpenInCity: (city: string) =>
    RESTAURANT_API.get<ApiResponse<RestaurantResponse[]>>(`/restaurants/open/${city}`).then(unwrap),

  create: (dto: CreateRestaurantDto) =>
    RESTAURANT_API.post<ApiResponse<RestaurantResponse>>('/restaurants', dto).then(unwrap),

  delete: (id: number) =>
    RESTAURANT_API.delete<ApiResponse<boolean>>(`/restaurants/${id}`).then(unwrap),

  getMenu: (restaurantId: number) =>
    RESTAURANT_API.get<ApiResponse<MenuItemResponse[]>>(`/menuitems/restaurant/${restaurantId}`).then(unwrap),

  addMenuItem: (dto: CreateMenuItemDto) =>
    RESTAURANT_API.post<ApiResponse<MenuItemResponse>>('/menuitems', dto).then(unwrap),

  toggleAvailability: (menuItemId: number) =>
    RESTAURANT_API.patch<ApiResponse<boolean>>(`/menuitems/${menuItemId}/toggle-availability`).then(unwrap),
}

// ════════════════════════════════════════════════════════════════
//  ORDER SERVICE  →  http://localhost:5224
// ════════════════════════════════════════════════════════════════
export const orderApi = {
  getByCustomer: (customerId: number) =>
    ORDER_API.get<ApiResponse<OrderResponse[]>>(`/orders/customer/${customerId}`).then(unwrap),

  getById: (id: number) =>
    ORDER_API.get<ApiResponse<OrderResponse>>(`/orders/${id}`).then(unwrap),

  create: (dto: CreateOrderDto) =>
    ORDER_API.post<ApiResponse<OrderResponse>>('/orders', dto).then(unwrap),

  updateStatus: (id: number, status: string) =>
    ORDER_API.put<ApiResponse<boolean>>(`/orders/${id}/status`, JSON.stringify(status), {
      headers: { 'Content-Type': 'application/json' }
    }).then(unwrap),

  cancel: (id: number) =>
    ORDER_API.post<ApiResponse<boolean>>(`/orders/${id}/cancel`).then(unwrap),
}

// ════════════════════════════════════════════════════════════════
//  NOTIFICATION SERVICE  →  http://localhost:5071
// ════════════════════════════════════════════════════════════════
export const notificationApi = {
  send: (dto: SendNotificationDto) =>
    NOTIFICATION_API.post<ApiResponse<NotificationResponse>>('/notifications', dto).then(unwrap),

  getByRecipient: (recipientId: number) =>
    NOTIFICATION_API.get<ApiResponse<NotificationResponse[]>>(`/notifications/recipient/${recipientId}`).then(unwrap),
}
