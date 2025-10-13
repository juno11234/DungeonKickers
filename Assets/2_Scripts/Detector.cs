using System;
using System.Collections.Generic;
using System.Linq; // LINQ 사용을 위해 추가
using UnityEngine;

public class Detector : MonoBehaviour
{
    public delegate void TargetFind(IFighter fighter);
    public event TargetFind OnTargetFind;
    public string fighterTag = "Enemy";
    Dictionary<Collider, IFighter> fighters = new Dictionary<Collider, IFighter>();
    public SphereCollider coll;

    private void Awake()
    {
        coll = GetComponent<SphereCollider>();
    }

    private void OnDisable()
    {
        fighters.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(fighterTag) && other.TryGetComponent(out IFighter fighter))
        {
            if (!fighters.ContainsKey(other))
            {
                fighters.Add(other, fighter);
                EventCloseDict();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(fighterTag))
        {
            if (fighters.ContainsKey(other))
            {
                fighters.Remove(other);
            }
        }
    }

    public void EventCloseDict()
    {
        if (fighters.Count <= 0) return;
        IFighter closer = FindClosestTarget(); // 기존 로직을 재사용
        if (closer != null)
        {
            OnTargetFind?.Invoke(closer);
        }
    }

    // [추가됨] 가장 가까운 타겟을 찾아 반환하는 Public 메서드
    public IFighter FindClosestTarget()
    {
        var keysToRemove = fighters.Where(pair => pair.Key == null || pair.Value == null || pair.Value.OnDie)
                                   .Select(pair => pair.Key)
                                   .ToList();
        foreach (var key in keysToRemove)
        {
            fighters.Remove(key);
        }

        if (fighters.Count == 0) return null;

        IFighter closestTarget = null;
        float closestDistSqr = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (var pair in fighters)
        {
            float dSqrToTarget = (pair.Value.GameObject.transform.position - currentPos).sqrMagnitude;
            if (dSqrToTarget < closestDistSqr)
            {
                closestDistSqr = dSqrToTarget;
                closestTarget = pair.Value;
            }
        }
        return closestTarget;
    }


    public void DictionaryRemove(Collider other)
    {
        if (fighters.ContainsKey(other))
        {
            fighters.Remove(other);
        }
    }

    public void DictionaryReset()
    {
        fighters.Clear();
    }
}