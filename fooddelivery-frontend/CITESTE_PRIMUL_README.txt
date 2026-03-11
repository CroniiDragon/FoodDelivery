╔══════════════════════════════════════════════════════════════════╗
║        FRONTEND — FoodDelivery Platform                         ║
║        React + TypeScript + Tailwind CSS                        ║
╚══════════════════════════════════════════════════════════════════╝

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 PASUL 1 — Instalează Node.js (dacă nu ai)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

 1. Du-te la: https://nodejs.org
 2. Descarcă versiunea LTS (Long Term Support) — butonul verde
 3. Rulează installerul, next → next → finish
 4. Verificare: deschide Command Prompt (cmd) și scrie:
       node --version     → trebuie să afișeze v20.x.x sau mai nou
       npm --version      → trebuie să afișeze 10.x.x sau mai nou


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 PASUL 2 — Copierea fișierelor frontend
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

 Copiază folderul "fooddelivery-frontend" din arhivă în:
    C:\Projects\FoodDelivery\  (lângă folderul soluției .sln)

 Structura finală trebuie să arate așa:
    C:\Projects\FoodDelivery\
    ├── FoodDelivery.sln
    ├── src\               ← proiectele C#
    └── fooddelivery-frontend\   ← FRONTUL (acest folder)


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 PASUL 3 — Instalarea dependențelor React
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

 Deschide Command Prompt și navighează în folderul frontend:

    cd C:\Projects\FoodDelivery\fooddelivery-frontend

 Instalează toate pachetele (o singură dată):

    npm install

 Durează 1-2 minute. La final vei vedea un folder "node_modules".


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 PASUL 4 — Pornire aplicație completă (back + front)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

 Ai nevoie de 2 ferestre deschise simultan:

 FEREASTRA 1 — Visual Studio 2022:
    → Apasă F5 (sau butonul Start cu Multiple Startup Projects)
    → Pornesc cele 4 microservicii pe porturile 5001, 5002, 5003, 5004

 FEREASTRA 2 — Command Prompt în folderul frontend:
    npm run dev
    → Frontend pornește pe http://localhost:3000

 Deschide browserul la: http://localhost:3000


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 CE VEDE UTILIZATORUL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

 http://localhost:3000/          → Dashboard cu statistici
 http://localhost:3000/restaurants → Lista restaurante + meniu
 http://localhost:3000/orders      → Comenzi + plasare comandă nouă
 http://localhost:3000/users       → Înregistrare clienți și curieri
 http://localhost:3000/notifications → Trimitere notificări


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 STRUCTURA FIȘIERELOR FRONTEND
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

 fooddelivery-frontend/
 ├── package.json          ← lista dependențelor npm
 ├── vite.config.ts        ← configurare server dev (port 3000)
 ├── tailwind.config.js    ← configurare Tailwind CSS
 ├── tsconfig.json         ← configurare TypeScript
 ├── index.html            ← pagina HTML de bază
 │
 └── src/
     ├── main.tsx          ← punctul de intrare React
     ├── App.tsx           ← router principal (toate rutele)
     ├── index.css         ← stiluri globale + Tailwind
     │
     ├── types/
     │   └── index.ts      ← TOATE tipurile TypeScript
     │                        (corespund DTO-urilor din backend)
     │
     ├── services/
     │   └── api.ts        ← TOATE apelurile HTTP către microservicii
     │                        userApi, restaurantApi, orderApi, notificationApi
     │
     ├── hooks/
     │   └── useAuth.ts    ← starea de autentificare (user logat)
     │
     ├── components/
     │   ├── layout/
     │   │   └── Layout.tsx    ← Sidebar + Header (comun tuturor paginilor)
     │   └── ui/
     │       └── index.tsx     ← componente reutilizabile:
     │                            StatCard, OrderStatusBadge, RoleBadge,
     │                            LoadingSpinner, EmptyState, Modal
     │
     └── pages/
         ├── DashboardPage.tsx      ← statistici generale
         ├── LoginPage.tsx          ← autentificare
         ├── RestaurantsPage.tsx    ← restaurante + meniu
         ├── OrdersPage.tsx         ← comenzi
         ├── UsersPage.tsx          ← clienți + curieri
         └── NotificationsPage.tsx  ← trimitere + istoric notificări


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 CUM COMUNICĂ FRONTUL CU BACK-ENDUL
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

 Frontend (React)           Backend (ASP.NET Core)
 localhost:3000     →  →  → localhost:5001  OrderService
                    →  →  → localhost:5002  RestaurantService
                    →  →  → localhost:5003  UserService
                    →  →  → localhost:5004  NotificationService

 Toate apelurile sunt definite în: src/services/api.ts
 Dacă schimbi porturile în backend, actualizează și api.ts


━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
 ERORI FRECVENTE
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

 "npm: command not found"
    → Node.js nu e instalat. Vezi Pasul 1.

 "Failed to fetch" sau "Network Error" în browser
    → Microserviciile nu rulează. Pornește Visual Studio (F5) primul.

 "CORS error" în consolă browser
    → Verifică că în fiecare Program.cs din backend ai:
      app.UseCors("AllowAll") și builder.Services.AddCors(...)

 Pagina albă în browser
    → Verifică consola browser (F12) pentru erori TypeScript

━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
