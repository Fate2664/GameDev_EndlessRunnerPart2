# Trailer opening

Open `Assets/Scenes/TrailerScene.unity` after Unity recompiles. If the scene was
already open when these changes were made, reload the saved scene to see the
new objects. The existing placed road and building transforms are preserved.

## Shot controls

- `TrailerDirector`: opening hold (1 second), camera blend (6 seconds), HUD fade
  (1 second), and traffic-aware steering settings. The HUD fades only after Cinemachine finishes
  the blend. Zero-duration blends and fades are supported.
- `IntroCamera`: Cinemachine Follow offset `(0, 200, 120)`, World Space binding,
  Hard Look At the player, FOV 60. Adjust this offset to frame the wide shot.
- `GameplayCamera`: Follow offset `(0, 21, 57.4)`, World Space binding, fixed
  rotation `(7.948, 180, 0)`, FOV 60. This matches the existing Level 1 camera.
- Both virtual cameras remain at the scene root with unit scale. The old
  car-mounted close-up camera is retained but inactive. The Audio Listener is
  now on Main Camera.

`CameraController` yields to an active Cinemachine Brain. Scenes without a live
Cinemachine camera keep the original follow behaviour.

## Driving and UI

There are no timed lane cues. With `Scripted Steering` enabled, the intro scans
traffic every physics step using vehicle collider bounds and relative motion.
It stays in its lane when clear, otherwise checks adjacent lanes and the full
crossing corridor, including vehicles approaching from behind. It never jumps
across two lanes. The existing Prometeo lane-change animation is retained.

On `TrailerDirector`, the default avoidance controls are:

- `Traffic Look Ahead`: 2 seconds of predicted relative movement.
- `Traffic Gap`: 20 world units of bumper clearance.
- `Traffic Side Padding`: 3 world units around the crossing footprint.
- `Lane Change Safety Time`: 0.45 seconds (the 0.25-second manoeuvre plus margin).
- `Lane Change Cooldown`: 0.8 seconds between accepted manoeuvres.
- `Traffic Braking`: 120 world units/s squared for the following-speed calculation.

When blocked, an intro-only speed governor removes motor torque at the limit and
caps forward Rigidbody velocity to a safe following speed. It preserves vertical
and sideways velocity. This is an arcade speed cap, not simulated wheel braking;
an unexpectedly close obstacle can cause a sharp slowdown. Traffic motion comes
from `MovingObstacle.WorldVelocity`, since these vehicles move through Transforms.
Traffic trigger hitboxes count as obstacles; score/road-spawn triggers do not.

The driver assumes this scene's straight road: forward is negative world Z and
lane X positions are `+laneDistance`, `0`, `-laneDistance`. It is not a general
navigation system, and cannot guarantee recovery from overlapping spawns or an
unavoidable oncoming collision. Avoidance and its speed limit stop when the
camera reaches the gameplay view; keyboard steering resumes from the current
target lane. A manoeuvre already in progress finishes normally.

Turn off `Scripted Steering` on TrailerDirector to steer with A/D throughout
the move. Automatic acceleration, wheel animation and engine sound continue.
Death triggers and pausing are suppressed during the camera move so the
opening can finish; ordinary collision/death handling resumes at handoff.

Traffic still uses the existing random spawner. The passes therefore vary between
takes; the car will not weave unnecessarily when its current lane is clear.

Canvas Groups on `InGameUI/MainGameUI` and `InGameUI/PickupOverlay` start at
alpha 0. Their children stay active and their counters can update while hidden.
Score, distance, death-screen, pause-screen and pickup-camera references are
wired to the active trailer player.

The car's own Prometeo controller is enabled. The redundant standalone
`GameManager/PlayerManager` is retained but inactive in this scene, so a second
controller cannot initialize the same car again.

## Where endless generation starts

The checked `RoadGenerationMarker` is on:

`Roads_3Lanes / Road_3Lane (3)`

Its road root is at world Z = -233. Its existing child `Spawner` collider sits
beyond the road centre along the driving direction. Generation waits for BOTH
that `RoadSpawn` trigger and completion of the camera move. If the car crosses
it early, one spawn cycle is deferred until the camera finishes; skipped road
crossings are not replayed.

To choose another piece, uncheck `Start Endless Generation` on the current
marker, add `RoadGenerationMarker` to the desired road instance, and check it
there. Keep its child Spawner collider enabled and tagged `RoadSpawn`. Make
these scene-instance changes, not a checked override on the shared prefab.
For the current straight route, forward is negative world Z.

`GameManager/SpawnManager` has `Wait For Marked Road` enabled and the
TrailerDirector reference assigned. The gate also protects construction,
spike and boss road requests. Ordinary gameplay scenes default to immediate
generation.

## Existing layout

RoadSpawner discovers the road pieces below `Initial Road Root` using their
RoadSpawn trigger parents and filters for world X = 0. This includes the
extended main route and excludes the decorative side street at X = 589.

LandSpawner has `Use Preplaced Layout` enabled and 46 explicit roadside plots
on each side. It sorts each side by world Z and continues 142 units beyond the
last registered plot. Each side has its own cursor, so the translated and
rotated building groups keep their existing staggering. Distant background
buildings are not registered or recycled. The original 15-pair startup buffer
is used only when `Use Preplaced Layout` is disabled.

When extending the authored roadside scenery, add those plot roots to the
corresponding preplaced list. A plot's world X is positive on the left and
negative on the right. Keep enough existing scenery ahead to cover the shot.

## Play Mode checks

1. The opening is high behind the car, with the HUD invisible from the first frame.
2. The car drives and changes lanes while the camera descends; the HUD fades
   only after the blend finishes.
3. Road positions and land counts stay unchanged until the marker and camera
   conditions are both satisfied. Multiple car colliders must not cause multiple
   spawn cycles on the same crossing.
4. New land extends the ends of the two roadside strips, with no duplicate
   startup buffer. The background scenery stays in place.
5. Steering, pause/death UI, score and distance work after the intro. Also open
   Level 1 to check the original camera and immediate generation behaviour.
6. Put a slower car ahead: the intro should take a clear adjacent lane. Put a
   car alongside or fast traffic behind in that lane: it must not cut across it.
7. Block all three lanes: the player should slow behind traffic, then resume
   when a gap opens. Repeat several random starts, including near camera handoff.

`Tests/TrailerTrafficDriver.Tests.ps1` exercises the actual driver source with
lightweight Unity/physics doubles (clear road, blocked lanes, rear approaches,
cooldown and query-buffer overflow). It does not simulate WheelCollider physics.

The scripts were compiled against this project's Unity 6000.3.9f1 assemblies.
Visual composition and physics timing still need a Play Mode preview.
