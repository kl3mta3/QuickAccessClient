# QuickAccessClient

**QuickAccessClient** is a lightweight, secure Windows desktop companion app designed to integrate seamlessly with the [Quick Access Portal](https://github.com/kl3mta3/QuickAccessPortal). It provides fast logins, user session tracking, and persistent user configuration — all while minimizing exposure of sensitive credentials.

---

## Purpose

The client offers a streamlined, secure way to interact with the portal from a user's machine, including:

- Secure login using a hashed API key (username + password)
- Local settings storage (e.g., "Remember Me" functionality)
- Lightweight UI for connecting and authenticating without exposing raw credentials
- Designed for internal IT teams who want quick access without re-authenticating every time

---

## Features

- **Secure API Key Auth**
  - No plain usernames or passwords sent.
  - API key is generated via SHA-512.
  - Passwords only stored as PBKDF2 hash for verification.


- **Persistent Username Storage**
  - Remembers the last used username across sessions.

- **Lightweight WPF UI**
  - Modern Windows experience for logging in with minimal steps.
  - Password field is masked and input validated.

- **Fail-Safe Handling**
  - Clear UI messages on login errors
  - Prevents empty inputs, malformed usernames, or insecure passwords

---

## Structure

- `LogonWindow.xaml` – Login form with username, password, and "Remember me" checkbox
- `Config.cs` – Contains hash logic (`SHA512`) and helper methods
- `Settings.settings` – Persists saved username across app restarts
- `Logon_Click` – Sends hashed username/password + API key to server for verification
- `MainWindow` – Provides Client List and access to Info
- `Go To KBA` – Goes to the stored URL
- `Open RDP` – Opens the remote Application based on path and arguments provided.
- `Report Issue` – Sends a report of an Issue with a Remote to the Quick Access Portal and Dashboard.

---

## Requirements

- .NET 8.0 or later  
- `System.Net.Http`
- Runs on Windows (built with WPF)

---


## Security Notes

This app never transmits the plaintext password — all authentication is done via pre-hashed values. Still, you should always:

- Serve your API over HTTPS
- Avoid storing plaintext anywhere, even in logs
- Never hardcode secrets or credentials

---

## About This Project
This application was designed and built during my time working in internal IT support, and while used in a real environment, 
all code and architectural decisions are my own. It is shared here as part of my personal portfolio to demonstrate full-stack 
architecture, secure authentication flows, and a responsive multi-role UI system.
