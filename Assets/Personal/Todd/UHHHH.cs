using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class UHHHH : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles = new Vector3(0, Time.time*50f, 0);
    }
}
