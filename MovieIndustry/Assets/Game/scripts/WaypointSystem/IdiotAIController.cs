using UnityEngine;
using Pathfinding;
using System.Collections;

public class IdiotAIController : MonoBehaviour {
	
	public AstarPath astarPath;
	public Vector3 targetPosition;
	private Seeker seeker;
    private CharacterController controller;
 	bool canMove = false;
    //The calculated path
    public Vector3[] path;
    public Path p;
    //The AI's speed per second
    public float speed = 100;
    
    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3;
 
    //The waypoint we are currently moving towards
    public int currentWaypoint = 0;
	public void Start () 
	{
		//astarPath.Scan();
        //Get a reference to the Seeker component we added earlier
        seeker = GetComponent<Seeker>();
		controller = GetComponent<CharacterController>();
		//p = new ABPath(transform.position, targetPosition, null);
       // seeker.StartPath(p);
		//path = p.vectorPath.ToArray();
        //Start a new path to the targetPosition, return the result to the OnPathComplete function
		Repath();
	}
	    
	public void Repath () 
	{
		targetPosition.x = Random.Range(-400, 400);
		targetPosition.y = Random.Range(-400, -100);
		p = new ABPath(transform.position, targetPosition, null);
		path = p.vectorPath.ToArray();
		seeker.StartPath(p, OnPathComplete);
	}
	
	public void OnPathComplete (Path p) 
	{
		canMove = true;
	}
	
	public void FixedUpdate () {
		if(!canMove)
		{
			return;
		}
		
        if (path == null) {
            //We have no path to move after yet
			Debug.Log ("path is null");
            return;
        }
        
        if ((currentWaypoint) >= p.vectorPath.Count) {
			canMove = false;
			currentWaypoint = 0;
			Repath();
            Debug.Log ("End Of Path Reached");
            return;
        }
        
        //Direction to the next waypoint
        Vector3 dir = p.vectorPath[currentWaypoint]; //(p.vectorPath[currentWaypoint]-transform.position).normalized;
       // dir *= speed * Time.fixedDeltaTime;
		//transform.position += dir;
		transform.position = Vector3.MoveTowards(transform.position, dir, speed * Time.fixedDeltaTime);
        //controller.Move (dir);
        
        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance (transform.position,p.vectorPath[currentWaypoint]) < nextWaypointDistance) {
            currentWaypoint++;
            return;
        }
    }
}
