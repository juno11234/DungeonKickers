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
   public void ShootRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // 3. 충돌한 오브젝트 정보 출력
            Debug.Log($"레이가 충돌한 오브젝트: {hit.collider.name}");
            Debug.Log($"충돌 지점: {hit.point}");       
        }
        else
        {
            Debug.Log("레이가 아무것도 충돌하지 않았습니다.");
        }
    } 

}
