using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    public bool InputShift { get; set; }
    public bool AttakcInput { get; set; }
    private List<PlayerUnit> _players = new List<PlayerUnit>();
    private Camera _mainCam;

    void Start()
    {
        _mainCam = Camera.main;
    }


    public void ShootRayRight()
    {
        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.gameObject.CompareTag("Ground"))
        {
            MoveControl(hit.point);

            Debug.Log($"레이가 충돌한 오브젝트: {hit.collider.name}");
        }
        else if (hit.transform.gameObject.CompareTag("Enemy"))
        {
            AttackControl();
        }
    }

    void MoveControl(Vector3 position)
    {
        foreach (PlayerUnit playerUnit in _players)
        {
            playerUnit.Move(position);
        }
    }

    void AttackControl()
    {
        foreach (PlayerUnit playerUnit in _players)
        {
            playerUnit.Attack();
        }
    }

    public void ShootRayLeft()
    {
        if (InputShift == false)
        {
            foreach (PlayerUnit players in _players)
            {
                players.CanceledSelected();
            }

            _players.Clear();
        }

        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit) &&
            hit.transform.TryGetComponent(out PlayerUnit playerUnit))
        {
            _players.Add(playerUnit);
            playerUnit.Selected();
            Debug.Log($"리스트개수: {_players.Count}");
        }

        Debug.Log($"레이가 충돌한 오브젝트: {hit.collider.name}");
    }
}