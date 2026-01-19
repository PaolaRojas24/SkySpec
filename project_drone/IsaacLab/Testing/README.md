# Drone Reinforcement Learning Project

## Development Errors, Limitations, and Attempted Solution Approaches

### 1. Project Context and Objective

The objective of this project is to develop a reinforcement learning (RL) environment for a quadrotor drone using NVIDIA Isaac Lab, with the goal of training control policies (initially PPO via SKRL) for stable flight and basic maneuvering. The project relies on a custom drone USD asset with four propellers and aims to integrate physics-based simulation, observation pipelines, action spaces, and reward functions compatible with Isaac Lab’s manager-based workflow.

Throughout the development, multiple implementation strategies were explored. However, persistent API incompatibilities, missing dependencies, and version mismatches prevented successful training execution. This document details the errors encountered and the different solution paths attempted.

---

### 2. Initial Approach: Direct Rigid-Body Control (Attempt 1)

#### 2.1 Concept

The first approach attempted to control the drone as a rigid body by directly applying forces or velocities to the drone base. The idea was to bypass articulated joints and represent thrust as body-level actions.

#### 2.2 Intended Tools

* `BodyVelocityActionCfg`
* `BodyForceActionCfg`

These action terms were expected to exist in Isaac Lab and allow direct control of linear and angular motion.

#### 2.3 Errors Encountered

* The referenced action configuration classes were not available in the installed Isaac Lab version.
* Import errors occurred due to missing modules or outdated documentation references.
* The available API did not expose body-level control terms compatible with the manager-based environment.

#### 2.4 Outcome

This approach was abandoned after confirming that body-level action terms were not supported in the installed Isaac Lab distribution.

---

### 3. Joint-Based Propeller Control Using Articulation (Attempt 2)

#### 3.1 Concept

The second approach involved modifying the drone USD to include four revolute joints (one per propeller). Each joint would represent a rotor, and control would be applied via joint velocities or efforts. This aligned better with Isaac Lab’s articulated robot paradigm.

#### 3.2 Implementation Steps

* Added four revolute joints to the USD (`Joint_Prop1` to `Joint_Prop4`).
* Switched to `JointVelocityActionCfg` for action definition.
* Attempted to model thrust indirectly through high joint angular velocities.

#### 3.3 Errors and Limitations

* Repeated API mismatches between documentation examples and installed Isaac Lab version.
* Parameter naming inconsistencies (e.g., `joint_names_expr` vs. `joint_names`).
* Missing or relocated modules in `isaaclab.envs.mdp`.
* Difficulty mapping joint velocities to physically meaningful thrust without a motor or aerodynamic model.
* Unclear coupling between joint rotation and upward force in the physics engine.

#### 3.4 Outcome

Although closer to the intended framework, this approach proved unstable and incompatible with the available APIs. The lack of a proper thrust model made learning impractical.

---

### 4. USD and Asset Configuration Issues

Across multiple attempts, significant issues arose from asset configuration:

* Confusion between `ArticulationCfg`, `RigidObjectCfg`, and legacy asset configuration classes.
* Deprecated or removed arguments such as `rigid_props`, `rigid_body_properties`, and `make_instanceable`.
* Inconsistent handling of collision, mass, and inertia parameters.
* Difficulty determining whether the drone USD should be treated as an articulation or a rigid object.

These issues caused runtime errors during environment initialization and physics setup.

---

### 5. Manager-Based Environment and MDP API Mismatches

#### 5.1 Observation and Reward Definitions

* `ObservationGroupCfg` API changed across versions (e.g., removal of `terms=` argument).
* Several example MDP functions referenced in tutorials were not present in the installed version.
* Required observation terms (e.g., base velocity, orientation) had unclear naming or import paths.

#### 5.2 ActionTermCfg Incompatibilities

* Custom action terms (e.g., `ThrustAction`) were attempted but failed due to abstract method requirements and signature mismatches.
* Changes in how actions are scaled, clipped, and applied caused further confusion.

---

### 6. Environment Execution and Dependency Errors (Attempt 3)

#### 6.1 Execution Methods Tried

* Running training via `env_isaaclab` CLI.
* Executing scripts with the system Python environment.
* Executing scripts using the Python interpreter bundled with Isaac Sim.

#### 6.2 Errors Encountered

* Persistent `ModuleNotFoundError` related to `omni` modules.
* Incompatibilities between Isaac Lab installed via pip and Isaac Sim runtime expectations.
* Conflicts between SKRL, Isaac Lab, and Isaac Sim versions.

#### 6.3 Outcome

Despite aligning versions as closely as possible, the environment could not be executed reliably. This attempt was formally abandoned to avoid repeating the same failure modes.

---

### 7. Summary of Root Causes

The main factors preventing successful progress were:

1. **Version fragmentation** between Isaac Lab, Isaac Sim, and SKRL.
2. **Outdated or inconsistent documentation** relative to the installed APIs.
3. **Lack of a native drone/thrust abstraction** in Isaac Lab comparable to wheeled or legged robots.
4. **High implementation overhead** for custom aerodynamic and motor models.
5. **Unclear best practices** for drone RL within the manager-based framework.

---

### 8. Lessons Learned and Implications

* Isaac Lab is currently optimized for articulated ground robots, not aerial vehicles.
* Drone control requires first-class support for thrust and torque modeling, which is non-trivial to retrofit.
* Version control and strict alignment between simulation tools are critical.
* A simpler baseline (e.g., point-mass drone or external simulator) may be more appropriate for early RL experiments.

---

### 9. Suggested Next Steps (High-Level)

While not implemented yet, future work could include:

* Using a simplified drone model with direct force control implemented outside the manager framework.
* Exploring alternative simulators with native drone support (e.g., AirSim, PyBullet-based drones).
* Treating the drone as a learning problem at a higher abstraction level (desired accelerations instead of rotor speeds).
* Freezing the software stack to a known working reference configuration before further development.
