using System;
using System.Collections.Generic;
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
            if (fighters.ContainsKey(other) == false)
            {
                fighters.Add(other, fighter);
                EventCloseDict();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(fighterTag) && other.TryGetComponent(out IFighter fighter))
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
        float min = float.MaxValue;
        IFighter closer = null;

        foreach (KeyValuePair<Collider, IFighter> pair in fighters)
        {
            float distance = Vector3.Distance(transform.position, pair.Value.GameObject.transform.position);
            if (distance < min)
            {
                min = distance;
                closer = pair.Value;
            }
        }
        OnTargetFind?.Invoke(closer);
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
