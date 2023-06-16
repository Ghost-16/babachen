using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using vec3 = UnityEngine.Vector3;
using list = System.Collections.Generic.List<TestSPH>;

using static Config;

//›“” ¬≈–—»ﬁ ÃŒ∆ÕŒ ÀŒÃ¿“‹!!

public class TestSPH : MonoBehaviour
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
    public static float P_MASS = Config.P_MASS;

    // Physics variables
    public vec3 pos;
    public vec3 prev_pos;
    public vec3 visual_pos;
    public float rho;
    public float rho_near;
    public float press;
    public float press_near;
    public list neighbours;
    public vec3 vel;
    public vec3 force;
    public float velocity;
    public float temperature;

    // Spatial partitioning position in grid
    public int grid_x;
    public int grid_y;
    public int grid_z;
    public int id = 0;


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
        temperature = 3000f;
        rho = 1.0f;
        rho_near = 1.0f;
        press = 0.0f;
        press_near = 0.0f;
        neighbours = new list();
        //vel = vec3.zero;
        force = new vec3(0f, -G, 0f);
        //velocity = 0.0f;
}

    // Update is called once per frame
    public void UpdateState()
    {
        float tt = /*Time.deltaTime **/ DT;

        if (name != "Base_Particle")
        {
            temperature -= 150 * velocity * tt;
            if (temperature < 100)
            {
                Destroy(gameObject);
            }
        }

        prev_pos = pos;

        vel += force * tt;
        pos += vel * tt;

        visual_pos = pos;
        transform.position = visual_pos;


        //vel = (pos - prev_pos) / tt;

        velocity = vel.magnitude;

        if (velocity > MAX_VEL)
        {
            vel = vel.normalized * MAX_VEL;
        }

        //rho = 0.0f;
        //rho_near = 0.0f;                //«¿◊≈Ã ◊“Œ ŒÕŒ ƒ≈À¿≈“

        neighbours = new list();
        force = new vec3(0, -G, 0);

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
}
