using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using vec3 = UnityEngine.Vector3;
using list = System.Collections.Generic.List<sph>;

using static Config;

public class sph : MonoBehaviour
{
    // Import simulation variables from Config.cs
    public static int N = Config.N;
    public static float SIM_W = Config.SIM_W;
    public static float BOTTOM = Config.BOTTOM;
    public static float DAM = Config.DAM;
    public static int DAM_BREAK = Config.DAM_BREAK;
    public static float G = Config.G;
    public static float SPACING = Config.SPACING;
    public static float K = Config.K;
    public static float K_NEAR = Config.K_NEAR;
    public static float REST_DENSITY = Config.REST_DENSITY;
    public static float R = Config.R;
    public static float SIGMA = Config.SIGMA;
    public static float MAX_VEL = Config.MAX_VEL;
    public static float WALL_DAMP = Config.WALL_DAMP;
    public static float VEL_DAMP = Config.VEL_DAMP;
    public static float DT = Config.DT;
    public static float WALL_POS = Config.WALL_POS;

    // Physics variables
    public vec3 pos;
    public vec3 prev_pos;
    public vec3 visual_pos;
    public float rho = 0.0f;
    public float rho_near = 0.0f;
    public float press = 0.0f;
    public float press_near = 0.0f;
    public list neighbours = new list();
    public vec3 vel = vec3.zero;
    public vec3 force = new vec3(0f, -G, 0f);
    public float velocity = 0.0f;
    public float temperature;

    // Spatial partitioning position in grid
    public int grid_x;
    public int grid_y;


    ////Poly6 Kernel
    //m_kernelPoly6 = 315.0f/(64.0f * 3.141592f * pow(m_smoothRadius, 9));
    ////Spiky Kernel
    //m_kernelSpiky = -45.0f/(3.141592f * pow(m_smoothRadius, 6));
    ////Viscosity Kernel
    //m_kernelViscosity = 45.0f/(3.141592f * pow(m_smoothRadius, 6));
    // Start is called before the first frame update
    void Start()
    {
        // Set initial position
        pos = transform.position;
        prev_pos = pos;
        visual_pos = pos;
        temperature = 560f;
    }

    // Update is called once per frame
    public void UpdateState()
    {
        prev_pos = pos;

        vel += force * Time.deltaTime * DT;
        pos += vel * Time.deltaTime * DT;

        visual_pos = pos;
        transform.position = visual_pos;

        force = new vec3(0, -G, 0);

        vel = (pos - prev_pos) / Time.deltaTime / DT;

        velocity = vel.magnitude;

        if (velocity > MAX_VEL) {
            vel = vel.normalized * MAX_VEL;
        }

        rho = 0.0f;
        rho_near = 0.0f;

        neighbours = new list();

        if (pos.y < BOTTOM)
        {
            if (name != "Base_Particle")
            {
                Destroy(gameObject);
            }
        }
    }

    public void CalculatePressure()
    {
        press = K * (rho - REST_DENSITY);
        press_near = K_NEAR * rho_near;
    }

    //void OnCollisionStay3D(Collision3D collision)
    //{
    //    // Calculate the normal vector of the collision
    //    vec3 normal = collision.contacts[0].normal;

    //    // Calculate the velocity of the particle in the normal direction
    //    float vel_normal = Vector3.Dot(vel, normal);

    //    // If the velocity is positive, the particle is moving away from the wall
    //    if (vel_normal > 0)
    //    {
    //        return;
    //    }

    //    // Calculate the velocity of the particle in the tangent direction
    //    vec3 vel_tangent = vel - normal * vel_normal;

    //    // Calculate the new velocity of the particle
    //    vel = vel_tangent - normal * vel_normal * WALL_DAMP;

    //    // Move the particle out of the wall
    //    pos = collision.contacts[0].point + normal * WALL_POS;
    //}

    
}
