# Project Drone

## Environment Design, Execution Errors, and Analysis

### 1. Project Overview

The **Drone** project aims to implement a reinforcement learning environment for a quadrotor UAV using **Isaac Lab** and **SKRL**. The environment is designed following the *DirectRLEnv* paradigm, with continuous control over thrust and moments applied directly to the drone’s base body. The long-term goal is to learn stable hovering and goal-reaching behaviors in 3D space.

This project represents the most complete and conceptually correct attempt within the overall drone RL effort, as it aligns closely with official Isaac Lab quadcopter examples. However, execution was ultimately blocked by task registration and configuration mismatches.

---

### 2. Environment Architecture

#### 2.1 Environment Type

* **Base class**: `DirectRLEnv`
* **Configuration class**: `QuadcopterEnvCfg`
* **Runtime class**: `QuadcopterEnv`

The environment directly applies forces and torques to the quadcopter body using Isaac Lab’s low-level physics API, bypassing joint-level actuation.

#### 2.2 Simulation and Scene Setup

Key characteristics:

* Physics timestep: 100 Hz (`dt = 1/100`)
* Vectorized simulation with **4096 parallel environments**
* Flat terrain using `TerrainImporterCfg`
* Dome lighting for visualization

The scene is replicated using Fabric cloning for performance, which is standard practice in Isaac Lab for large-scale RL training.

---

### 3. Robot Model and Control Strategy

#### 3.1 Robot Asset

* Robot configuration imported from `CRAZYFLIE_CFG`
* Treated as an **Articulation**, but controlled as a rigid body
* Root body identified via `find_bodies("body")`

#### 3.2 Action Space

* **4-dimensional continuous action space**

  * Action[0]: total thrust (normalized)
  * Action[1:4]: body moments (roll, pitch, yaw)

Actions are clamped to `[-1, 1]` and mapped to physical quantities:

* Thrust scaled by `thrust_to_weight`
* Moments scaled by `moment_scale`

This abstraction significantly simplifies the control problem compared to rotor-level actuation.

---

### 4. Observations and Rewards

#### 4.1 Observations

The policy observation vector consists of:

* Root linear velocity (body frame)
* Root angular velocity (body frame)
* Projected gravity vector (body frame)
* Relative position to the goal (body frame)

Total observation dimension: **12**

#### 4.2 Reward Structure

The reward function balances stability and goal-reaching:

* Penalization of linear velocity magnitude
* Penalization of angular velocity magnitude
* Dense reward based on distance to target position

Distance is mapped using a hyperbolic tangent to avoid gradient saturation.

---

### 5. Episode Termination and Reset Logic

Episodes terminate when:

* The drone crashes (altitude < 0.1 m)
* The drone flies too high (altitude > 2.0 m)
* Maximum episode length is reached

Reset logic includes:

* Randomized goal position sampling
* Randomized reset timing to avoid synchronization artifacts
* Full reset of root pose, velocity, and joint states

Extensive logging is implemented to track episodic reward components and final distance to goal.

---

### 6. Debug Visualization

Optional debug visualization is supported via:

* Cuboid markers for goal position
* Custom environment UI window (`QuadcopterEnvWindow`)

This aids qualitative inspection during development and debugging.

---

### 7. Execution Command

The environment is intended to be executed using the following command:

```
(env_isaaclab) usuario@pc:~/Documents/Github/SkySpec/project_drone/IsaacLab/Testing/Drone$ \
python scripts/skrl/train.py --task Template-Drone-v0
```

This command relies on correct Hydra task registration and environment configuration discovery.

---

### 8. Runtime Error Encountered

During execution, the following critical error occurs:

```
AttributeError: module 'Drone.tasks.manager_based.drone.drone_env_cfg' has no attribute 'DroneEnvCfg'
```

This error arises during Hydra-based task registration when Isaac Lab attempts to dynamically load the environment configuration class.

---

### 9. Root Cause Analysis

The core issue is a **naming and registration mismatch**:

* The configuration class is named **`QuadcopterEnvCfg`**
* Hydra expects a class named **`DroneEnvCfg`** based on the task registration entry
* As a result, `getattr(mod, attr_name)` fails at runtime

This indicates that:

* The task registry entry does not match the environment configuration class name
* Or the environment was adapted from a template without updating all registry references

The remaining OmniGraph and Fabric warnings occur during shutdown and are secondary effects, not the primary failure.

---

### 10. Why This Error Is Non-Trivial

This failure is not due to physics, control logic, or reward design, but rather to:

* Tight coupling between **Hydra task names**, **Python module paths**, and **class names**
* Lack of runtime validation before simulation startup
* Sparse error messaging that obscures the true source of the mismatch

As a result, the environment appears to initialize correctly until task loading fails late in the startup process.

---

### 11. Lessons Learned from Project Drone

* Isaac Lab task registration is highly sensitive to naming conventions
* Environment logic can be correct while remaining unusable due to configuration plumbing
* Reusing templates requires systematic renaming across configs, registries, and scripts
* Direct force-and-torque control is the most viable abstraction for drone RL in Isaac Lab

---

### 12. Project Status

**Current state**: Environment logic implemented but not executable due to configuration mismatch.

**Blocking issue**: Hydra task registration expecting `DroneEnvCfg` instead of `QuadcopterEnvCfg`.

This project serves as a strong technical reference, despite being blocked at execution time.
