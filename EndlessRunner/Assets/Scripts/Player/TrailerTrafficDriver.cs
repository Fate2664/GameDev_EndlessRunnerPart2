using System.Collections.Generic;
using UnityEngine;

// Small, intro-only driver for this project's straight, negative-Z road.
// Uses conservative swept footprints, not rays through vehicle centres.
public sealed class TrailerTrafficDriver
{
    private struct Traffic
    {
        public Bounds bounds;
        public Vector3 velocity;
    }

    private readonly PlayerController player;
    private readonly Collider[] bodyColliders;
    private readonly List<Traffic> traffic = new List<Traffic>();
    private Collider[] overlaps = new Collider[64];
    private float nextLaneChangeTime;

    public TrailerTrafficDriver(PlayerController player)
    {
        this.player = player;
        bodyColliders = player.GetComponentsInChildren<Collider>();
    }

    public void Tick(float lookAhead, float gap, float sidePadding,
        float crossingTime, float cooldown, float braking)
    {
        if (player.carRigidbody == null) return;
        lookAhead = Mathf.Max(lookAhead, crossingTime);
        Bounds body = GetBodyBounds();
        float speed = Mathf.Max(0f, -player.carRigidbody.linearVelocity.z);

        // Include fast vehicles approaching from behind as well as those ahead.
        // No fixed-size truncation: grow the query if all slots were filled.
        float range = Mathf.Max(2000f, (speed + 900f) * lookAhead + body.size.z + gap);
        Physics.SyncTransforms(); // Traffic is Transform-driven in Update.
        int count;
        do
        {
            count = Physics.OverlapBoxNonAlloc(body.center,
                new Vector3(player.laneDistance * 2f + body.size.x, body.extents.y + 10f, range),
                overlaps, Quaternion.identity, ~0, QueryTriggerInteraction.Collide);
            if (count < overlaps.Length) break;
            System.Array.Resize(ref overlaps, overlaps.Length * 2);
        } while (true);

        traffic.Clear();
        for (int i = 0; i < count; i++)
        {
            Collider obstacle = overlaps[i];
            if (obstacle.transform.IsChildOf(player.transform) ||
                obstacle.CompareTag("MovingObstacleTrigger") || obstacle.CompareTag("StaticObstacleTrigger") ||
                obstacle.CompareTag("NoSpawnTrigger")) continue;

            MovingObstacle moving = obstacle.GetComponentInParent<MovingObstacle>();
            if (moving != null && moving.CompareTag("MovingObstacleTrigger")) continue;
            // Include real traffic's trigger volumes too; these are its death hitboxes.
            if (moving == null && !obstacle.CompareTag("Obstacle")) continue;
            traffic.Add(new Traffic
            {
                bounds = obstacle.bounds,
                velocity = moving != null ? moving.WorldVelocity : Vector3.zero
            });
        }

        int lane = player.DesiredLane;
        Bounds currentPath = LaneCorridor(body, lane);
        if (!player.IsChangingLane && Time.time >= nextLaneChangeTime &&
            !IsClear(currentPath, speed, lookAhead, gap, sidePadding))
        {
            int bestLane = -1;
            float bestClearance = -1f;
            for (int candidate = lane - 1; candidate <= lane + 1; candidate += 2)
            {
                if (candidate < 0 || candidate > 2) continue;
                Bounds crossing = LaneCorridor(body, candidate);
                Bounds destination = body;
                destination.center += Vector3.right * (LaneX(candidate) - player.transform.position.x);
                if (!IsClear(crossing, speed, crossingTime, gap, sidePadding) ||
                    !IsClear(destination, speed, lookAhead, gap, sidePadding)) continue;

                float clearance = FollowingSpeed(destination, speed, gap, sidePadding, braking);
                if (clearance > bestClearance)
                {
                    bestClearance = clearance;
                    bestLane = candidate;
                }
            }

            if (bestLane >= 0 && player.TrySetLane(bestLane))
            {
                nextLaneChangeTime = Time.time + cooldown;
                currentPath = LaneCorridor(body, bestLane);
            }
        }

        // Still follow safely while a turn is settling or both neighbours are blocked.
        // Recomputed every physics step; no stale queued requests or timed cues.
        player.SetCutsceneSpeedLimit(FollowingSpeed(currentPath, speed, gap, sidePadding, braking));
    }

    private float LaneX(int lane) => (1 - lane) * player.laneDistance;

    private Bounds GetBodyBounds()
    {
        Bounds bounds = new Bounds(player.transform.position, Vector3.zero);
        bool found = false;
        foreach (Collider collider in bodyColliders)
        {
            if (collider == null || !collider.enabled || collider.isTrigger ||
                collider is WheelCollider || !collider.gameObject.activeInHierarchy) continue;
            if (!found) bounds = collider.bounds;
            else bounds.Encapsulate(collider.bounds);
            found = true;
        }
        return bounds;
    }

    private Bounds LaneCorridor(Bounds body, int lane)
    {
        Bounds destination = body;
        destination.center += Vector3.right * (LaneX(lane) - player.transform.position.x);
        body.Encapsulate(destination);
        return body;
    }

    private bool IsClear(Bounds path, float speed, float seconds, float gap, float sidePadding)
    {
        foreach (Traffic obstacle in traffic)
        {
            // Sweep the obstacle relative to the moving car. Using the complete
            // sideways corridor is deliberately conservative during a lane change.
            Bounds swept = obstacle.bounds;
            Bounds end = obstacle.bounds;
            end.center += (obstacle.velocity + Vector3.forward * speed) * seconds;
            swept.Encapsulate(end);
            swept.Expand(new Vector3(sidePadding * 2f, 0f, gap * 2f));
            if (path.Intersects(swept)) return false;
        }
        return true;
    }

    private float FollowingSpeed(Bounds path, float speed, float gap, float sidePadding, float braking)
    {
        float limit = float.PositiveInfinity;
        foreach (Traffic obstacle in traffic)
        {
            Bounds other = obstacle.bounds;
            if (other.max.x + sidePadding < path.min.x || other.min.x - sidePadding > path.max.x ||
                other.max.y < path.min.y || other.min.y > path.max.y ||
                other.center.z > path.center.z) continue;

            // Negative Z is forward. Work from bumper bounds, not root positions.
            float available = path.min.z - other.max.z - gap;
            float obstacleSpeed = Mathf.Max(0f, -obstacle.velocity.z);
            float closingSpeed = Mathf.Max(0f, speed - obstacleSpeed);
            // Reserve one physics step for a changing traffic speed/engine torque.
            available = Mathf.Max(0f, available - closingSpeed * Time.fixedDeltaTime);
            float safeSpeed = obstacleSpeed + Mathf.Sqrt(2f * Mathf.Max(1f, braking) * available);
            limit = Mathf.Min(limit, safeSpeed);
        }
        return limit;
    }
}
