using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    [SerializeField] private Material fireMaterial;
    [SerializeField] private Renderer baseParticle;

    private Color pColor;
    private float intensity;
    static Dictionary<string, Color> fireColors = new Dictionary<string, Color>()
    {
        {"White", new Color(1f,1f,1f,1f)},
        {"Yellow", new Color(1f,1f,0f,1f)},
        {"Orange", new Color(1f,0.6f,0f,1f)},
        {"ROrange", new Color(1f,0.4f,0f,1f)},
        {"DeepRed", new Color(0.6f,0f,0f,1f)},
        {"Black", new Color(0f,0f,0f,1f)}
    };

    // Start is called before the first frame update
    void Start()
    {
        fireMaterial = baseParticle.GetComponent<Renderer>().material;
        fireMaterial.EnableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform child in transform)
        {
            float temp = child.GetComponent<TestSPH>().temperature;
            if (temp > 1250)
            {
                pColor = fireColors["White"];
                intensity = 10f;
            }
            else if (temp > 1150)
            {
                pColor = fireColors["Yellow"];
                intensity = 7f;
            }
            else if (temp > 900)
            {
                pColor = fireColors["Orange"];
                intensity = 5f;
            }
            else if (temp > 770)
            {
                pColor = fireColors["ROrange"];
                intensity = 3f;
            }
            else if (temp > 530)
            {
                pColor = fireColors["DeepRed"];
                intensity = 2f;
            }
            else
            {
                pColor = fireColors["Black"];
                intensity = 0f;
            }

            fireMaterial.SetColor("_Color", pColor);
            fireMaterial.SetColor("_EmissionColor", pColor * intensity);
            fireMaterial.EnableKeyword("_EMISSION");
        }
        //Debug.Log(temp);
        
    }
}
