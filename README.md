# ✈️ FlightBoard System

![.NET](https://img.shields.io/badge/.NET%208.0-512BD4?logo=dotnet&logoColor=white)
![React](https://img.shields.io/badge/React-20232A?logo=react&logoColor=61DAFB)
![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?logo=typescript&logoColor=white)
![SQLite](https://img.shields.io/badge/SQLite-07405E?logo=sqlite&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-512BD4?logo=signalr&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?logo=docker&logoColor=white)

---

## 📌 Overview
**FlightBoard** is a professional, full-stack, real-time flight board management system.  
It is designed with **Clean Architecture** principles, **real-time updates** via SignalR, and a **generic CRUD infrastructure** for rapid feature expansion.

---

## 🚀 Features
- **Real-Time Updates** – Broadcast changes via SignalR without polling.
- **Server-Side Status Calculation** – A single source of truth for flight status.
- **Advanced Filtering & Search** – Filter by status and/or destination.
- **Persistent Data** – SQLite + EF Core migrations.
- **Generic CRUD Layer** – Add new modules in minutes.
- **Unit Testing** – Business logic covered with xUnit + Moq.
- **Modern UI** – Responsive Material UI + Redux Toolkit + React Query.
- **Dockerized** – Ready to run anywhere.

---

## 🛠 Tech Stack

**Backend**  
- Swashbuckle.AspNetCore (Swagger)
- ASP.NET Core Web API (C#)
- Entity Framework Core (SQLite)
- SignalR
- FluentValidation
- xUnit + Moq
- Serilog (structured logging)

**Frontend**  
- React, TypeScript, Vite
- Redux Toolkit, React Query
- Material UI, @microsoft/signalr
- React Hook Form + Yup
- Axios, Day.js, Framer Motion
- TanStack Query (server state)
- Material UI
- SASS

**DevOps**  
- Docker & Docker Compose

---

## 🏗 Architectural Choices

**Backend**  
- Clean Architecture: Separation into API, DAL, Infrastructure, Business Logic, and Real-Time layers.
- Generic CRUD: CrudControllerBase<T> + CrudEfDal<T> + UnitOfWork for rapid module creation.
- SignalR Hub: Pushes real-time create/delete/status updates.
- Middleware: Centralized error handling with HttpExceptionMiddleware.

**Frontend**  
- Pages/Components: Modular UI (FlightBoard page + reusable components).
- State Management: Redux Toolkit for UI state, React Query for server state.
- Providers: AppProvider wraps Redux + React Query.
- Hooks: Custom hooks for logic encapsulation.

---

## ⚙️ Setup

## Manual Setup
**Backend**  
 - cd Server/FlightBoard.Api
 - dotnet build
 - dotnet run

**Frontend**  
 - cd Client
 - pnpm install
 - pnpm dev

### Docker Setup
- docker compose up --build 

---

## Running Tests

**Backend**  
- cd Server/FlightBoard.Tests
- dotnet test

---

## Final Notes
 CRUD foundation, modular Clean Architecture layers, and real-time communication via SignalR ensures that new features can be added quickly without sacrificing code quality. The frontend’s separation into Pages, Components, and Hooks, along with unified state management (Redux Toolkit + React Query), provides a consistent and maintainable development experience.

 You can run the system either manually by starting the backend and frontend separately, or via Docker Compose for a fully containerized setup. The solution also includes unit testing for core business rules, ensuring that critical logic remains reliable as the project grows.

