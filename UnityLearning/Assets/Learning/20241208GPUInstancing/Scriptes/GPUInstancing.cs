using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUInstancing : MonoBehaviour
{
    public Transform _prefab;
    public int _instanceCount;
    public float _radius;
    public bool _randomCol = false;
    // Start is called before the first frame update
    void Start()
    {
        Transform parent = new GameObject("Dynamic Gameobject").transform;
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        for (int i = 0; i < _instanceCount; i++)
        {
            Transform t = Instantiate(_prefab, parent);
            t.localPosition = Random.insideUnitSphere * _radius;
            //t.GetComponent<MeshRenderer>().sharedMaterial.color = Random.ColorHSV();
            block.SetColor("_Color" , Random.ColorHSV());
            t.GetComponent<MeshRenderer>().SetPropertyBlock(block);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
