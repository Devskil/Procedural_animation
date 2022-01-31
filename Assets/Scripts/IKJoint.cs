using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKJoint
{
    // la position modifiée par l'algo : en fait la somme des positions des sous-branches. 
    // _weight comptera le nombre de sous-branches ayant touchées cette articulation.
    private Vector3 _position;

    // un lien vers le Transform de l'arbre d'Unity
    private Transform _transform;

    // un poids qui indique combien de fois ce point a été bougé par l'algo.
    private float _weight = 0.0f;


    public string name
    {
        get
        {
            return _transform.name;
        }
    }
    public Vector3 position     // la position moyenne
    {
        get
        {
            if (_weight == 0.0f) return _position;
            else return _position / _weight;
        }
    }

    public Vector3 positionTransform
    {
        get
        {
            return _transform.position;
        }
    }

    public Transform transform
    {
        get
        {
            return _transform;
        }
    }

    public Vector3 positionOrigParent
    {
        get
        {
            return _transform.parent.position;
        }
    }

    public IKJoint(Transform t)
    {
        _transform = t;
        _position = Vector3.zero;
        _weight = 0;
    }

    public void SetPosition(Vector3 p)
    {
        _position = p;
    }

    public void AddPosition(Vector3 p)
    {
        _position += p;
        _weight++;
    }


    public void ToTransform()
    {
        if(_weight == 0f)
            _transform.position = _position;
        else
        {
            _transform.position = _position / _weight;
            _position /= _weight;
        }
        _weight = 0f;
    }

    public void Solve(IKJoint anchor, float l)
    {
        AddPosition(anchor.position + (_transform.position - anchor.position).normalized * l);
    }
}
