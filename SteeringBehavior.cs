﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the place to put all of the various steering behavior methods we're going
/// to be using. Probably best to put them all here, not in NPCController.
/// </summary>

public class SteeringBehavior : MonoBehaviour {

    // The agent at hand here, and whatever target it is dealing with
    public NPCController agent;
    public NPCController target;

    public GameObject player;
    public NPCController house;

    // Below are a bunch of variable declarations that will be used for the next few
    // assignments. Only a few of them are needed for the first assignment.

    // For pursue and evade functions
    public float maxPrediction;
    public float maxAcceleration;

    // For arrive function
    public float maxSpeed;
    public float targetRadiusL;
    public float slowRadiusL;
    public float timeToTarget;

    // For Face function
    public float maxRotation;
    public float maxAngularAcceleration;
    public float targetRadiusA;
    public float slowRadiusA;

    // For wander function
    public float wanderOffset;
    public float wanderRadius;
    public float wanderRate;
    private float wanderOrientation;

    //Haoran
    //hw2 
    private RaycastHit hitInfo;
    public float whiskerLength;
    public float sideWhiskerScale;
    public float whiskerAngle;
    public bool avoidTree;
    public  Vector3 treePosition;
    public float treeEscapeRadius;

    public int countHitWall = 0;


    // Holds the path to follow
    public GameObject[] Path;
    public int current = 0;

    protected void Start() {
        agent = GetComponent<NPCController>();
        wanderOrientation = agent.orientation;
    }

    //Haoran
    //arrive at house?
    public bool ArriveHouse(){
        if ((agent.position - house.position).magnitude < 1.5f){
            return true;
        }else{
            return false;
        }
    }
    //Haoran
    //calculate distance to house
    public float distanceToHouse(){
        return (agent.position - house.position).magnitude;
    }
    //Haoran
    //pathfollow
    public (Vector3,float) PathFollow()
    {
        Vector3 targetOnPath = computeTargetOnThePath();
        //Debug.Log(targetOnPath);
        agent.DrawCircle(targetOnPath, 0.3f);
        Vector3 linearAcc = Seek(targetOnPath);
        float angularAcc = FaceTo(targetOnPath - agent.position);
        return (linearAcc, angularAcc);
    }

    //Haoran
    //pathfollow helper function: find the target position on path
    private Vector3 computeTargetOnThePath()
    {
        //compute line vectors,  n nodes => n-1 vectors
        List<Vector3> lineVectors = new List<Vector3>();
        for (int i = 1; i < Path.Length; i++)
        {
            lineVectors.Add(Path[i].transform.position - Path[i - 1].transform.position);
            //Debug.Log(i + "$" + lineVectors[i - 1]);
        }
        //Debug.Log(agent.position);
        //compute projected points, n nodes => n-1 vectors => n-1 prjected points 
        List<Vector3> projectedPoints = new List<Vector3>(); 
        for (int i = 0; i < lineVectors.Count; i++)
        {
            //Debug.Log(i+"?"+ (Path[i].transform.position + Vector3.Project(agent.position - Path[i].transform.position, lineVectors[i])));
            //projectedPoints.Add(Path[i].transform.position + Vector3.Project(agent.position- Path[i].transform.position, lineVectors[i] ));
            //agent.DrawCircle(Path[i].transform.position + Vector3.Project(agent.position - Path[i].transform.position, lineVectors[i]),0.5f);
            projectedPoints.Add(FindNearestPointOnLine(Path[i].transform.position, Path[i + 1].transform.position, agent.position));
            //Debug.Log(i + "?" + FindNearestPointOnLine(Path[i].transform.position, Path[i + 1].transform.position, agent.position));



        }

        //find nearest projection point
        int index = 0;
        Vector3 nearestPoint = new Vector3(0,0,0);
        float distance = Mathf.Infinity;

        List<float> distList = new List<float>();


        for (int i = 0; i <projectedPoints.Count;i++)
        {
            if ((agent.position - projectedPoints[i]).magnitude < distance)
            {
                distance = (agent.position - projectedPoints[i]).magnitude;
                index = i;
                nearestPoint = projectedPoints[i];
            }
        }

        //Debug.Log("Nearest Index = " + index + ".");
        //agent.DrawCircle(nearestPoint, 0.3f);

        //calculate the target to seek
        //index 0 ~ n-1
        float lookAheadDistance = 5;
        Vector3 targetPoint = new Vector3(0,0,0);

        if (lookAheadDistance< (Path[index + 1].transform.position - projectedPoints[index]).magnitude)
        {
            targetPoint = projectedPoints[index] + lookAheadDistance * (lineVectors[index].normalized);
        }
        else
        {
            lookAheadDistance -= (Path[index + 1].transform.position - projectedPoints[index]).magnitude;
            for (int k = index+1; k < lineVectors.Count; k++)
            {
                if (lineVectors[k].magnitude < lookAheadDistance)
                {
                    lookAheadDistance -= lineVectors[k].magnitude;
                }
                else
                {
                    targetPoint = projectedPoints[k] + lookAheadDistance * (lineVectors[k].normalized);
                    //check last segment
                    if (k == lineVectors.Count-1 && lookAheadDistance > lineVectors[k].magnitude)
                    {
                        targetPoint = Path[k+1].transform.position;
                    }
                    break;
                }
            }

        }

        return targetPoint;

    }

    //Haoran
    // find nearest position on line segment
    private Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 end, Vector3 point)
    {
        //Get heading
        Vector3 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector3 lhs = point - origin;
        float dotP = Vector3.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return origin + heading * dotP;
    }

    //Haoran 
    //dynamic pursue
    //return linear acc
    public Vector3 Pursue()
    {
        float distance = (target.position - agent.position).magnitude;

        // speed scalar
        float speed = agent.velocity.magnitude;

        // if speed small-> use bigger predictionTime
        float predictTime = (speed <= distance / maxPrediction ? maxPrediction : distance / speed);

        //draw prediction circle
        agent.DrawCircle(target.position + target.velocity * predictTime, 0.3f);

        //direction to prediction point
        Vector3 linear_acc = (target.position + target.velocity * predictTime) - agent.position;

        //clip to map linear acc
        linear_acc.Normalize();
        linear_acc *= maxAcceleration;

        return linear_acc;
    }


    //Haoran
    //evade from target
    //return linear acc
    public Vector3 Evade()
    {
        float distance = (target.position - agent.position).magnitude;

        // speed scalar
        float speed = agent.velocity.magnitude;

        // if speed small-> use bigger predictionTime
        float prediction = (speed <= distance / maxPrediction ? maxPrediction : distance / speed);

        //draw prediction circle
        agent.DrawCircle(target.position + target.velocity * prediction, 0.5f);

        //direction to evade prediction point
        Vector3 linear_acc = agent.position - (target.position + target.velocity * prediction);

        //clip to map linear acc
        linear_acc.Normalize();
        linear_acc *= maxAcceleration;

        return linear_acc;
    }

    //Haoran
    //evade position p
    //return linear acc
    public Vector3 Evade(Vector3 p)
    {
        float distance = (p - agent.position).magnitude;

        // speed scalar
        float speed = agent.velocity.magnitude;

        // if speed small-> use bigger predictionTime
        float prediction = (speed <= distance / maxPrediction ? maxPrediction : distance / speed);

        //draw prediction circle
        agent.DrawCircle(p + target.velocity * prediction, 0.5f);

        //direction to evade prediction point
        Vector3 linear_acc = p - (p + target.velocity * prediction);

        //clip to map linear acc
        linear_acc.Normalize();
        linear_acc *= maxAcceleration;

        return linear_acc;
    }

    //Haoran
    //dynamic arrive given direction
    //return linear acc
    public Vector3 ChasePlayer()
    {

        Vector3 direction = this.player.transform.position - agent.position;

        if (direction.magnitude <= slowRadiusL)
        {
            agent.label.text = "chaseplayer\n<In slowRadiusL>";
            //agent.DestroyPoints();
            agent.DrawCircle(this.player.transform.position, slowRadiusL);
        }
        else
        {
            agent.DestroyPoints();
        }

        // stop if arrive (in target radius) and return zero linear_acc
        if (direction.magnitude < targetRadiusA+2f)
        {
            agent.label.text = "Arrived!\n<In targetRadiusA>";
            agent.velocity = Vector3.zero;
            return Vector3.zero;
        }

        //calculate appropriate speed
        float speed = (direction.magnitude > slowRadiusL ? maxSpeed : maxSpeed * direction.magnitude / slowRadiusL);

        //apply direction
        direction.Normalize();
        Vector3 velocity = direction * speed;

        // calculate linear_acc
        Vector3 linear_acc = (velocity - agent.velocity) / timeToTarget;

        // clip linear_acc
        if (linear_acc.magnitude > maxAcceleration)
        {
            linear_acc.Normalize();
            linear_acc *= maxAcceleration;
        }

        return linear_acc;
    }


    //Haoran
    //dynamic arrive given target
    //return linear acc
    public Vector3 Arrive()
    {
        Vector3 direction = target.position - agent.position;
        return Arrive(direction);
    }

    //Haoran
    //dynamic arrive given direction
    //return linear acc
    public Vector3 Arrive(Vector3 direction)
    {
       // Vector3 direction = target.position - agent.position;

        if (direction.magnitude <= slowRadiusL)
        {
            agent.label.text = "dynamic arrive\n<In slowRadiusL>";
            //agent.DestroyPoints();
            agent.DrawCircle(target.position, slowRadiusL);
        }
        else
        {
            agent.DestroyPoints();
        }

        // stop if arrive (in target radius) and return zero linear_acc
        if (direction.magnitude < targetRadiusA+1.2f)
        {
            agent.label.text = "dynamic arrive\n<In targetRadiusA>";
            agent.velocity = Vector3.zero;
            return Vector3.zero;
        }

        //calculate appropriate speed
        float speed = (direction.magnitude > slowRadiusL ? maxSpeed : maxSpeed * direction.magnitude / slowRadiusL);

        //apply direction
        direction.Normalize();
        Vector3 velocity = direction * speed;

        // calculate linear_acc
        Vector3 linear_acc = (velocity - agent.velocity) / timeToTarget;

        // clip linear_acc
        if (linear_acc.magnitude > maxAcceleration)
        {
            linear_acc.Normalize();
            linear_acc *= maxAcceleration;
        }

        return linear_acc;
    }

    //Haoran
    //return angular acc
    public float FaceAway()
    {
        Vector3 direction = agent.position - target.position; //only diff with face
        return FaceAway(direction);
    }

    //Haoran
    // Calculate the angular acceleration required to rotate to target
    //return angular acc
    public float FaceAway(Vector3 direction)
    {
        //// Check for a zero direction, and make no change if so
        if (direction.magnitude <= 0.001f)
        {
            return 0;
        }

        // Get anount of angle need to rotate
        float rotationAmount = Mathf.Atan2(direction.x, direction.z) - agent.orientation;
        //agent.orientaion range [-inf,inf]

        // clip to (-pi, pi) interval
        while (rotationAmount > Mathf.PI)
        {
            rotationAmount -= 2 * Mathf.PI;
        }
        while (rotationAmount < -Mathf.PI)
        {
            rotationAmount += 2 * Mathf.PI;
        }

        // if already facing target, set angular speed to zero
        if (Mathf.Abs(rotationAmount) < targetRadiusA)
        {
            agent.rotation = 0;
        }

        // greater than slowRadius => clip to max rotation speed
        // less than slowRadius => clip to scaled rotation speed 
        float rotationSpeed = (rotationAmount > slowRadiusA ? maxRotation : maxRotation * Mathf.Abs(rotationAmount) / slowRadiusA);

        // get the correct rotation direction
        rotationSpeed *= rotationAmount / Mathf.Abs(rotationAmount);

        // calculate the rotation acceleration
        float angular_acc = rotationSpeed - agent.rotation;
        angular_acc /= timeToTarget;

        // clip to max angular acc if needed
        if (Mathf.Abs(angular_acc) > maxAngularAcceleration)
        {
            angular_acc /= Mathf.Abs(angular_acc);
            angular_acc *= maxAngularAcceleration;
        }

        return angular_acc;
    }

    //Haoran
    //seek target
    //return linear acc
    public Vector3 Seek()
    {
        return Seek(target.position);
    }
    public Vector3 Seek(Vector3 targetPosition) {
        Vector3 linear_acc = targetPosition - agent.position; //seek direction vector

        //clip to max linear acceleration
        if (linear_acc.magnitude > this.maxAcceleration){
            linear_acc = linear_acc.normalized * maxAcceleration;
        }

        //clip to max speed is handled in the UpdateMovement in NPCController.cs 
        //angular acceleration will be handled by face()  

        return linear_acc;  //returns the linear acc 
    }

    //Haoran
    //flee from target
    //return linear acc
    public Vector3 Flee()
    {
        return Flee(target.position) ;
    }

    //Haoran
    //flee from given position
    //return linear acc
    public Vector3 Flee(Vector3 targetPosition)
    {
        agent.DrawCircle(targetPosition+ new Vector3(0f,1f,0f), treeEscapeRadius);
        Vector3 linear_acc = agent.position - targetPosition;

        //clip to max linear acceleration
        if (linear_acc.magnitude > this.maxAcceleration)
        {
            linear_acc = linear_acc.normalized * maxAcceleration;
        }

        //clip to max speed is handled in the UpdateMovement in NPCController.cs 
        //angular acceleration will be handled by face()

        return linear_acc;
    }

    //Haoran
    // Calculate the angular acceleration required to rotate to target
    //return angular acc
    public float Face()
    {
        Vector3 direction = target.position - agent.position;
        return FaceTo(direction);
    }

    //Haoran
    //return angular acc required to face the player
    public float FaceToPlayer()
    {
        Vector3 direction = player.transform.position - agent.position;
        // Check for a zero direction, and make no change if so
        if (direction.magnitude == 0)
        {
            return 0;
        }

        // Get anount of angle need to rotate
        float rotationAmount = Mathf.Atan2(direction.x, direction.z) - agent.orientation;
        //agent.orientaion range [-inf,inf]

        // clip to (-pi, pi) interval
        while (rotationAmount > Mathf.PI)
        {
            rotationAmount -= 2 * Mathf.PI;
        }
        while (rotationAmount < -Mathf.PI)
        {
            rotationAmount += 2 * Mathf.PI;
        }

        // if already facing target, set angular speed to zero
        if (Mathf.Abs(rotationAmount) < targetRadiusA)
        {
            agent.rotation = 0;
        }

        // greater than slowRadius => clip to max rotation speed
        // less than slowRadius => clip to scaled rotation speed 
        float rotationSpeed = (rotationAmount > slowRadiusA ? maxRotation : maxRotation * Mathf.Abs(rotationAmount) / slowRadiusA);

        // get the correct rotation direction
        rotationSpeed *= rotationAmount / Mathf.Abs(rotationAmount);

        // calculate the rotation acceleration
        float angular_acc = rotationSpeed - agent.rotation;
        angular_acc /= timeToTarget;

        // clip to max angular acc if needed
        if (Mathf.Abs(angular_acc) > maxAngularAcceleration)
        {
            angular_acc /= Mathf.Abs(angular_acc);
            angular_acc *= maxAngularAcceleration;
        }

        return angular_acc;
    }

    //Haoran
    //face to given direction
    //return angular acc
    public float FaceTo(Vector3 direction) { 
        // Check for a zero direction, and make no change if so
        if (direction.magnitude == 0)
        {
            return 0;
        }

        // Get anount of angle need to rotate
        float rotationAmount = Mathf.Atan2(direction.x, direction.z) - agent.orientation;
        //agent.orientaion range [-inf,inf]

        // clip to (-pi, pi) interval
        while (rotationAmount > Mathf.PI)
        {
            rotationAmount -= 2 * Mathf.PI;
        }
        while (rotationAmount < -Mathf.PI)
        {
            rotationAmount += 2 * Mathf.PI;
        }

        // if already facing target, set angular speed to zero
        if (Mathf.Abs(rotationAmount) < targetRadiusA)
        {
            agent.rotation = 0;
        }

        // greater than slowRadius => clip to max rotation speed
        // less than slowRadius => clip to scaled rotation speed 
        float rotationSpeed = (rotationAmount > slowRadiusA ? maxRotation : maxRotation * Mathf.Abs(rotationAmount) / slowRadiusA);

        // get the correct rotation direction
        rotationSpeed *= rotationAmount / Mathf.Abs(rotationAmount);

        // calculate the rotation acceleration
        float angular_acc = rotationSpeed - agent.rotation;
        angular_acc /= timeToTarget;

        // clip to max angular acc if needed
        if (Mathf.Abs(angular_acc) > maxAngularAcceleration)
        {
            angular_acc /= Mathf.Abs(angular_acc);
            angular_acc *= maxAngularAcceleration;
        }

        return angular_acc;
    }

    //Haoran
    //Draw whiskers based on facing direction; Cast rays and check if ray hit any obstacles
    //return true and assign value to hitInfo if one of whiskers hit obstacle
    public bool PerformWhisker(out RaycastHit hitInfo)
    {
        if (agent.velocity.magnitude < 3f)
        {
            hitInfo = new RaycastHit();
            return false;
        }
        Vector3 faceDir = new Vector3(Mathf.Sin(agent.orientation), 0, Mathf.Cos(agent.orientation));
        faceDir.Normalize();
        Vector3 leftDir = new Vector3(Mathf.Sin(agent.orientation - whiskerAngle), 0, Mathf.Cos(agent.orientation - whiskerAngle));
        leftDir.Normalize();
        Vector3 rightDir = new Vector3(Mathf.Sin(agent.orientation + whiskerAngle), 0, Mathf.Cos(agent.orientation + whiskerAngle));
        rightDir.Normalize();

        Vector3 raySource = agent.position;
        raySource = new Vector3(raySource.x, 0.1f, raySource.z);
        //agent.DrawWhiskers(agent.position + leftDir * wanderOffset, agent.position + rightDir * wanderOffset, raySource);
        //Debug.Log("orientation:"+agent.orientation);
        
        RaycastHit hitL;
        if (Physics.Raycast(raySource, leftDir, out hitL, whiskerLength* sideWhiskerScale))
        {
            //Debug.Log(hitL.collider.name);
            agent.Draw3Whiskers(agent.position + leftDir * whiskerLength * sideWhiskerScale, agent.position + faceDir * whiskerLength, agent.position + rightDir * whiskerLength * sideWhiskerScale, agent.position);

            //agent.label.text = "<==: " + hitL.collider.name;
            hitInfo = hitL;
            return true;
        }
        RaycastHit hitR;
        if (Physics.Raycast(raySource, rightDir, out hitR, whiskerLength* sideWhiskerScale))
        {
            //Debug.Log(hitR.collider.name);
            agent.Draw3Whiskers(agent.position + leftDir * whiskerLength * sideWhiskerScale, agent.position + faceDir * whiskerLength, agent.position + rightDir * whiskerLength * sideWhiskerScale, agent.position);

            //agent.label.text = hitR.collider.name + "==>";
            hitInfo = hitR;
            return true;
        }
        RaycastHit hitM;
        if (Physics.Raycast(raySource, leftDir, out hitM, whiskerLength))
        {
            //Debug.Log(hitL.collider.name);
            agent.Draw3Whiskers(agent.position + leftDir * wanderOffset * sideWhiskerScale, agent.position + faceDir * wanderOffset, agent.position + rightDir * wanderOffset * sideWhiskerScale, agent.position);

            //agent.label.text = "<" + hitM.collider.name + ">";
            
            hitInfo = hitM;
            return true;
        }
        hitInfo = new RaycastHit();
        return false;
    }

    //cast 3 rays based on agent's velocity direction
    //return true if any ray hits obstacle
    public bool PerformWhiskerAlongVelocity(out RaycastHit hitInfo)
    {
        if (agent.velocity.magnitude < 0.3f)
        {
            hitInfo = new RaycastHit();
            return false;
        }
        Vector3 VelocityDir = agent.velocity;
        
        VelocityDir = new Vector3(VelocityDir.x, 0, VelocityDir.z);
        VelocityDir.Normalize();
        Vector3 leftDir = Quaternion.AngleAxis(-20f, Vector3.up) * VelocityDir;
        leftDir.Normalize();
        Vector3 rightDir = Quaternion.AngleAxis(20f, Vector3.up) * VelocityDir;
        rightDir.Normalize();
        //Debug.Log("magnitude: " + agent.velocity.magnitude + " velocity: " + VelocityDir);
        Vector3 raySource = agent.position;
        raySource = new Vector3(raySource.x, 0, raySource.z);
        //Debug.Log("orientation:"+agent.orientation);

        RaycastHit hitL;
        if (Physics.Raycast(raySource, leftDir, out hitL, whiskerLength * sideWhiskerScale))
        {
            // Debug.Log(hitL.collider.name);
            agent.Draw3Whiskers(agent.position + leftDir * whiskerLength * sideWhiskerScale, agent.position + VelocityDir * whiskerLength, agent.position + rightDir * whiskerLength * sideWhiskerScale, agent.position);
            //agent.label.text = "<==: " + hitL.collider.name;
            hitInfo = hitL;
            return true;
        }
        RaycastHit hitR;
        if (Physics.Raycast(raySource, rightDir, out hitR, whiskerLength * sideWhiskerScale))
        {
            //Debug.Log(hitR.collider.name);
            agent.Draw3Whiskers(agent.position + leftDir * whiskerLength * sideWhiskerScale, agent.position + VelocityDir * whiskerLength, agent.position + rightDir * whiskerLength * sideWhiskerScale, agent.position);
            //agent.label.text = hitR.collider.name + "==>";
            hitInfo = hitR;
            return true;
        }
        RaycastHit hitM;
        if (Physics.Raycast(raySource, leftDir, out hitM, whiskerLength))
        {
            // Debug.Log(hitM.collider.name);
         
            agent.Draw3Whiskers(agent.position + leftDir * wanderOffset * sideWhiskerScale, agent.position + VelocityDir * wanderOffset, agent.position + rightDir * wanderOffset * sideWhiskerScale, agent.position);

            //agent.label.text = "<" + hitM.collider.name + ">";

            hitInfo = hitM;
            return true;
        }
        hitInfo = new RaycastHit();
        return false;
    }

    //Haoran
    //this function is supposed to be used after whiskers hitting the wall
    //given hit point info, Calculate a new target position on the normal vector of the hit surface starting from hit point, then seek this new target.
    //return linear_acc and angular_acc
    public (Vector3,float) SeekAndFaceToNewTarget(RaycastHit info)
    {
        //hit wall
        
        //calculate position of avoidence target
        Vector3 newTarget = info.point + info.normal * wanderOffset* 3; //calculate normal 
        //agent.Draw1Whiskers(newTarget,agent.position);
        Vector3 linear_acc = newTarget - agent.position; //seek direction vector
             
        //clip to max linear acceleration
        if (linear_acc.magnitude > this.maxAcceleration)
        {
            linear_acc = linear_acc.normalized * maxAcceleration;
        }
        //Haoran
        //Vector3 intentionAcc = (info.point - agent.position);
        ////if (intentionAcc.magnitude < this.maxAcceleration)
        ////{
        //    intentionAcc = intentionAcc.normalized * maxAcceleration;
        ////}
        //linear_acc += intentionAcc;
        //if (linear_acc.magnitude > this.maxAcceleration)
        //{
        //    linear_acc = linear_acc.normalized * maxAcceleration;
        //}
        //

        countHitWall += 1;
        if (countHitWall >= 100)
        {
            linear_acc = (new Vector3(Mathf.Sin(agent.orientation), 0, Mathf.Cos(agent.orientation)).normalized * maxAcceleration);
        }
        else if (countHitWall >= 200)
        {
            countHitWall = 0;
        }


        float angular_acc = FaceTo(newTarget - agent.position);
        return (linear_acc, angular_acc);
    }

    //Haoran 
    // wander returns the angular_acc(account for face direction) 
    //return angular acc
    public float Wander()
    //    public float Wander(out Vector3 linear)
    {

        // adjust the initial wanderOrientation with a small random angle
        wanderOrientation += (Random.value - Random.value) * wanderRate;

        // Calculate the combined target orientation
        float orientation = wanderOrientation + agent.orientation;


        // the wander circle center position
        Vector3 position = agent.position + wanderOffset * new Vector3(Mathf.Sin(agent.orientation), 0, Mathf.Cos(agent.orientation));
        //agent.DrawConcentricCircle(wanderRadius);
        agent.DrawCircle(position, wanderRadius);

        

        // Calculate the wander target 
        position += wanderRadius * new Vector3(Mathf.Sin(orientation), 0, Mathf.Cos(orientation));

        // direction to wander target
        Vector3 direction = position - agent.position;

        // Get the naive direction to the target
        float rotation = Mathf.Atan2(direction.x, direction.z) - agent.orientation;

        //clip to [-pi,pi]
        while (rotation > Mathf.PI)
        {
            rotation -= 2 * Mathf.PI;
        }
        while (rotation < -Mathf.PI)
        {
            rotation += 2 * Mathf.PI;
        }
        float rotationSize = Mathf.Abs(rotation);

        // within targetRadius -> set roration speed to 0
        if (rotationSize < targetRadiusA)
        {
            agent.rotation = 0;
        }

        //calculate desire rotation speed
        float rotationSpeed = (rotationSize > slowRadiusA ? maxRotation : maxRotation * rotationSize / slowRadiusA);

        // apply direction
        rotationSpeed *= rotation / rotationSize;

        // Acceleration tries to get to the target rotation
        float angular_acc = rotationSpeed - agent.rotation;
        angular_acc /= timeToTarget;//angular acc

        // clip angular_acc
        if (Mathf.Abs(angular_acc) > maxAngularAcceleration)
        {
            angular_acc /= Mathf.Abs(angular_acc);
            angular_acc *= maxAngularAcceleration;
        }

        return angular_acc;

    }
    //Haoran
    //collision prediction
    //return true if target and agent will be within a close distance in a short amount of time(dangerous zone)
    public bool CollisionPrediction(out Vector3 collisionPoint)
    {
        Vector3 dp = target.position - agent.position;
        Vector3 dv = target.velocity - agent.velocity;
        float tClosest = -Vector3.Dot(dp, dv) / (dv.magnitude * dv.magnitude);
        // Debug.Log("tclosest: " + tClosest);
        Vector3 p_agent = agent.position + agent.velocity * tClosest;
        Vector3 p_target = target.position + target.velocity * tClosest;
        if (tClosest > 0f)
        {
            //agent.DrawCircle(p_agent, 0.1f);
        }
        if ((p_agent - p_target).magnitude < 3f && tClosest > 0f && tClosest < 2f)
        {
            collisionPoint = p_agent;
            return true;
        }
        else
        {
            collisionPoint = new Vector3(0f, 0f, 0f);
            return false;
        }
    }
    
    //Distance to target
    public float DistanceToTarget(){
        return Vector3.Distance(agent.position,target.position);
    } 
    
    //Haoran
    public void Stop(){
        agent.velocity = new Vector3(0f,0f,0f);
    }
    //Haoran
    public void StopTarget()
    {
        target.velocity = new Vector3(0f, 0f, 0f);
    }



    // ETC.

}
