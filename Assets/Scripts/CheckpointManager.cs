using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public float MaxTimeToReachNextCheckpoint = 30f;
    public float TimeLeft = 30f;
    
    public KartAgent kartAgent;
    public Checkpoint nextCheckPointToReach;
    
    private int CurrentCheckpointIndex;
    private List<Checkpoint> Checkpoints;
    private Checkpoint lastCheckpoint;

    public event Action<Checkpoint> reachedCheckpoint; 

    void Start()
    {
        Checkpoints = FindObjectOfType<Checkpoints>().checkPoints;
        ResetCheckpoints();
    }

    public void ResetCheckpoints()
    {
        CurrentCheckpointIndex = 0;
        TimeLeft = MaxTimeToReachNextCheckpoint;
        
        SetNextCheckpoint();
    }

    private void Update()
    {
        TimeLeft -= Time.deltaTime;

        if (TimeLeft < 0f)
        {
            kartAgent.AddReward(-1f);
            kartAgent.EndEpisode();
        }
    }

    public void CheckPointReached(Checkpoint checkpoint)
    {
        if (nextCheckPointToReach != checkpoint) return;
        
        lastCheckpoint = Checkpoints[CurrentCheckpointIndex];
        reachedCheckpoint?.Invoke(checkpoint);
        CurrentCheckpointIndex++;

        if (CurrentCheckpointIndex >= Checkpoints.Count)
        {
            kartAgent.AddReward(0.5f);
            kartAgent.EndEpisode();
        }
        else
        {
            kartAgent.AddReward((0.5f) / Checkpoints.Count);
            SetNextCheckpoint();
        }
    }

    private void SetNextCheckpoint()
    {
        if (Checkpoints.Count > 0)
        {
            TimeLeft = MaxTimeToReachNextCheckpoint;
            nextCheckPointToReach = Checkpoints[CurrentCheckpointIndex];
            
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        Debug.unityLogger.Log("Collision with " + other.tag);
        if (other.CompareTag("Wall"))
        {
            // Apply a negative reward for hitting the wall
            kartAgent.AddReward(-0.001f);
            Debug.Log("Collision with wall! Negative reward applied.");
            //Debug.unityLogger.Log("Collision with wall! Negative reward applied.");

            // Optional: End the episode if wall collisions are critical failures
            // EndEpisode();
        }
    }

}
