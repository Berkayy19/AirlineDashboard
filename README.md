# 🚀 Setup Guide for Airline Dashboard

Follow these steps to get the project running on your machine:

---

## ✅ Requirements
Make sure you have these tools installed:

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)  
- [Node.js (LTS)](https://nodejs.org/en/download/) + npm  
- [Angular CLI](https://angular.dev/cli)  
  ```bash
  npm install -g @angular/cli
  ```
- Git  

---

## 🔧 Setup Instructions

### 1. Clone the Repository
```bash
git clone https://github.com/Berkayy19/AirlineDashboard.git
cd AirlineDashboard
```

### 2. Configure Lufthansa API Credentials
1. Register at the Lufthansa Developer Portal: https://developer.lufthansa.com  
2. Create an app and get your Client ID and Client Secret.  
3. Open `FlightBackend/appsettings.json` and add your credentials:

```json
{
  "Lufthansa": {
    "ClientId": "YOUR_CLIENT_ID_HERE",
    "ClientSecret": "YOUR_CLIENT_SECRET_HERE"
  }
}
```

⚠️ Do not commit real credentials to GitHub.

---

### 3. Run the Backend
```bash
cd FlightBackend
dotnet run
```

Backend runs on:  
👉 https://localhost:5286 (check `launchSettings.json` for the port)

---

### 4. Run the Frontend
```bash
cd Frontend
npm install
ng serve
```

Frontend runs on:  
👉 http://localhost:4200  

---

✅ Now you can search for Lufthansa flights by route, date, or flight number using the Angular frontend.
