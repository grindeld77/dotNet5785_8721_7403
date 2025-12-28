# 🚑 Emergency Response Management System (MDA)

![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![WPF](https://img.shields.io/badge/WPF-Windows_Presentation_Foundation-0078D7?style=for-the-badge&logo=windows&logoColor=white)
![Architecture](https://img.shields.io/badge/Architecture-MVVM_%7C_Layered-4BC51D?style=for-the-badge)
![Status](https://img.shields.io/badge/Status-Completed-success?style=for-the-badge)

> **A high-performance simulation and dispatcher system for emergency services.** > Built with **C# & WPF**, focusing on real-time data processing, multithreading, and clean architecture.

---

## 📌 Overview
This system is designed to manage the full lifecycle of emergency calls and volunteer dispatching for Magen David Adom (MDA). Unlike standard management apps, it features a **custom simulation engine** that mimics real-world time progression, creating dynamic scenarios where calls expire, volunteers move, and risk levels escalate automatically.

### 🔥 Key Highlights
* **Real-Time Simulation:** A background thread engine controls the system clock, updating call urgencies and statuses live.
* **Smart UI Visualization:** Uses advanced WPF `Converters` and `DataTriggers` to change interface colors dynamically based on call risk levels (Green 🟢 ➜ Red 🔴).
* **MVVM Architecture:** Strict separation between Logic (BL), Data (DAL), and UI (PL) for maximum scalability.

---

## 🖼️ Visual Showcase

| **Live Dashboard** | **Smart Call List** |
|:---:|:---:|
| Control the simulation clock and view real-time stats. | Dynamic progress bars indicating urgency. |
| *Add `dashboard.png` here* | *Add `calls.png` here* |

| **Volunteer Dispatch** | **Risk Management** |
|:---:|:---:|
| Master-Detail view for assigning volunteers. | Auto-detection of overdue/risk calls. |
| *Add `volunteer.png` here* | *Add `risk.png` here* |

> **Note:** Screenshots are available in the `screenshots` folder.

---

## 🛠️ Tech Stack & Features

### ⚙️ Technologies
* **Language:** C# 12 / .NET 8.0
* **UI Framework:** WPF (Windows Presentation Foundation)
* **Design Pattern:** MVVM (Model-View-ViewModel)
* **Data Persistence:** XML with LINQ (DAL implementation)
* **Multithreading:** `Task` / `BackgroundWorker` for simulation

### ✨ Core Features
| Feature | Description |
| :--- | :--- |
| **Volunteers** | Management of personnel, location tracking, and "Active/Inactive" status toggles. |
| **Emergency Calls** | Create, assign, and track calls. Auto-calculation of distances to nearest volunteers. |
| **Smart Risk** | Algorithms that calculate call urgency based on time elapsed vs. max processing time. |
| **History Log** | Full audit trail of completed and cancelled events. |

---

## 🚀 Getting Started

1.  **Clone the Repository**
    ```bash
    git clone [https://github.com/shimon2005/MDA-Volunteer-Manager.git](https://github.com/shimon2005/MDA-Volunteer-Manager.git)
    ```
2.  **Open in Visual Studio**
    * Launch `dotNet5785_8721_7403.sln`.
    * Ensure `.NET 8.0 SDK` is installed.
3.  **Build & Run**
    * Set `PL` as the startup project.
    * Press `F5` to run.

### 🔑 Default Credentials
* **Manager Access:** `shimon78900`
* **Volunteer Access:** `grinfeld770`

---

## 🏗️ Architecture Design

The solution follows a strict **N-Tier Architecture**:

```mermaid
graph TD;
    PL[🖥️ PL - Presentation Layer] -->|Calls| BL[⚙️ BL - Business Logic];
    BL -->|Reads/Writes| DAL[💾 DAL - Data Access Layer];
    DAL -->|Persists| XML[(XML Files)];
    
    subgraph Core Logic
    BL -- "Calculations & Rules" --> BL
    end

---
## 👨‍💻 Authors

- [Shimon Khakshour](https://github.com/shimon2005)
- [Levi Grinfeld](https://github.com/grindeld77)

Course: Windows Systems Mini Project, 2025

Instructor: [Yair Goldstein](https://github.com/yair-go)
