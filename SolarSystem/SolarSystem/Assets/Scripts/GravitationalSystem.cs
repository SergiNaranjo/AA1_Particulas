using UnityEngine;
using UnityEngine.Rendering;

public class GravitationalOrbit : MonoBehaviour
{
    readonly float gravConst = 39.478f;
    GameObject[] celestials;
    void Start()
    {
        celestials = GameObject.FindGameObjectsWithTag("Celestial");

        InitialVelocity();
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        Gravity();
    }

    // Update is called once per frame
    void Gravity()
    {
        foreach (GameObject a in celestials)
        {
            foreach (GameObject b in celestials)
            {
                if (!a.Equals(b))
                {
                    float m1 = a.GetComponent<Rigidbody>().mass;
                    float m2 = b.GetComponent<Rigidbody>().mass;
                    float r = Vector3.Distance(a.transform.position, b.transform.position);

                    a.GetComponent<Rigidbody>().AddForce((b.transform.position - a.transform.position).normalized *
                        (gravConst * (m1 * m2) / (r * r)));
                }
            }
        }
    }

    void InitialVelocity()
    {
        foreach (GameObject a in celestials)
        {
            foreach (GameObject b in celestials)
            {
                if (!a.Equals(b))
                {
                    float m2 = b.GetComponent<Rigidbody>().mass;
                    float r = Vector3.Distance(a.transform.position, b.transform.position);
                    a.transform.LookAt(b.transform);

                    a.GetComponent<Rigidbody>().linearVelocity += a.transform.right * Mathf.Sqrt((gravConst * m2) / r);

                }
            }
        }
    }
}
