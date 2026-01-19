# Project Prueba

## Manager-Based Environment Design, Import Errors, and Analysis

### 1. Project Overview

The **Prueba** project explores a *manager-based* reinforcement learning environment in **Isaac Lab**, targeting an end-effector pose tracking task using a **UR robotic arm with a gripper**. Unlike the Drone project, this environment follows the **ManagerBasedRLEnv** paradigm, relying heavily on modular MDP components (actions, observations, rewards, terminations, curriculum, and events).

Conceptually, this project aligns very closely with official Isaac Lab manipulation examples and represents an advanced and well-structured attempt to implement a realistic manipulation task. However, execution is blocked by Python package import and initialization issues that occur before the environment can be instantiated.

---

### 2. Environment Architecture

#### 2.1 Environment Type

* **Base class**: `ManagerBasedRLEnvCfg`
* **Primary configuration class**: `PruebaEnvCfg`
* **Play configuration**: `PruebaEnvCfg_PLAY`

The environment is defined entirely through configuration classes, following Isaac Lab’s declarative MDP design philosophy.

---

### 3. Scene and Asset Configuration

#### 3.1 Scene Definition

The scene is defined via `PruebaSceneCfg` and includes:

* A ground plane
* A UR robot arm with gripper (`UR_GRIPPER_CFG`)
* A table asset loaded from NVIDIA Nucleus
* Dome lighting for visualization

The scene supports large-scale vectorization with:

* **2000 parallel environments** for training
* Adjustable spacing between environments

#### 3.2 Asset Sources

Assets are resolved dynamically from the NVIDIA Nucleus server using:

* `NVIDIA_NUCLEUS_DIR`
* `ISAAC_NUCLEUS_DIR`

This introduces an external dependency on correct Nucleus configuration and availability.

---

### 4. MDP Specification

#### 4.1 Actions

* Joint position control over six UR arm joints
* Actions scaled and offset relative to default joint positions
* Debug visualization enabled for action inspection

#### 4.2 Commands

* Uniformly sampled end-effector pose commands
* Commands resampled every 4 seconds
* Position and orientation ranges constrained to a feasible workspace

#### 4.3 Observations

The policy observation group includes:

* Relative joint positions (with noise)
* Relative joint velocities (with noise)
* Current pose command
* Previous action

Observation corruption is enabled during training and disabled during play.

#### 4.4 Rewards

Reward terms encourage accurate tracking of the commanded end-effector pose:

* End-effector position tracking (coarse and fine-grained)
* End-effector orientation tracking
* Penalties on joint velocity magnitude
* Penalties on action rate

A curriculum gradually increases penalty weights to stabilize learning.

#### 4.5 Terminations and Events

* Episodes terminate only on time-out
* Joint states are randomized on reset to improve robustness

---

### 5. Simulation Settings

Key simulation parameters:

* Physics timestep: 60 Hz (`dt = 1/60`)
* Control decimation: 2
* Episode length: 3 seconds
* Viewer camera positioned for global scene visibility

---

### 6. Execution Command

The environment is intended to be launched using:

```
(env_isaaclab) usuario@pc:~/Documents/Github/SkySpec/project_drone/IsaacLab/Testing/Prueba$ \
python scripts/skrl/train.py --task Template-Prueba-v0
```

Execution relies on correct task discovery and package imports via Isaac Lab’s task registry.

---

### 7. Runtime Error Encountered

During startup, the following critical error occurs:

```
ImportError: cannot import name 'agents' from partially initialized module 'Prueba.tasks.manager_based'
(most likely due to a circular import)
```

This error appears during dynamic package discovery when Isaac Lab recursively imports task modules.

---

### 8. Root Cause Analysis

The failure is caused by a **circular import and package structure issue**:

* The package `Prueba.tasks.manager_based` attempts to import an `agents` submodule
* At import time, the parent package is only partially initialized
* Python raises an ImportError due to the unresolved circular dependency

This indicates that:

* The project directory structure mirrors Isaac Lab templates, but not all expected submodules (e.g., `agents`) are fully defined or correctly placed
* The `__init__.py` files trigger eager imports that conflict with Python’s module initialization order

---

### 9. Why This Error Is Subtle

This issue is non-trivial because:

* The environment configuration (`PruebaEnvCfg`) itself is syntactically and semantically valid
* The failure occurs **before** Hydra loads the environment configuration
* The error is triggered by Isaac Lab’s automatic task discovery mechanism, not by user code execution

As a result, the simulation initializes successfully, but crashes during task registration.

---

### 10. Secondary Warnings

Following the exception, several warnings appear related to:

* OmniGraph category removal
* Fabric state cleanup
* PhysX stage detachment

These warnings are shutdown artifacts and are **not the primary cause** of the failure.

---

### 11. Lessons Learned from Project Prueba

* Manager-based environments are highly sensitive to Python package layout
* Circular imports can silently emerge from template-based directory structures
* Task discovery executes code eagerly, leaving little margin for partial initialization errors
* Correct environment logic is insufficient without strict adherence to Isaac Lab’s task packaging conventions

---

### 12. Project Status

**Current state**: Fully defined manager-based environment blocked at import time.

**Blocking issue**: Circular import involving `Prueba.tasks.manager_based` and a missing or prematurely imported `agents` module.

Despite being non-executable, this project demonstrates a correct and advanced use of Isaac Lab’s manager-based MDP framework.
