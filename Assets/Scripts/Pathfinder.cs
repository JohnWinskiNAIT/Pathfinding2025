using System.IO;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] GameObject startNode, endNode, currentNode, targetNode, prevNode;
    [SerializeField] GameObject[] waypoints;

    float movementSpeed = 3.0f;
    int waypointIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = startNode.transform.position;
        currentNode = startNode;
        targetNode = currentNode;
        endNode = waypoints[waypointIndex];
    }

    // Update is called once per frame
    void Update()
    {
        // If the AI is at the targetNode then find a new target to move to.
        if (Vector3.Distance(transform.position, targetNode.transform.position) < 0.1f)
        {
            prevNode = currentNode;
            currentNode = targetNode;

            if (currentNode == endNode)
            {
                waypointIndex++;
                
                if (waypointIndex >= waypoints.Length)
                {
                    waypointIndex = 0;
                }

                endNode = waypoints[waypointIndex];
            }

            float closestDistance = 10000;
            //GameObject closestNode;

            Pathnode pathscript = currentNode.GetComponent<Pathnode>();

            if (pathscript != null)
            {
                //bool found = false;
                //int pathIndex = 0;

                //int randomNum = Random.Range(0, pathscript.connections.Count);
                for (int i = 0; i < pathscript.connections.Count; i++)
                {
                    if (pathscript.connections[i] != prevNode)
                    {
                        if (Vector3.Distance(pathscript.connections[i].transform.position, endNode.transform.position) < closestDistance)
                        {
                            targetNode = pathscript.connections[i];
                            closestDistance = Vector3.Distance(pathscript.connections[i].transform.position, endNode.transform.position);
                        }
                    }
                }
                //targetNode = pathscript.connections[pathIndex];
            }
        }
        else
        {
            transform.Translate((targetNode.transform.position - transform.position).normalized * movementSpeed * Time.deltaTime);
        }
    }
}
