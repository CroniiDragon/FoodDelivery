import axios from 'axios'
import type {
  ApiResponse, UserResponse, CreateCustomerDto, CreateCourierDto, LoginDto,
  RestaurantResponse, CreateRestaurantDto, MenuItemResponse, CreateMenuItemDto,
  OrderResponse, CreateOrderDto, DeliveryStrategy,
  NotificationResponse, SendNotificationDto,
} from '../types'

const USER_API         = axios.create({ baseURL: 'https://localhost:7059/api' })
const RESTAURANT_API   = axios.create({ baseURL: 'https://localhost:7074/api' })
const ORDER_API        = axios.create({ baseURL: 'https://localhost:7014/api' })
const NOTIFICATION_API = axios.create({ baseURL: 'https://localhost:7129/api' })

const unwrap = <T>(res: { data: ApiResponse<T> }): T => res.data.data

export const userApi = {
  getAll:         () => USER_API.get<ApiResponse<UserResponse[]>>('/users').then(unwrap),
  getById:        (id: number) => USER_API.get<ApiResponse<UserResponse>>(`/users/${id}`).then(unwrap),
  createCustomer: (dto: CreateCustomerDto) => USER_API.post<ApiResponse<UserResponse>>('/users/customers', dto).then(unwrap),
  createCourier:  (dto: CreateCourierDto)  => USER_API.post<ApiResponse<UserResponse>>('/users/couriers', dto).then(unwrap),
  login:          (dto: LoginDto)          => USER_API.post<ApiResponse<UserResponse>>('/users/login', dto).then(unwrap),
  delete:         (id: number)             => USER_API.delete<ApiResponse<boolean>>(`/users/${id}`).then(unwrap),
}

export const restaurantApi = {
  getAll:             () => RESTAURANT_API.get<ApiResponse<RestaurantResponse[]>>('/restaurant').then(unwrap),
  getById:            (id: number) => RESTAURANT_API.get<ApiResponse<RestaurantResponse>>(`/restaurant/${id}`).then(unwrap),
  create:             (dto: CreateRestaurantDto) => RESTAURANT_API.post<ApiResponse<RestaurantResponse>>('/restaurant', dto).then(unwrap),
  delete:             (id: number) => RESTAURANT_API.delete<ApiResponse<boolean>>(`/restaurant/${id}`).then(unwrap),
  cloneBranch:        (id: number) => RESTAURANT_API.post<ApiResponse<RestaurantResponse>>(`/restaurant/${id}/clone-branch`).then(unwrap),
  cloneSeasonal:      (id: number) => RESTAURANT_API.post<ApiResponse<RestaurantResponse>>(`/restaurant/${id}/clone-seasonal`).then(unwrap),
  getMenu:            (restaurantId: number) => RESTAURANT_API.get<ApiResponse<MenuItemResponse[]>>(`/menuitems/restaurant/${restaurantId}`).then(unwrap),
  addMenuItem:        (dto: CreateMenuItemDto) => RESTAURANT_API.post<ApiResponse<MenuItemResponse>>('/menuitems', dto).then(unwrap),
  toggleAvailability: (id: number) => RESTAURANT_API.patch<ApiResponse<boolean>>(`/menuitems/${id}/toggle-availability`).then(unwrap),
}

export const orderApi = {
  getByCustomer: (customerId: number) => ORDER_API.get<ApiResponse<OrderResponse[]>>(`/orders/customer/${customerId}`).then(unwrap),
  getById:       (id: number)         => ORDER_API.get<ApiResponse<OrderResponse>>(`/orders/${id}`).then(unwrap),
  create:        (dto: CreateOrderDto) => ORDER_API.post<ApiResponse<OrderResponse>>('/orders', dto).then(unwrap),
  updateStatus:  (id: number, status: string) => ORDER_API.put<ApiResponse<boolean>>(`/orders/${id}/status`, JSON.stringify(status), { headers: { 'Content-Type': 'application/json' } }).then(unwrap),
  cancel:        (id: number) => ORDER_API.post<ApiResponse<boolean>>(`/orders/${id}/cancel`).then(unwrap),
  setStrategy:   (strategy: DeliveryStrategy) => ORDER_API.post<ApiResponse<string>>(`/orders/strategy/${strategy}`).then(unwrap),
}

export const notificationApi = {
  send:           (dto: SendNotificationDto) => NOTIFICATION_API.post<ApiResponse<NotificationResponse>>('/notifications', dto).then(unwrap),
  getByRecipient: (recipientId: number)      => NOTIFICATION_API.get<ApiResponse<NotificationResponse[]>>(`/notifications/recipient/${recipientId}`).then(unwrap),
}
