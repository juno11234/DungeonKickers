using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    Transform target;
    LinkedList<PlayerUnit> players;
    void Start()
    {

    }

    void Update()
    {

    }
    void MoveControl()
    {
        foreach (PlayerUnit playerUnit in players)
        {
            playerUnit.Move(target);
        }
    }
}
