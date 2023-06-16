using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = System.Random;
using vec3 = UnityEngine.Vector3;

//›“¿ ÿ“” ¿ ƒÀﬂ “≈—“Œ¬€’ ¬≈–—»…

public class TestShower : MonoBehaviour
{
    // Get the Simulation object
    public GameObject Simulation;
    // Get the Base_Particle object from Scene
    public GameObject Base_Particle;
    private static Random Random = new Random();
    //float speed_x = rand_speed();
    //float speed_y = rand_speed();
    //float speed_z = rand_speed();
    public float vel_mult = 1;
    public int[] init_speed = new int[6];
    public int dist_prob;
    public float spawn_rate;
    public float maxCount = 100000;
    private float time;

    public bool started;
    public bool finished;

    // Start is called before the first frame update
    void Start()
    {
        Simulation = GameObject.Find("Explosion");
        Base_Particle = GameObject.Find("Base_Particle");
        spawn_rate = 0.2f;
        dist_prob = 30;
        vel_mult = 1;
        started = false;
        finished = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(started && Simulation.transform.childCount == 1) {
            finished = true;
        }
    }

    public void Explode()
    {
        started = true; finished = false;
        // Limit the number of particles
        for (int i = Simulation.transform.childCount; i < maxCount; i++)
        {
            //Debug.Log("Creating particle");
            // Spawn particles at a constant rate
            //time += Time.deltaTime;
            //if (time < 1.0f / spawn_rate)
            //{
            //    return;
            //}
            // Create a new particle at the current position of the object
            GameObject new_particle = Instantiate(Base_Particle, transform.position /*+ new Vector3((float)Random.NextDouble(), (float)Random.NextDouble(), (float)Random.NextDouble())*/, Quaternion.identity);

            new_particle.GetComponent<TestSPH>().id = i;
            // update the particle's position
            new_particle.GetComponent<TestSPH>().pos = transform.position;
            new_particle.GetComponent<TestSPH>().prev_pos = transform.position;
            new_particle.GetComponent<TestSPH>().visual_pos = transform.position;
            new_particle.GetComponent<TestSPH>().vel = new Vector3(my_rand(init_speed[0], init_speed[1])* vel_mult, my_rand(init_speed[2], init_speed[3])* vel_mult, my_rand(init_speed[4], init_speed[5])* vel_mult);

            if (my_rand(0, 100) < dist_prob)
            {
                new_particle.GetComponent<TestSPH>().vel *= (my_rand(5, 10)* vel_mult);
            }

            // Set as child of the Simulation object
            new_particle.transform.parent = Simulation.transform;

            // Reset time
            // time = 0.0f;
        }
    }

    static float my_rand(int x, int y)
    {
        //float r = Random.Next(x, y);
        return Random.Next(x, y);
    }
}
