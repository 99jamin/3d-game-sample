using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour, IObservable<GameObject>
{
    [Serializable]
    public class WeaponTriggerZone
    {
        public Vector3 position;
        public float radius;
    }
    
    [SerializeField] private WeaponTriggerZone[] _triggerZones;
    
    public int AttackPower => attackPower;
    [SerializeField] private int attackPower;
    [SerializeField] private LayerMask targetLayerMask;
    
    private List<IObserver<GameObject>> _observers = new List<IObserver<GameObject>>();
    
    // ---
    // 충돌 처리
    private Vector3[] _previousPositions;
    private HashSet<Collider> _hitColliders;
    private Ray _ray = new Ray();
    private RaycastHit[] _hits = new RaycastHit[10];
    private bool _isAttacking = false;

    private void Start()
    {
        _previousPositions = new Vector3[_triggerZones.Length];
        _hitColliders = new HashSet<Collider>();
    }

    public void AttackStart()
    {
        _isAttacking = true;
        _hitColliders.Clear();

        for (int i = 0; i < _triggerZones.Length; i++)
        {
            _previousPositions[i] = transform.position + transform.TransformVector(_triggerZones[i].position);
        }
    }

    public void AttackEnd()
    {
        _isAttacking = false;
    }

    private void FixedUpdate()
    {
        
    }

    public void Subscribe(IObserver<GameObject> observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void Unsubscribe(IObserver<GameObject> observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(GameObject value)
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(value);
        }
    }

    private void OnDestroy()
    {
        foreach (var observer in _observers)
        {
            observer.OnCompleted();
        }
        _observers.Clear();
    }
    
#if UNITY_EDITOR
    
    private void OnDrawGizmos()
    {
        foreach (var triggerZone in _triggerZones)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(triggerZone.position, triggerZone.radius);
        }
    }
    
#endif
}
