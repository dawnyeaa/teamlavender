using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ParticleSteppedFrames : MonoBehaviour
{
    [SerializeField]
    private float frameRate = 50f;

    private float cachedRate = 0f;
    private float updateStep = 0f;
    private float nextSim = 0f;
    private ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        cachedRate = frameRate;
        updateStep = (1f / cachedRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (cachedRate != frameRate)
        {
            cachedRate = frameRate;
            updateStep = (1f / cachedRate);
        }
        if (Time.time >= nextSim)
        {
            ps.Simulate(updateStep, true, false, true);
            nextSim = Time.time + updateStep;
        }
        
    }
}