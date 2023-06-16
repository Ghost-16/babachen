using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = System.Random;
using vec3 = UnityEngine.Vector3;

public class Shower : MonoBehaviour
{
    // Get the Simulation object
    public GameObject Simulation;
    // Get the Base_Particle object from Scene
    public GameObject Base_Particle;
    private static Random Random = new Random();
    //float speed_x = rand_speed();
    //float speed_y = rand_speed();
    //float speed_z = rand_speed();
    //public Vector3 init_speed = new Vector3(rand_speed(), rand_speed(), rand_speed());
    public float spawn_rate;
    public float maxCount = 1000;
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        Simulation = GameObject.Find("Explosion");
        Base_Particle = GameObject.Find("Base_Particle");
        spawn_rate = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        //time += Time.deltaTime;
        //if (time < 1.0f / spawn_rate)
        //{
        //    return;
        //}
        //else
        //{
        //    Explode();
        //    time = 0.0f;
        //}
    }

    public void Explode()
    {
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
            GameObject new_particle = Instantiate(Base_Particle, transform.position + new Vector3(my_rand(-1, 1), my_rand(-1, 1), my_rand(-1, 1)), Quaternion.identity);

            // update the particle's position
            new_particle.GetComponent<sph>().pos = transform.position;
            new_particle.GetComponent<sph>().prev_pos = transform.position;
            new_particle.GetComponent<sph>().visual_pos = transform.position;
            new_particle.GetComponent<sph>().vel = new Vector3(my_rand(-10, 10), my_rand(0, 15), my_rand(-10, 10));

            // Set as child of the Simulation object
            new_particle.transform.parent = Simulation.transform;

            // Reset time
            //time = 0.0f;
        }
    }

    static float my_rand(int x, int y)
    {
        //float r = Random.Next(x, y);
        return Random.Next(x, y); ;
    }
}