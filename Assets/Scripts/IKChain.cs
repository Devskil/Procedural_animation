using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKChain
{
    // Quand la chaine comporte une cible pour la racine. 
    // Ce sera le cas que pour la chaine comportant le root de l'arbre.
    private IKJoint rootTarget = null;

    // Quand la chaine à une cible à atteindre, 
    // ce ne sera pas forcément le cas pour toutes les chaines.
    private IKJoint endTarget = null;
    private GameObject target;
    // Toutes articulations (IKJoint) triées de la racine vers la feuille. N articulations.
    private List<IKJoint> joints = new List<IKJoint>();

    // Contraintes pour chaque articulation : la longueur (à modifier pour 
    // ajouter des contraintes sur les angles). N-1 contraintes.
    private List<float> constraints = new List<float>();


    // Un cylndre entre chaque articulation (Joint). N-1 cylindres.
    //private List<GameObject> cylinders = new List<GameObject>();    



    // Créer la chaine d'IK en partant du noeud endNode et en remontant jusqu'au noeud plus haut, ou jusqu'à la racine
    public IKChain(Transform _endNode, GameObject rootNode, Transform _rootTarget = null, Transform _endTarget = null)
    {
        Debug.Log("=== IKChain::createChain: ===");
        // TODO : construire la chaine allant de _endNode vers _rootTarget en remontant dans l'arbre. 
        // Chaque Transform dans Unity a accés à son parent 'tr.parent'
        Transform currentNode = _endNode;

        if(_endNode.childCount == 0)
        {
            target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            target.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            target.transform.position = _endNode.position;
            target.transform.name = _endNode.name + "'s Target";
            target.GetComponent<SphereCollider>().enabled = false;
            target.transform.SetParent(rootNode.transform);
            endTarget = new IKJoint(target.transform);
        }
        do
        {
            IKJoint tmp = new IKJoint(currentNode);
            joints.Add(tmp);
            if(currentNode != _endNode.root)
                currentNode = currentNode.parent;
        } while (currentNode != _endNode.root && currentNode.childCount <= 1);

        IKJoint root = new IKJoint(currentNode);
        joints.Add(root);
        joints.Reverse();
        
        if (currentNode == _endNode.root)
            rootTarget = new IKJoint(_rootTarget);

        for (int i = 0; i < joints.Count - 1; ++i)
        {
            
            constraints.Add((joints[i + 1].transform.position - joints[i].transform.position).magnitude);
        }
    }


    public void Merge(IKJoint j)
    {
        // TODO-2 : fusionne les noeuds carrefour quand il y a plusieurs chaines cinématiques
        for (int i = 0; i < joints.Count; ++i)
            if (joints[i].name == j.name)
                joints[i] = j;             
    }


    public IKJoint First()
    {
        return joints[0];
    }
    public IKJoint Last()
    {
        return joints[joints.Count - 1];
    }

    public void Backward()
    {
        if (endTarget != null)
            Last().SetPosition(endTarget.transform.position);
        for (int i = joints.Count - 2; i >=0; --i)
            joints[i].Solve(joints[i + 1], constraints[i]);
    }

    public void Forward()
    {
        if(rootTarget != null)
            First().SetPosition(rootTarget.transform.position);
        for (int i = 1; i < joints.Count; ++i)
            joints[i].Solve(joints[i - 1], constraints[i - 1]);
    }

    public void ToTransform()
    {
        foreach(IKJoint j in joints)
            j.ToTransform();
    }

    public void MoveTarget()
    {
        if(target != null)
        {
            target.transform.position = Last().transform.position;
        }
        
    }

    public void ResetPos()
    {
        foreach (IKJoint j in joints)
            j.SetPosition(Vector3.zero);
    }

    public void Check()
    {
        // TODO : des Debug.Log pour afficher le contenu de la chaine (ne sert que pour le debug)

        Debug.Log(joints.Count);
        foreach(IKJoint j in joints)
        {
            Debug.Log(j.name);
        }
        foreach (float c in constraints)
        {
            Debug.Log(c);
        }
    }
}
