using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkateSoundController : MonoBehaviour
{

    public AudioSource boardOneshot;
    public AudioSource boardLoopSourceA;
    public AudioSource boardLoopSourceB;

    public AudioClip[] boardPopSounds;
    public AudioClip[] boardLandingSounds;
    public AudioClip[] boardRollingLoops;

    public AnimationCurve pushPitchCurve;
    public float speedPitchModifier = 0.5f;
    public float speedPitchBoost = 0.5f;
    public float speedVolumeModifier = 0.5f;
    public float minRollingVolume = 0.25f;

    public int surface = 0; // number corresponds to audio clip array for surfaces
    int cachedSurface = 0;
    float surfaceTransitionTime = 1f;
    public float surfaceTransitionCrossfadeDuration = 0.2f;

    bool rolling = true;
    public float pushDuration = 0.1f;
    public float pushPitchScale = 0.5f;
    float pushTime = 0f;
    
    float pushPitchModifier = 0f;
    float airTime = 0f;
    bool audioSourceA = true; //use this to ping-pong between audio sources loading up the appropriate loop in the other source

    [SerializeField]
    float rollingSpeed = 1f;
    void Start()
    {
        pushTime = pushDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (pushTime < pushDuration)
        {
            pushTime += Time.deltaTime;
            pushPitchModifier = pushPitchCurve.Evaluate(Mathf.Clamp01(pushTime/pushDuration));
        }
        ProcessRoll();
    }

    public void Airborne()
    {
        rolling = false;
    }

    public void PopSound()
    {
        boardOneshot.clip = boardPopSounds[Random.Range(0, boardPopSounds.Length)];
        boardOneshot.Play();
        Airborne();
    }
    public void LandingSound()
    {
        boardLoopSourceA.volume = 1f;
        boardLoopSourceB.volume = 1f;
        boardOneshot.clip = boardLandingSounds[Random.Range(0, boardLandingSounds.Length)];
        boardOneshot.Play();
        rolling = true;
    }
    public void PushSound()
    {
        pushTime = 0f;
    }
    public void SetRollingSpeed(float speed)
    {
        rollingSpeed = speed;
    }
    void ProcessRoll()
    {
        if (rolling)
        {
            boardLoopSourceA.pitch = speedPitchBoost + rollingSpeed * speedPitchModifier + pushPitchModifier * pushPitchScale;
            boardLoopSourceB.pitch = speedPitchBoost + rollingSpeed * speedPitchModifier + pushPitchModifier * pushPitchScale;
            boardLoopSourceA.volume = Mathf.Max(rollingSpeed * speedVolumeModifier, minRollingVolume);
            boardLoopSourceB.volume = Mathf.Max(rollingSpeed * speedVolumeModifier, minRollingVolume);

            airTime = 0f;
        }
        if (!rolling)
        {
            airTime += Time.deltaTime;
            boardLoopSourceA.volume = Mathf.Clamp((((1f - airTime) * 0.15f) - 0.1f),0f,1f); //im magic numbering this and i dont care the way this sounds is right
            boardLoopSourceB.volume = Mathf.Clamp((((1f - airTime) * 0.15f) - 0.1f), 0f, 1f); //fuck it do em both
        }
        if (surface > boardRollingLoops.Length)
        {
            surface = boardRollingLoops.Length;
        }
        if (surface != cachedSurface)
        {
            //I know there's a better way to do this but I'm not literate enough to know how. Dont fucking "at" me
            if (audioSourceA)
            {
                boardLoopSourceB.clip = boardRollingLoops[surface]; //we are rolling source A so chamber source B
                //boardLoopSourceA.volume = 0f;
                //boardLoopSourceB.volume = 1f;
                boardLoopSourceB.Play();
            }
            else
            {
                boardLoopSourceA.clip = boardRollingLoops[surface]; //vice versa. Make it neater if you dare
                //boardLoopSourceB.volume = 0f;
                //boardLoopSourceA.volume = 1f;
                boardLoopSourceA.Play();

            }
            cachedSurface = surface;
            audioSourceA = !audioSourceA;
            surfaceTransitionTime = surfaceTransitionCrossfadeDuration;
        }

        if (rolling && surfaceTransitionTime > 0)
        {
            if (audioSourceA)
            {
                boardLoopSourceB.volume *= Mathf.InverseLerp(0, surfaceTransitionCrossfadeDuration, surfaceTransitionTime);
                boardLoopSourceA.volume *= Mathf.InverseLerp(surfaceTransitionCrossfadeDuration, 0, surfaceTransitionTime);
            }
            else
            {
                boardLoopSourceA.volume *= Mathf.InverseLerp(0, surfaceTransitionCrossfadeDuration, surfaceTransitionTime);
                boardLoopSourceB.volume *= Mathf.InverseLerp(surfaceTransitionCrossfadeDuration, 0, surfaceTransitionTime);
            }
            
            surfaceTransitionTime -= Time.deltaTime;
        }
    }
}
