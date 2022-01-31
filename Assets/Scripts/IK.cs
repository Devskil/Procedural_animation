using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK : MonoBehaviour
{
    // Le transform (noeud) racine de l'arbre, 
    // le constructeur créera une sphère sur ce point pour en garder une copie visuelle.
    public GameObject rootNode = null;
    private GameObject root;
    // Un transform (noeud) (probablement une feuille) qui devra arriver sur targetNode
    public Transform srcNode = null;

    // Le transform (noeud) cible pour srcNode
    public Transform targetNode = null;

    // Si vrai, recréer toutes les chaines dans Update
    public bool createChains = true;

    // Toutes les chaines cinématiques 
    public List<IKChain> chains = new List<IKChain>();

    // Nombre d'itération de l'algo à chaque appel
    public int nb_ite = 10;

    private List<Transform> leafs = new List<Transform>();
    private List<Transform> crossings = new List<Transform>();
    void returnLeafs(Transform t)
    {
        int i = t.childCount;
        if (i != 1)
            leafs.Add(t);
        if(i > 1)
        {
            crossings.Add(t);
            for (int j = 0; j < i; ++j)
                returnLeafs(t.GetChild(j));         
        }
        else if (i == 1)
            returnLeafs(t.GetChild(0));
    }
    void Start()
    {
        if (createChains)
        {
            Debug.Log("(Re)Create CHAIN");
            root = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            root.transform.localScale = new Vector3(0.01f,0.01f,0.01f);
            root.transform.position = rootNode.transform.position;
            root.transform.rotation = rootNode.transform.rotation;
            root.GetComponent<SphereCollider>().enabled = false;
            // TODO : 
            // Création des chaines : une chaine cinématique est un chemin entre deux noeuds carrefours.
            // Dans la 1ere question, une unique chaine sera suffisante entre srcNode et rootNode.
            returnLeafs(rootNode.transform);
            // TODO-2 : Pour parcourir tous les transform d'un arbre d'Unity vous pouvez faire une fonction récursive
            // ou utiliser GetComponentInChildren comme ceci :
            // foreach (Transform tr in gameObject.GetComponentsInChildren<Transform>())
            foreach (Transform t in leafs)
                chains.Add(new IKChain(t, rootNode, root.transform));

            // TODO-2 : Dans le cas où il y a plusieurs chaines, fusionne les IKJoint entre chaque articulation.
            foreach(Transform cross in crossings)
            {
                IKJoint tmp = new IKJoint(cross);
                foreach (IKChain chain in chains) 
                    chain.Merge(tmp);
            }
            createChains = false;
        }
    }

    void Update()
    {
        if (createChains)
            Start();

        root.transform.position = rootNode.transform.position;
        IKOneStep(true);

        if (Input.GetKeyDown(KeyCode.C))
        {
            foreach (IKChain ch in chains)
                ch.Check();
        }
    }


    void IKOneStep(bool down)
    {
        int j;

        for (j = 0; j < nb_ite; ++j)
        {
            for (int i = chains.Count - 1; i >= 0; --i)
            {
                chains[i].Backward();
            }

            for (int i = chains.Count - 1; i >= 0; --i)
                chains[i].ToTransform();

            foreach (IKChain c in chains)
                c.ResetPos();
            foreach (IKChain c in chains)
                c.Forward();
            foreach (IKChain c in chains)
                c.ToTransform();
            foreach (IKChain c in chains)
                c.ResetPos();
        }
    }
    void OnAnimatorMove()
    {
        foreach (IKChain c in chains)
            c.MoveTarget();
    }
}
