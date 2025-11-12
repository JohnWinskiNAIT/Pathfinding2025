using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathnode : MonoBehaviour
{
    public List<GameObject> connections;
    public List<GameObject> force;
    public List<GameObject> block;
    public List<GameObject> activeConnections;

    Pathnode()
    {
        connections = new List<GameObject>();
        force = new List<GameObject>();
        block = new List<GameObject>();
        activeConnections = new List<GameObject>();
    }

    public void AddConnection(GameObject target)
    {
        connections.Add(target);
    }

    public void ClearConnections()
    {
        connections = new List<GameObject>();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.5f);

        foreach (GameObject target in connections)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, target.transform.position + new Vector3(0, 0.5f, 0));
        }
    }
}