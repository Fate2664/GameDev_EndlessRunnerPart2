// Runs the production decision code outside Unity with minimal physics doubles.
// Does not test WheelColliders, actual scene geometry, or Unity execution order.
using System;
using UnityEngine;

public static class TrafficTests
{
    private static int passed;
    private static PlayerController player;
    private static TrailerTrafficDriver driver;
    private static void Reset(int lane = 0)
    {
        Time.time = 0;
        Physics.items = new Collider[0];
        player = new PlayerController();
        player.DesiredLane = lane;
        player.transform.position = new Vector3((1 - lane) * 40, 0, 0);
        player.body.bounds = new Bounds(player.transform.position, new Vector3(16, 10, 36));
        player.carRigidbody.linearVelocity = new Vector3(0, 0, -300);
        driver = new TrailerTrafficDriver(player);
    }
    private static Collider Car(float x, float z, float speed = 100)
    {
        return new Collider { bounds = new Bounds(new Vector3(x, 0, z), new Vector3(16, 10, 36)),
            moving = new MovingObstacle { WorldVelocity = new Vector3(0, 0, -speed) } };
    }
    private static void Tick() { driver.Tick(2, 20, 3, .45f, .8f, 120); }
    private static void Check(bool condition, string name)
    {
        if (!condition) throw new Exception("FAIL: " + name);
        Console.WriteLine("PASS: " + name);
        passed++;
    }
    public static void Main()
    {
        Reset(); Tick();
        Check(player.DesiredLane == 0 && float.IsPositiveInfinity(player.limit), "Clear road: no forced weaving or speed cap");
        Reset(); Physics.items = new[] { Car(40, -250) }; Tick();
        Check(player.DesiredLane == 1, "Slower traffic ahead: take adjacent clear lane");
        Reset(); Physics.items = new[] { Car(40, -150), Car(0, 0) }; Tick();
        Check(player.DesiredLane == 0 && player.limit < 300, "Occupied neighbour: slow down, never jump two lanes");
        Reset(); Physics.items = new[] { Car(40, -150), Car(0, 150, 600) }; Tick();
        Check(player.DesiredLane == 0 && player.limit < 300, "Fast traffic behind: reject unsafe merge");
        Reset(); Physics.items = new[] { Car(40, -60) }; Tick();
        Check(player.DesiredLane == 0 && player.limit < 150, "Threat already in crossing corridor: brake instead of cutting across");
        Reset(1); Physics.items = new[] { Car(0, -150), Car(40, 0), Car(-40, 0) }; Tick();
        Check(player.DesiredLane == 1 && player.limit < 300, "All three lanes blocked: follow traffic");
        Physics.items = new Collider[0]; Tick();
        Check(float.IsPositiveInfinity(player.limit), "Gap opens: remove following-speed limit");
        Reset(); Physics.items = new[] { Car(40, -250) }; player.allowLaneChange = false; Tick();
        Check(player.DesiredLane == 0, "Controller settling lock is respected");
        player.allowLaneChange = true; Tick();
        Check(player.DesiredLane == 1, "Replan when controller is ready");
        player.transform.position = new Vector3(0, 0, 0);
        player.body.bounds = new Bounds(player.transform.position, new Vector3(16, 10, 36));
        Physics.items = new[] { Car(0, -250) }; Time.time = .1f; Tick();
        Check(player.DesiredLane == 1, "Cooldown prevents immediate second manoeuvre");
        Time.time = 1; Tick();
        Check(player.DesiredLane != 1, "Cooldown expires: avoidance resumes");
        Reset(); var score = Car(40, -60); score.tag = "MovingObstacleTrigger";
        Physics.items = new[] { score }; Tick();
        Check(player.DesiredLane == 0 && float.IsPositiveInfinity(player.limit), "Score triggers ignored");
        Reset(); Physics.items = new Collider[80];
        for (int i = 0; i < 79; i++) { Physics.items[i] = Car(40, -20); Physics.items[i].tag = "MovingObstacleTrigger"; }
        Physics.items[79] = Car(40, -250); Tick();
        Check(player.DesiredLane == 1, "Full query buffer grows without losing threats");
        Reset(); Physics.items = new[] { Car(40, -250, 400) }; Tick();
        Check(player.DesiredLane == 0 && player.limit > 300, "Traffic pulling away: no needless lane change");
        Reset(); Physics.items = new[] { Car(40, -56, 0), Car(0, 0, 0) }; Tick();
        Check(player.limit == 0, "Stopped traffic at minimum gap: stop forward motion");
        Console.WriteLine(passed + " decision tests passed.");
    }
}

public class PlayerController
{
    public Rigidbody carRigidbody = new Rigidbody();
    public Transform transform = new Transform();
    public Collider body = new Collider();
    public int DesiredLane;
    public bool IsChangingLane;
    public bool allowLaneChange = true;
    public float laneDistance = 40, limit;
    public T[] GetComponentsInChildren<T>() { return new[] { (T)(object)body }; }
    public bool TrySetLane(int lane) { if (!allowLaneChange) return false; DesiredLane = lane; return true; }
    public void SetCutsceneSpeedLimit(float value) { limit = value; }
}
public class MovingObstacle
{
    public Vector3 WorldVelocity;
    public bool CompareTag(string value) { return false; }
}
namespace UnityEngine
{
    public struct Vector3
    {
        public float x, y, z;
        public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }
        public static Vector3 zero => new Vector3();
        public static Vector3 right => new Vector3(1, 0, 0);
        public static Vector3 forward => new Vector3(0, 0, 1);
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        public static Vector3 operator *(Vector3 a, float b) => new Vector3(a.x * b, a.y * b, a.z * b);
    }
    public struct Bounds
    {
        public Vector3 center, size;
        public Bounds(Vector3 center, Vector3 size) { this.center = center; this.size = size; }
        public Vector3 extents => size * .5f;
        public Vector3 min => center - extents;
        public Vector3 max => center + extents;
        public void Encapsulate(Bounds other)
        {
            var a = new Vector3(Math.Min(min.x, other.min.x), Math.Min(min.y, other.min.y), Math.Min(min.z, other.min.z));
            var b = new Vector3(Math.Max(max.x, other.max.x), Math.Max(max.y, other.max.y), Math.Max(max.z, other.max.z));
            center = (a + b) * .5f; size = b - a;
        }
        public void Expand(Vector3 amount) { size += amount; }
        public bool Intersects(Bounds b) => min.x <= b.max.x && max.x >= b.min.x && min.y <= b.max.y && max.y >= b.min.y && min.z <= b.max.z && max.z >= b.min.z;
    }
    public class Transform { public Vector3 position; public bool IsChildOf(Transform other) => this == other; }
    public class GameObject { public bool activeInHierarchy = true; }
    public class Collider
    {
        public Transform transform = new Transform();
        public GameObject gameObject = new GameObject();
        public Bounds bounds;
        public string tag = "Obstacle";
        public bool enabled = true, isTrigger;
        public MovingObstacle moving;
        public bool CompareTag(string value) => tag == value;
        public T GetComponentInParent<T>() where T : class => moving as T;
    }
    public class WheelCollider : Collider { }
    public class Rigidbody { public Vector3 linearVelocity; }
    public struct Quaternion { public static Quaternion identity => new Quaternion(); }
    public enum QueryTriggerInteraction { Collide }
    public static class Time { public static float time; public static float fixedDeltaTime = .02f; }
    public static class Mathf
    {
        public static float Max(float a, float b) => Math.Max(a, b);
        public static float Min(float a, float b) => Math.Min(a, b);
        public static float Sqrt(float a) => (float)Math.Sqrt(a);
    }
    public static class Physics
    {
        public static Collider[] items;
        public static void SyncTransforms() { }
        public static int OverlapBoxNonAlloc(Vector3 center, Vector3 extents, Collider[] buffer, Quaternion rotation, int mask, QueryTriggerInteraction triggers)
        {
            int count = 0; var query = new Bounds(center, extents * 2);
            foreach (var item in items)
                if (query.Intersects(item.bounds) && count < buffer.Length) buffer[count++] = item;
            return count;
        }
    }
}
