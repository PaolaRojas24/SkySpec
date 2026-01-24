# SkySpec's Repository

This is a research and development repository focused on **autonomous drone navigation using Reinforcement Learning (RL)** and **high-fidelity simulation environments**.
The project explores how simulation platforms such as **NVIDIA Isaac Sim**, **Isaac Lab**, **Unity**, and **Blender** can be leveraged to design, train, and evaluate intelligent drone systems for real-world applications.

This repository serves as a **centralized workspace** for simulation assets, experiments, technical documentation, reports, and presentations developed as part of the SkySpec initiative.

---

## Project Goals

The main objectives of this project are:

* Develop drone simulation environments suitable for Reinforcement Learning.
* Train and evaluate RL agents for navigation and control tasks.
* Compare different simulation platforms and workflows.
* Create reusable 3D drone assets and simulation pipelines.
* Document experiments, findings, and lessons learned in a structured manner.

While the long-term vision includes real-world deployment (e.g., inspection, monitoring, emergency response), this repository currently focuses on **simulation-based development and experimentation**.

---

## Repository Structure

The repository is organized into documentation, simulation projects, and platform-specific experiments.

```text
.
├── Documents
│   ├── OIST-photos
│   ├── one-pagers
│   └── weekly-presentations
│
├── project_drone
│   ├── Blender
│   │   ├── UAV.blend
│   │   ├── drone.blend
│   │   ├── drone.obj
│   │   ├── drone.usd
│   │   └── textures
│   │       └── uav_quadcopter_c.jpg
│   │
│   ├── Documents
│   │   ├── Presentations
│   │   └── Reports
│   │
│   ├── IsaacLab
│   │   ├── Courses
│   │   │   ├── Cartpole
│   │   │   └── Reach
│   │   └── Testing
│   │       ├── Drone
│   │       ├── quadcopter
│   │       └── README.md
│   │
│   ├── IsaacSim
│   │   ├── Courses
│   │   └── Drone
│   │
│   └── Unity
│       ├── 3Ddrone
│       └── sensor
│
└── README.md
```

---

## Folder Overview

### `/Documents`

High-level project documentation and communication material, including:

* Photos and visual references.
* One-page summaries of the conferences attended.
* Weekly presentations and progress updates.

---

### `/project_drone`

Core technical workspace for drone simulation and experimentation.

#### `Blender/`

* 3D drone models and assets.
* Exports in multiple formats (`.blend`, `.obj`, `.usd`) for compatibility with different simulators.
* Textures used for visualization and rendering.

#### `Documents/`

* **Reports**: Technical documents covering Reinforcement Learning, simulation platforms, and final project reports.
* **Presentations**: Slides comparing platforms and presenting simulation results.

#### `IsaacLab/`

* Reinforcement Learning experiments using **Isaac Lab**.
* Includes course-based examples (e.g., Cartpole, Reach) and custom drone testing environments.
* Used to prototype RL workflows and evaluate control strategies.

#### `IsaacSim/`

* Simulation environments built with **NVIDIA Isaac Sim**.
* Focused on physics-accurate drone simulation and environment setup.
* Includes both learning materials and custom drone scenes.

#### `Unity/`

* Experiments using **Unity** for drone simulation and sensor testing.
* Used primarily for comparison with NVIDIA Omniverse-based workflows.

---

## Technologies Used

* **NVIDIA Isaac Sim**
* **Isaac Lab**
* **Reinforcement Learning (PPO and related methods)**
* **Blender (3D modeling)**
* **Unity**
* **Python**
* **USD (Universal Scene Description)**
