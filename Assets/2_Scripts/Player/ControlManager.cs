using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    [SerializeField] private PlayerUnit[] allPlayer;
    private LinkedList<PlayerUnit> _players = new LinkedList<PlayerUnit>();
    private Camera _mainCam;

    void Start()
    {
        _mainCam = Camera.main;
        foreach (PlayerUnit player in allPlayer)
        {
            _players.AddLast(player);
        }
    }

    void MoveControl(Vector3 position)
    {
        foreach (PlayerUnit playerUnit in _players)
        {
            playerUnit.Move(position);
        }
    }

    public void ShootRay()
    {
        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            MoveControl(hit.point);
            //. 충돌한 오브젝트 정보 출력
            Debug.Log($"레이가 충돌한 오브젝트: {hit.collider.name}");
        }
        else
        {
            Debug.Log("레이가 아무것도 충돌하지 않았습니다.");
        }
    }
}