﻿using UnityEngine;

public class FollowWP : MonoBehaviour {
    // Array of way points
    public GameObject[] waypoints;
    // waypoint index
    int currentWP = 0;
    // Tank speed
    public float speed = 10.0f;
    // Tank rotation speed
    public float rotSpeed = 10.0f;
    // Limit how far the tracker moves in front of the tank
    public float lookAhead = 10.0f;
    // Starting waypoint for this tank
    public int startingWaypoint = 0;
    
    // Store a tracker that the tanks will follow
    GameObject tracker;
    // Rigidbody for physics collisions
    Rigidbody rb;
    
    void Start() {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        
        // Set starting waypoint
        currentWP = startingWaypoint;
        
        // Create a cylinder for visual purposes
        tracker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        // Destroy the cylinder collider so it doesn't cause any issues with physics
        DestroyImmediate(tracker.GetComponent<Collider>());
        // Disable the trackers mesh renderer so you can't see it in the game
        tracker.GetComponent<MeshRenderer>().enabled = false;
        // Rotate and place the tracker
        tracker.transform.position = this.transform.position;
        tracker.transform.rotation = this.transform.rotation;
    }
    
    void ProcessTracker() {
        // Check the tracker doesn't get too far ahead of the tank
        if (Vector3.Distance(tracker.transform.position, this.transform.position) > lookAhead) return;
        
        // Check if the tank has reached a certain distance from the current waypoint
        if (Vector3.Distance(tracker.transform.position, waypoints[currentWP].transform.position) < 3.0f) {
            // Select next waypoint
            currentWP++;
        }
        
        // Check we haven't reached the last waypoint
        if (currentWP >= waypoints.Length) {
            // Reset if we have
            currentWP = 0;
        }
        
        // Aim the tracker at the current waypoint
        tracker.transform.LookAt(waypoints[currentWP].transform);
        // Move the tracker towards the waypoint
        tracker.transform.Translate(0.0f, 0.0f, (speed + 20.0f) * Time.deltaTime);
    }
    
    // Update is called once per frame
    void Update() {
        // Call the ProcessTracker method to move the tracker
        ProcessTracker();
        
        // Create a Quaternion to look at a Vector
        Quaternion lookAtWP = Quaternion.LookRotation(tracker.transform.position - this.transform.position);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, lookAtWP, rotSpeed * Time.deltaTime);
        
        // Move the tank using Rigidbody for physics collisions
        if (rb != null) {
            // Use velocity for realistic physics collisions
            Vector3 forwardMovement = transform.forward * speed;
            rb.linearVelocity = new Vector3(forwardMovement.x, rb.linearVelocity.y, forwardMovement.z);
        } else {
            // Fallback to transform movement if no Rigidbody
            this.transform.Translate(0.0f, 0.0f, speed * Time.deltaTime);
        }
    }
}