using UnityEngine;

public class Bilboard : MonoBehaviour
{
    [SerializeField] private Camera cam;
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(cam.transform.rotation.eulerAngles);
    }
}
