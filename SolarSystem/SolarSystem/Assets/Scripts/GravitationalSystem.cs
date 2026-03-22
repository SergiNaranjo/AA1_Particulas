using UnityEngine;

public class StableNBodySimulation : MonoBehaviour
{
    [System.Serializable]
    public class Body
    {
        public Transform transform;
        public float mass = 1f;

        [HideInInspector] public Vector3 velocity;
        [HideInInspector] public Vector3 acceleration;
    }

    public Body[] bodies;

    [Header("Simulation Settings")]
    public float gravitationalConstant = 1f;
    public float timeStep = 0.001f;
    public float softening = 0.01f; // prevents singularities whe distance is very small

    void Start()
    {
        ComputeAccelerations();

        // Initialize circular orbits (assuming body[0] is the central body (The Sun))
        for (int i = 1; i < bodies.Length; i++)
        {
            SetupCircularOrbit(i, 0);
        }
    }

    void FixedUpdate()
    {
        Simulate();
    }

    void Simulate()
    {
        float dt = timeStep;

        // Step 1: Update positions using current velocity and acceleration 
        foreach (var body in bodies)
        {
            body.transform.position += body.velocity * dt + 0.5f * body.acceleration * dt * dt;
        }

        // Sore previous accelerations
        Vector3[] oldAccelerations = new Vector3[bodies.Length];
        for (int i = 0; i < bodies.Length; i++)
            oldAccelerations[i] = bodies[i].acceleration;

        // Step 2: Recalculate accelerations based on new positions
        ComputeAccelerations();

        // Step 3: Update velocities using average acceleration (Verlet integration)
        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i].velocity += 0.5f * (oldAccelerations[i] + bodies[i].acceleration) * dt;
        }
    }

    void ComputeAccelerations()
    {
        // Reset all accelerations
        foreach (var body in bodies)
            body.acceleration = Vector3.zero;

        // Compute pairwise gravitational interactions (i < j)
        for (int i = 0; i < bodies.Length; i++)
        {
            for (int j = i + 1; j < bodies.Length; j++)
            {
                Vector3 direction = bodies[j].transform.position - bodies[i].transform.position;
                float distanceSqr = direction.sqrMagnitude + softening * softening;
                float distance = Mathf.Sqrt(distanceSqr);

                Vector3 forceDir = direction / distance;

                float forceMagnitude = gravitationalConstant * bodies[i].mass * bodies[j].mass / distanceSqr;

                Vector3 force = forceDir * forceMagnitude;

                bodies[i].acceleration += force / bodies[i].mass;
                bodies[j].acceleration -= force / bodies[j].mass;
            }
        }
    }

    void SetupCircularOrbit(int planetIndex, int centerIndex)
    {
        Body planet = bodies[planetIndex];
        Body center = bodies[centerIndex];

        Vector3 radiusVector = planet.transform.position - center.transform.position;
        float distance = radiusVector.magnitude;

        float orbitalSpeed = Mathf.Sqrt(gravitationalConstant * center.mass / distance);

        // Choose an arbitrary axis that is not parallel to the radius vector
        Vector3 arbitraryAxis = Vector3.forward;

        if (Vector3.Dot(radiusVector.normalized, arbitraryAxis) > 0.9f)
            arbitraryAxis = Vector3.right;

        Vector3 perpendicular = Vector3.Cross(radiusVector, arbitraryAxis).normalized;

        planet.velocity = perpendicular * orbitalSpeed;
    }
}