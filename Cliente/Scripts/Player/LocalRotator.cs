using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalRotator : MonoBehaviour
{
    public float xChange;
    public float yChange;
    public float zChange;

    public Transform target;
    private void LateUpdate()
    {
       // transform.Translate(target.localPosition.x - xChange, target.localPosition.y - yChange, target.localPosition.z - zChange, Space.World);
         this.transform.localPosition = new Vector3(target.localPosition.x - xChange, target.localPosition.y - yChange, target.localPosition.z - zChange);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
