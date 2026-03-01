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
    public float softening = 0.01f; // evita explosiones cuando r ? 0

    void Start()
    {
        ComputeAccelerations();

        // Si el 0 es el Sol y el 1 es el planeta
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

        // 1?? Actualizar posiciones
        foreach (var body in bodies)
        {
            body.transform.position += body.velocity * dt + 0.5f * body.acceleration * dt * dt;
        }

        // Guardamos aceleraciones anteriores
        Vector3[] oldAccelerations = new Vector3[bodies.Length];
        for (int i = 0; i < bodies.Length; i++)
            oldAccelerations[i] = bodies[i].acceleration;

        // 2?? Recalcular aceleraciones
        ComputeAccelerations();

        // 3?? Actualizar velocidades
        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i].velocity += 0.5f * (oldAccelerations[i] + bodies[i].acceleration) * dt;
        }
    }

    void ComputeAccelerations()
    {
        // Reset
        foreach (var body in bodies)
            body.acceleration = Vector3.zero;

        // Calcular fuerza por pares (i < j)
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

        // Elegimos un eje que NO sea paralelo
        Vector3 arbitraryAxis = Vector3.forward;

        if (Vector3.Dot(radiusVector.normalized, arbitraryAxis) > 0.9f)
            arbitraryAxis = Vector3.right;

        Vector3 perpendicular = Vector3.Cross(radiusVector, arbitraryAxis).normalized;

        planet.velocity = perpendicular * orbitalSpeed;
    }
}