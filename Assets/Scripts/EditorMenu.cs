using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorMenu : MonoBehaviour
{
    static GameObject pathNode;
    static GameObject[] spawner;
    static GameObject[] allNodes;

    // Create a sub menu item called Spawn Floor Nodes under the menu item Grid Generation
    [MenuItem("Grid Generation/Spawn Floor Nodes", priority = 0)]

    // Spawn pathnodes on a selected floor
    static void SpawnFloorNodes()
    {
        // The transform of the floor
        //Transform floor;
        // The extents of the floor
        Vector3 floorExtents;
        // The count for giving all nodes and unique ID
        int count = 0;

        // Load the Pathnode prefab from the Resources folder
        pathNode = Resources.Load("Pathnode") as GameObject;

        // Clear all the nodes currently attached to the selected floor
        ClearFloorNodes();

        // Store the transform of the selected floor
        //floor = Selection.transforms[0];

        foreach (Transform floor in Selection.transforms)
        {
            // Store the extents of the selected floor leaving a small margin around the edges by subtracting a Vector3
            floorExtents = floor.gameObject.GetComponent<BoxCollider>().bounds.extents - new Vector3(1, 0, 1);

            // Starting at the bottom left corner spawn pathnodes along the entire surface of the selected floor
            for (float i = floorExtents.x * -2; i <= 0; i += 2)
            {
                for (float j = floorExtents.z * -2; j <= 0; j += 2)
                {
                    GameObject spawnedNode = Instantiate(pathNode, floor.position + floorExtents + new Vector3(i, 0, j), floor.rotation);
                    count++;
                    spawnedNode.name = "Pathnode" + count;
                    spawnedNode.transform.parent = floor;
                }
            }
        }





    }

    // Validate the Spawn Floor Nodes sub menu item
    [MenuItem("Grid Generation/Spawn Floor Nodes", true)]

    static bool ValidateSpawnFloorNodes()
    {
        bool validated = true;

        // Allow the user to spawn nodes if only one floor is selected and it is tagged properly

        foreach (Transform sphere in Selection.transforms)
        {
            if (sphere.tag != "Floor")
            {
                validated = false;
            }
        }

        return validated;
    }

    [MenuItem("Grid Generation/Clear Floor Nodes", priority = 1)]

    static void ClearFloorNodes()
    {
        int breakout = 0;
        foreach (Transform floor in Selection.transforms)
        {
            //Destroy the child object at the first index until there are no children
            for (; floor.transform.childCount > 0; breakout++)
            {
                DestroyImmediate(floor.transform.GetChild(0).gameObject);

                // If the loop does not appear to be ending then break
                if (breakout > 10000)
                {
                    Debug.Log("infinite");
                    break;
                }
            }
        }
        
    }

    [MenuItem("Grid Generation/Clear Floor Nodes", true)]

    static bool ValidateClearFloorNodes()
    {
        bool validated = true;


        foreach (Transform floor in Selection.transforms)
        {
            if (floor.tag != "Floor")
            {
                validated = false;
            }
        }

        return validated;
    }

    // Create a sub menu item called Spawn Trigger Nodes under the menu item Grid Generation
    [MenuItem("Grid Generation/Spawn Trigger Nodes", priority = 50)]

    // Spawn pathnodes in a selected trigger
    static void SpawnTriggerNodes()
    {
        // The transform of the trigger
        Transform trigger;
        // The extents of the trigger
        Vector3 triggerExtents;
        // The count for giving all nodes and unique ID
        int count = 0;

        RaycastHit hit;

        // Load the Pathnode prefab from the Resources folder
        pathNode = Resources.Load("Pathnode") as GameObject;

        // Clear all the nodes currently attached to the selected trigger
        ClearTriggerNodes();

        // Store the transform of the selected floor
        trigger = Selection.transforms[0];

        // Store the extents of the selected trigger leaving a small margin around the edges by subtracting a Vector3
        triggerExtents = trigger.gameObject.GetComponent<BoxCollider>().bounds.extents - new Vector3(1, 0, 1);

        // Starting at the bottom left corner spawn pathnodes along the entire surface of the selected trigger
        for (float i = triggerExtents.x * -2; i <= 0; i += 3)
        {
            for (float j = triggerExtents.z * -2; j <= 0; j += 3)
            {
                // Raycast down from the current location
                if (Physics.Raycast(trigger.position + triggerExtents + new Vector3(i, 0, j), Vector3.down, out hit))
                {
                    // If the raycast hits the floor object then spawn a node at that location
                    if (hit.transform.tag == "Floor")
                    {
                        GameObject spawnedNode = Instantiate(pathNode, hit.point, trigger.rotation);
                        count++;
                        spawnedNode.name = "Pathnode" + count;
                        spawnedNode.transform.parent = trigger;
                    } 
                }
            }
        }



    }

    // Validate the Spawn Trigger Nodes sub menu item
    [MenuItem("Grid Generation/Spawn Trigger Nodes", true)]

    static bool ValidateSpawnTriggerNodes()
    {
        bool validated = true;

        // Allow the user to spawn nodes if only one floor is selected and it is tagged properly
        if (Selection.transforms.Length == 1)
        {
            foreach (Transform sphere in Selection.transforms)
            {
                if (sphere.tag != "SpawnTrigger")
                {
                    validated = false;
                }
            }
        }
        else
        {
            validated = false;
        }
        return validated;
    }


    [MenuItem("Grid Generation/Clear Trigger Nodes", priority = 50)]

    static void ClearTriggerNodes()
    {
        int breakout = 0;

        // Destroy the child object at the first index until there are no children
        for (; Selection.gameObjects[0].transform.childCount > 0; breakout++)
        {
            DestroyImmediate(Selection.gameObjects[0].transform.GetChild(0).gameObject);

            // If the loop does not appear to be ending then break
            if (breakout > 10000)
            {
                Debug.Log("infinite");
                break;
            }
        }
    }

    [MenuItem("Grid Generation/Clear Trigger Nodes", true)]

    static bool ValidateTriggerFloorNodes()
    {
        bool validated = true;

        if (Selection.transforms.Length == 1)
        {
            foreach (Transform sphere in Selection.transforms)
            {
                if (sphere.tag != "SpawnTrigger")
                {
                    validated = false;
                }
            }
        }
        else
        {
            validated = false;
        }
        return validated;
    }

    [MenuItem("Grid Generation/Clear All Nodes", priority = 100)]

    static void ClearAllNodes()
    {
        allNodes = GameObject.FindGameObjectsWithTag("Pathnode");
        foreach (GameObject node in allNodes)
        {
            DestroyImmediate(node);
        }
    }

    [MenuItem("Grid Generation/Build Paths", priority = 100)]

    static void BuildPaths()
    {
        allNodes = GameObject.FindGameObjectsWithTag("Pathnode");
        foreach (GameObject node in allNodes)
        {
            node.GetComponent<Pathnode>().ClearConnections();

            foreach (GameObject target in allNodes)
            {
                // Make sure the target node is not the current node
                if (node.transform != target.transform)
                {
                    if (Vector3.Distance(node.transform.position, target.transform.position) <= 3.1f &&
                        !Physics.SphereCast(new Ray(node.transform.position + new Vector3(0, 1, 0), target.transform.position - node.transform.position), 0.4f, Vector3.Distance(node.transform.position, target.transform.position)) &&
                        !Physics.CheckSphere(node.transform.position + new Vector3(0, 1, 0), 0.4f))
                    {
                        node.GetComponent<Pathnode>().AddConnection(target);
                    }
                }
            }
        }
    }

    [MenuItem("Grid Generation/Clear Paths", priority = 100)]

    static void ClearPaths()
    {
        allNodes = GameObject.FindGameObjectsWithTag("Pathnode");
        foreach (GameObject node in allNodes)
        {
            node.GetComponent<Pathnode>().ClearConnections();
        }
    }
}
