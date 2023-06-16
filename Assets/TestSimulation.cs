using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using list = System.Collections.Generic.List<TestSPH>;
using vec3 = UnityEngine.Vector3;

using static Config;

using GetSocialSdk.Capture.Scripts;

//ЭТУ ВЕРСИЮ МОЖНО ЛОМАТЬ!!

public class TestSimulation : MonoBehaviour
{
    //public TestShower Emitter;

    public list particles = new list();

    public Timer timer1;


    public GetSocialCapture capture;    //РЕКОРДЕР



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
    public static float EPS = 0.00000000001f;
    public static float AIR_DENS = 1.204f;
    public static float P_MASS = Config.P_MASS;

    // Base Particle Object
    public GameObject Base_Particle;

    // Spatial Partitioning Grid Variables
    public int grid_size_x;
    public int grid_size_y;
    public int grid_size_z;
    public list[,,] grid;
    public float x_min;
    public float x_max;
    public float y_min;
    public float y_max;
    public float z_min;
    public float z_max;

    void Start()
    {
        //Base_Particle = GameObject.Find("Base_Particle");

        grid_size_x = 60;
        grid_size_y = 60;
        grid_size_z = 60;
        x_min = -20f;
        x_max = 40f;
        y_min = -1f;
        y_max = 59f;
        z_min = -20f;
        z_max = 40f;
    // Initialize spatial partitioning grid
    grid = new list[grid_size_x, grid_size_y, grid_size_z];
        for (int i = 0; i < grid_size_x; i++)
        {
            for (int j = 0; j < grid_size_y; j++)
            {
                for (int k = 0; k < grid_size_z; k++)
                {
                    grid[i, j, k] = new list();
                }
            }
        }
    }

    // Utility variables
    private float density;
    private float density_near;
    private float dist;
    private float distance;
    private float normal_distance;
    private float relative_distance;
    private float total_pressure;
    private float velocity_difference;
    private vec3 pressure_force;
    private vec3 particule_to_neighbor;
    private vec3 pressure_vector;
    private vec3 normal_p_to_n;
    private vec3 viscosity_force;
    private float time;
    private float temper_difference;

    public void calculate_density(list particles)
    {
        /*
            Calculates density of particles
            Density is calculated by summing the relative distance of neighboring particles
            We distinguish density and near density to avoid particles to collide with each other
            which creates instability
        Args:
            particles (list[Particle]): list of particles
        */

        float h2 = R * R;   //квадрат максимального радиуса

        // For each particle
        foreach (TestSPH p in particles)
        {
                density = 0.0f;
                density_near = 0.0f;
                //p.rho = 0f;
                //p.rho_near = 0f;

                // for each particle in the 27 neighboring cells in the spatial partitioning grid
                //for (int i = p.grid_x - 1; i <= p.grid_x + 1; i++)
                //{
                //    for (int j = p.grid_y - 1; j <= p.grid_y + 1; j++)
                //    {
                //        for (int k = p.grid_z - 1; k <= p.grid_z + 1; k++)
                //        {
                //            // If the cell is in the grid
                //            if (i >= 0 && i < grid_size_x && j >= 0 && j < grid_size_y && k >= 0 && k < grid_size_z)
                //            {
                // For each particle in the cell
                foreach (TestSPH n in particles)
                {
                    // расстояние между частицами
                    dist = Vector3.Distance(p.pos, n.pos);

                    if (dist < R && n.id != p.id)
                    {
                        normal_distance = 1 - dist / R;
                        p.rho += normal_distance * normal_distance * P_MASS;
                        p.rho_near += normal_distance * normal_distance * normal_distance * P_MASS;
                        //n.rho += normal_distance * normal_distance * P_MASS;
                        //n.rho_near += normal_distance * normal_distance * normal_distance * P_MASS;

                        // Add n to p's neighbors for later use
                        p.neighbours.Add(n);
                    }
                }
                //            }
                //        }
                //    }
                //}
                p.rho += density;                               //ТУТ НАБИРАЕТСЯ ПЛОТНОСТЬ
                p.rho_near += density_near;
                p.CalculatePressure();                          //ТУТ СЧИТАЕТСЯ ДАВЛЕНИЕ
            
        }
    }

    public void create_pressure(list particles)
    {
        /*
            Calculates pressure force of particles
            Neighbors list and pressure have already been calculated by calculate_density
            We calculate the pressure force by summing the pressure force of each neighbor
            and apply it in the direction of the neighbor
        Args:
            particles (list[Particle]): list of particles
        */

        foreach (TestSPH p in particles)
        {
                pressure_force = vec3.zero;

                foreach (TestSPH n in p.neighbours)
                {
                    particule_to_neighbor = n.pos - p.pos;
                    distance = Vector3.Distance(p.pos, n.pos);

                    normal_distance = 1 - distance / R;
                    total_pressure = -P_MASS * distance * distance * (p.press + n.press) / (2f * p.rho * n.rho);
                    //if (n.rho == 0f && n.rho_near == 0f)
                    //{
                    //    total_pressure = (float)(45 / (System.Math.PI * R * R * R)) * ((p.press + n.press) / (2 * EPS)) * normal_distance * normal_distance + ((p.press_near + n.press_near) / (2 * EPS)) * normal_distance * normal_distance * normal_distance;
                    //}
                    //else { total_pressure = (float)(45 / (System.Math.PI * R * R * R)) * ((p.press + n.press) / (2 * n.rho)) * normal_distance * normal_distance + ((p.press_near + n.press_near) / (2 * n.rho_near)) * normal_distance * normal_distance * normal_distance; }
                    if (distance == 0f) { distance = 0.0000000001f; }
                    pressure_vector = total_pressure * particule_to_neighbor.normalized / distance;
                    //n.force -= pressure_vector;
                    pressure_force += pressure_vector;
                }
                p.force += pressure_force;
            
        }
    }

    
    public void calculate_temperature(list particles)
    {
        /*
            Calculates temperature of particles
            
        Args:
            particles (list[Particle]): list of particles
        */
        // For each particle
        foreach (TestSPH p in particles)
        {
            float temper = 0f;

            foreach (TestSPH n in p.neighbours)
            {
                particule_to_neighbor = n.pos - p.pos;
                distance = Vector3.Distance(p.pos, n.pos);

                temper_difference = p.temperature - n.temperature;

                normal_distance = 1 - distance / R;
                temper += 1200 * P_MASS * temper_difference * (-1/normal_distance) / 1.3f;  //1.3 это изобарная теплоемкость воздуха при давлении ~220 атмосфер
                n.temperature -= temper;
            }
            if (p.neighbours.Count < 1)
            {
                temper -= 50;
            }
            p.temperature += temper;
        }
    }

    public void calculate_viscosity(list particles)
    {
        /*
        Calculates the viscosity force of particles
        Force = (relative distance of particles)*(viscosity weight)*(velocity difference of particles)
        Velocity difference is calculated on the vector between the particles
        Args:
            particles (list[Particle]): list of particles
        */
        foreach (TestSPH p in particles)
        {
            foreach (TestSPH n in p.neighbours)
            {
                particule_to_neighbor = n.pos - p.pos;
                distance = Vector3.Distance(p.pos, n.pos);
                normal_p_to_n = particule_to_neighbor.normalized;
                relative_distance = distance / R;
                velocity_difference = Vector3.Dot(p.vel - n.vel, normal_p_to_n);
                if (velocity_difference > 0)
                {
                    viscosity_force = (1 - relative_distance) * velocity_difference * SIGMA * normal_p_to_n;
                    p.force -= P_MASS * viscosity_force * 0.5f;
                    //n.vel += P_MASS * viscosity_force * 0.5f;
                }
            }
        }
    }

    public void calculate_buoyancy(list particles)
    {
        /*
            Calculates buoyancy force of particles
            (выталкивающая сила, связана с температурой)
        Args:
            particles (list[Particle]): list of particles
        */
        foreach (TestSPH p in particles)
        {
                vec3 buoyancy_force = vec3.zero;

                buoyancy_force.y = 0.0004f * AIR_DENS * G * p.temperature / (p.rho * p.rho);
                p.force += buoyancy_force;
            
        }
    }
    public void destroy_old(list particles)
    {
            float min_y = 1000000;
            int p_id = -1;
            foreach (TestSPH p in particles)
            {
                if (p.pos.y < min_y)
                {
                    min_y = p.pos.y;
                    p_id = p.id;
                }
            }
            foreach (TestSPH p in particles)
            {
                if (p.id == p_id)
                {
                    Destroy(p.gameObject);
                }
            }
        
    }

    // Update is called once per frame
    void Update()
    {

        // Add children GameObjects to particles list
        time = Time.realtimeSinceStartup;
        particles.Clear();
        foreach (Transform child in transform)
        {
            particles.Add(child.GetComponent<TestSPH>());
        }

        // Assign particles to spatial partitioning grid
        //for (int i = 0; i < grid_size_x; i++)
        //{
        //    for (int j = 0; j < grid_size_y; j++)
        //    {
        //        for (int k = 0; k < grid_size_z; k++)
        //        {
        //            grid[i, j, k].Clear();
        //        }
        //    }
        //}
        //foreach (TestSPH p in particles)
        //{
        //    // Assign grid_x and grid_y using x_min y_min x_max y_max
        //    p.grid_x = (int)((p.pos.x - x_min) / (x_max - x_min) * grid_size_x);
        //    p.grid_y = (int)((p.pos.y - y_min) / (y_max - y_min) * grid_size_y);
        //    p.grid_z = (int)((p.pos.z - z_min) / (z_max - z_min) * grid_size_z);

        //    // Add particle to grid if it is within bounds
        //    if (p.grid_x >= 0 && p.grid_x < grid_size_x && p.grid_y >= 0 && p.grid_y < grid_size_y && p.grid_z >= 0 && p.grid_z < grid_size_z)
        //    {
        //        grid[p.grid_x, p.grid_y, p.grid_z].Add(p);
        //    }
        //}
        time = Time.realtimeSinceStartup - time;
        //Debug.Log("Time to assign particles to grid: " + time);

        time = Time.realtimeSinceStartup;
        foreach (TestSPH p in particles)
        {
            p.UpdateState();
        }

        capture.CaptureFrame();     //ЗАХВАТ КАДРА


        time = Time.realtimeSinceStartup - time;
        //Debug.Log("Time to update particles: " + time);


        calculate_temperature(particles);

        time = Time.realtimeSinceStartup;
        calculate_density(particles);
        time = Time.realtimeSinceStartup - time;
        //Debug.Log("Time to calculate density: " + time);

        time = Time.realtimeSinceStartup;
        foreach (TestSPH p in particles)
        {
            p.CalculatePressure();
        }
        time = Time.realtimeSinceStartup - time;
        //Debug.Log("Time to calculate pressure: " + time);

        time = Time.realtimeSinceStartup;
        create_pressure(particles);
        time = Time.realtimeSinceStartup - time;
        //Debug.Log("Time to create pressure: " + time);

        time = Time.realtimeSinceStartup;
        calculate_viscosity(particles);
        time = Time.realtimeSinceStartup - time;
        //Debug.Log("Time to calculate viscosity: " + time);

        calculate_buoyancy(particles);

        //time = Time.realtimeSinceStartup - timer1.now_time;
        //if (time > 30)
        //{
        //    destroy_old(particles);
        //}
    }

    //void _computePressure(void)
    //{
    //    //h^2
    //    float h2 = m_smoothRadius * m_smoothRadius;

    //    //reset neightbor table
    //    m_neighborTable.reset(m_pointBuffer.size());

    //    for (unsigned int i = 0; i < m_pointBuffer.size(); i++)
    //    {
    //        Point* pi = m_pointBuffer.get(i);

    //        float sum = 0.f;
    //        m_neighborTable.point_prepare(i);

    //        int gridCell[8];
    //        m_gridContainer.findCells(pi->pos, m_smoothRadius / m_unitScale, gridCell);

    //        for (int cell = 0; cell < 8; cell++)
    //        {
    //            if (gridCell[cell] == -1) continue;

    //            int pndx = m_gridContainer.getGridData(gridCell[cell]);
    //            while (pndx != -1)
    //            {
    //                Point* pj = m_pointBuffer.get(pndx);
    //                if (pj == pi)
    //                {
    //                    sum += pow(h2, 3.f);  //self
    //                }
    //                else
    //                {
    //                    fVector3 pi_pj = (pi->pos - pj->pos) * m_unitScale;
    //                    float r2 = pi_pj.len_sq();
    //                    if (h2 > r2)
    //                    {
    //                        float h2_r2 = h2 - r2;
    //                        sum += pow(h2_r2, 3.f);  //(h^2-r^2)^3

    //                        if (!m_neighborTable.point_add_neighbor(pndx, sqrt(r2)))
    //                        {
    //                            goto NEIGHBOR_FULL;
    //                        }
    //                    }
    //                }
    //                pndx = pj->next;
    //            }

    //        }

    //    NEIGHBOR_FULL:
    //        m_neighborTable.point_commit();

    //        //m_kernelPoly6 = 315.0f/(64.0f * 3.141592f * h^9);
    //        pi->density = m_kernelPoly6 * m_pointMass * sum;
    //        pi->pressure = (pi->density - m_restDensity) * m_gasConstantK;
    //    }
    //}

    //void _computeForce(void)
    //{
    //    float h2 = m_smoothRadius * m_smoothRadius;

    //    for (unsigned int i = 0; i < m_pointBuffer.size(); i++)
    //    {
    //        Point* pi = m_pointBuffer.get(i);

    //        fVector3 accel_sum;
    //        int neighborCounts = m_neighborTable.getNeighborCounts(i);

    //        for (int j = 0; j < neighborCounts; j++)
    //        {
    //            unsigned short neighborIndex;
    //            float r;
    //            m_neighborTable.getNeighborInfo(i, j, neighborIndex, r);

    //            Point* pj = m_pointBuffer.get(neighborIndex);
    //            //r(i)-r(j)
    //            fVector3 ri_rj = (pi->pos - pj->pos) * m_unitScale;
    //            //h-r
    //            float h_r = m_smoothRadius - r;
    //            //h^2-r^2
    //            float h2_r2 = h2 - r * r;

    //            //F_Pressure
    //            //m_kernelSpiky = -45.0f/(3.141592f * h^6);			
    //            float pterm = -m_pointMass * m_kernelSpiky * h_r * h_r * (pi->pressure + pj->pressure) / (2.f * pi->density * pj->density);
    //            accel_sum += ri_rj * pterm / r;

    //            //F_Viscosity
    //            //m_kernelViscosity = 45.0f/(3.141592f * h^6);
    //            float vterm = m_kernelViscosity * m_viscosity * h_r * m_pointMass / (pi->density * pj->density);
    //            accel_sum += (pj->velocity_eval - pi->velocity_eval) * vterm;
    //        }

    //        pi->accel = accel_sum;
    //    }
    //}
}