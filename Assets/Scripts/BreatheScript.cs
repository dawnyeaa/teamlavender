using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreatheScript : MonoBehaviour
{
    Vector3 initialPos;
    Quaternion initialRot;
    Vector3 modified;
    public Vector3 breatheAmplitude;
    public Vector3 breatheFrequency;
    public Vector3 seesawAmplitude;
    public float seesawFrequency;

    void Start()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 wibbly = new Vector3(Mathf.Sin(Time.time * breatheFrequency.x),Mathf.Sin(Time.time * breatheFrequency.y),Mathf.Sin(Time.time * breatheFrequency.z));
        float wobbly = Mathf.Sin(Time.time * seesawFrequency);
        //Fanning notation
        transform.position = new Vector3(wibbly.x * breatheAmplitude.x, wibbly.y * breatheAmplitude.y, wibbly.y * breatheAmplitude.y) + initialPos;
        transform.rotation = initialRot;
        transform.Rotate(new Vector3(wobbly * seesawAmplitude.x, wobbly * seesawAmplitude.y, wobbly * seesawAmplitude.z));
    }
}