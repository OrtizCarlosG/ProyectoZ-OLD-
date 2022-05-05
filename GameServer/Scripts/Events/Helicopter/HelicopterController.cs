using UnityEngine;
using UnityEngine.UI;

public class HelicopterController : MonoBehaviour
{
    public Transform target;
    public float forwardForce = 5.0f;
    public float throttle = 0.0f;
    public float minimumThrottle = 0.0f;
    public float maximumThrottle = 100.0f;
    public float heightDistance = 0.0f;
    public float heightReaction = 10.0f;
    public float turnSpeed = 6.0f;
    public float accuracy = 10.0f;
    public float minAltitude = 20.0f;
    public float safeDistance = 20.0f;
    public float timerMin = 1.0f;
    private float distanceToGround = 0.0f;
    private float distanceAhead = 0.0f;
    private float timer = 0.0f;
    private float startTime = 0.0f;

    private Rigidbody _helicopter;

    void Start()
    {
        startTime = Time.time;
        _helicopter = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!target)
            if (GameObject.FindWithTag("Player"))
                target = GameObject.FindWithTag("Player").transform;
            else
                return;
        if (target)
        {
            // Timer
            timer = Time.time - startTime;
            heightDistance = (target.position - transform.position).y;
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(transform.position, transform.forward, out hit))
                distanceAhead = hit.distance;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit))
                distanceToGround = hit.distance;
            // If there's nothing ahead of us...
            if (distanceAhead > safeDistance)
            {
                _helicopter.AddForce(transform.forward * forwardForce);
            }
            else
            {
                _helicopter.AddForce(-transform.forward * forwardForce);
            }
            // Choose whether to go up or down
            if (distanceToGround > minAltitude)
                throttle = Mathf.Lerp(minimumThrottle, maximumThrottle, ((heightDistance - minAltitude) / heightReaction));
            else
                throttle = Mathf.Lerp(throttle, maximumThrottle, (Time.deltaTime * heightReaction));
            // Make the throttle happen
            _helicopter.AddForce(Vector3.up * throttle);
            // The 'gunner' part of the script
            var predictedTargetPosition = target.position;
            var relativePos = predictedTargetPosition - transform.position;
            var rotation = Quaternion.LookRotation(relativePos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
            var targetDirection = target.position - transform.position;
            if (Vector3.Angle(targetDirection, transform.forward) < accuracy)
                if (timer > timerMin)
                {
                    BroadcastMessage("Fire", target, SendMessageOptions.DontRequireReceiver);
                    startTime = Time.time;
                }
        }
    }

}