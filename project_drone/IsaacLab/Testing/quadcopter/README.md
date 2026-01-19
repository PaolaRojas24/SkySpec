# Project: Quadcopter (Integrated Isaac Lab Drone – First Attempt)

## Objective and Rationale

The *Quadcopter* project represented the first attempt to avoid custom drone assets and instead rely on an **official, integrated aerial robot provided by Isaac Lab** (the Crazyflie configuration). The rationale behind this approach was straightforward:

* Reduce complexity related to USD authoring and articulation definitions.
* Leverage a reference robot already validated by Isaac Lab developers.
* Focus efforts on reinforcement learning logic (observations, rewards, control) rather than asset debugging.

To this end, a **manager-based RL environment** (`ManagerBasedRLEnv`) was implemented, closely following Isaac Lab examples but adapted to version **0.47.5**.

## Environment Design

The environment (`QuadcopterEnvCfg`) was structured as a manager-based RL task with the following characteristics:

* **Robot**: `CRAZYFLIE_CFG` imported from `isaaclab_assets`.
* **Physics**: 100 Hz simulation, terrain plane, replicated physics for large-scale parallel training (4096 environments).
* **Control**: Continuous 4D action space representing total thrust and body moments, applied via external forces and torques.
* **Observations**: Root linear velocity, angular velocity, projected gravity, and relative goal position.
* **Rewards**: Penalization of linear and angular velocity magnitude, and shaped reward for distance to a sampled 3D goal.
* **Execution**: Training via SKRL PPO using Hydra configuration.

To maintain compatibility with Isaac Lab 0.47.5, **all manager configuration dictionaries** (`actions`, `observations`, `rewards`, `commands`, `terminations`, `curriculum`) were intentionally defined as **empty dictionaries**, acting as placeholders. This was done to avoid import errors from newer APIs not present in the installed version.

## Execution Command

The environment was launched using the following command:

```
(env_isaaclab) usuario@pc:~/Documents/Github/SkySpec/project_drone/IsaacLab/Testing/quadcopter$ \
python scripts/skrl/train.py --task Template-Quadcopter-v0
```

## Observed Runtime Behavior

At runtime, the simulation:

* Successfully launched Isaac Sim.
* Created and replicated the scene (4096 environments).
* Loaded terrain, robot articulation, lighting, and physics correctly.
* Initialized managers for **commands, events, and recorders** without fatal errors.

However, execution consistently failed during environment initialization, *before the first RL step*, with the following critical exception:

```
AttributeError: 'ActionManager' object has no attribute '_terms'
```

## Root Cause Analysis

This error originates inside Isaac Lab’s `ActionManager` during the `load_managers()` phase. Specifically:

* `ActionManager` assumes that at least one **action term** has been registered.
* When `cfg.actions` is an empty dictionary, the internal `_terms` attribute is **never initialized**.
* Subsequent calls to `total_action_dim` attempt to iterate over `self._terms.values()`, resulting in an attribute access failure.

This behavior reveals a **hard assumption in the manager-based API**: empty manager dictionaries are *not supported*, even though they appear syntactically valid.

## Why This Was Non-Trivial to Fix

This issue is subtle and non-obvious for several reasons:

1. **No validation error** is raised at configuration parsing time.
2. The scene and simulation start successfully, giving the impression that the setup is correct.
3. The failure occurs deep inside Isaac Lab internals, far removed from user-defined code.
4. The intended workaround (defining minimal action terms) requires importing manager term classes that:

   * Either do not exist in Isaac Lab 0.47.5, or
   * Differ significantly from newer documentation and examples.

As a result, the project reached a **deadlock**: empty managers cause runtime crashes, but defining valid managers requires APIs unavailable in the installed version.

## Outcome and Lessons Learned

This attempt demonstrated that:

* Using an *official Isaac Lab quadcopter* does **not eliminate environment-level compatibility issues**.
* The manager-based framework in version 0.47.5 is **not robust to partial or placeholder configurations**.
* There is a strong coupling between Isaac Lab version, manager APIs, and example code.

Despite correct physics setup and robot instantiation, the environment could not progress to training due to **framework-level assumptions** rather than modeling or control errors.

## Key Takeaway

The failure of the Quadcopter project was **not due to drone dynamics, RL logic, or asset definition**, but rather to a mismatch between:

* The documented manager-based workflow, and
* The actual guarantees enforced internally by Isaac Lab 0.47.5.

This reinforced the broader conclusion across all drone-related experiments: **Isaac Lab version alignment is a first-order constraint**, and manager-based environments cannot be safely prototyped using incomplete configurations.
