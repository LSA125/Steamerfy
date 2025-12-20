<![CDATA[# Steamify

🎮 **[Play Now at steamify.xyz](https://steamify.xyz)**

> [!WARNING]
> **Azure Free Tier Notice**: The backend is hosted on Azure's free tier, which may take **30-60 seconds to wake up** on first load if it has been idle. Please be patient during initial connection!

## Overview

Steamify is a **real-time multiplayer trivia game** that generates quiz questions from players' Steam libraries. Compete with friends to see who knows their shared gaming experiences best! The game uses SignalR for low-latency WebSocket communication, enabling a seamless live quiz experience.

![Steamify Screenshot](image.png)

## ✨ Features

- **Steam Integration** — Fetches game data from players' Steam profiles to generate personalized questions
- **Real-time Multiplayer** — Uses SignalR WebSockets for instant synchronization across all players
- **Dynamic Lobbies** — Create private games or join public matchmaking
- **Live Scoring** — See answers and scores update in real-time as players respond
- **Turn Timer** — Visual countdown with bonus time mechanics for fast answers
- **Responsive Design** — Works on desktop and mobile browsers

## 🏗️ Architecture

| Component | Technology | Description |
|-----------|------------|-------------|
| **Frontend** | Angular 17+ | Single-page application with Material UI |
| **Backend** | .NET 8 | ASP.NET Core Web API with SignalR Hub |
| **Real-time** | Azure SignalR Service | Managed WebSocket infrastructure |
| **Hosting** | Azure Static Web Apps + Azure App Service | Serverless frontend, scalable backend |

## 📁 Project Structure

```
Steamerfy/
├── steamerfy.client/          # Angular frontend
│   ├── src/app/
│   │   ├── lobby/             # Lobby creation/joining UI
│   │   ├── game/              # Main gameplay components
│   │   ├── endscreen/         # Results and scoreboard
│   │   ├── models/            # TypeScript interfaces
│   │   └── game.service.ts    # SignalR client service
│   └── src/environments/      # Environment configs
│
├── Steamerfy.Server/          # .NET backend
│   ├── HubsAndSockets/        # SignalR GameHub
│   ├── Services/              # Game logic (GameService)
│   ├── Models/                # Data models (Lobby, Player, Question)
│   ├── ExternalApiHandlers/   # Steam API integration
│   └── Factory/               # Question generation
│
└── .github/workflows/         # CI/CD pipelines
```

## 🚀 Getting Started

### Prerequisites

- [Node.js 18+](https://nodejs.org/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

### Running Locally

**Option 1: Visual Studio (Recommended)**
1. Clone the repository
2. Open `Steamerfy.sln` in Visual Studio
3. Press **F5** to run — both frontend and backend will start together

**Option 2: Command Line**

```bash
# Terminal 1: Start the backend
cd Steamerfy.Server
dotnet run

# Terminal 2: Start the frontend
cd steamerfy.client
npm install
npm start
```

The frontend will be available at `http://localhost:4200` and will proxy API requests to the backend.

### Building for Production

```bash
# Build frontend
cd steamerfy.client
npm run build

# Build backend
cd Steamerfy.Server
dotnet publish -c Release
```

## 🐳 Docker

Docker support is available for containerized deployment:

```bash
docker-compose up
```

## 🔧 Configuration

### Environment Variables

| Variable | Description |
|----------|-------------|
| `Azure:SignalR:ConnectionString` | Azure SignalR Service connection string |
| `Steam:ApiKey` | Steam Web API key (optional, for additional features) |

### Frontend Environment

Edit `steamerfy.client/src/environments/environment.prod.ts` to configure production settings.

## 📄 License

This project is for educational and portfolio purposes.

---

Built using Angular & .NET
]]>
