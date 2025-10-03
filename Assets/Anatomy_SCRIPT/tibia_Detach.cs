using UnityEngine;

public class tibia_Detach : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
           transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
