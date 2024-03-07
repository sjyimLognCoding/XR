using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisController : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Material[] materials;

    public float rate = 0.0125f;
    public float refresh = 0.025f;

    private void Start()
    {
        if (meshRenderer != null)
        {
            materials = meshRenderer.materials;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(effectCo());
        }
    }

    private IEnumerator effectCo()
    {
        float counter = 0;

        if (materials.Length > 0)
        {
            while (materials[0].GetFloat("_DisintegrateScale") < 1)
            {
                counter += rate;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat("_DisintegrateScale", counter);
                }

                yield return new WaitForSeconds(refresh);
            }
        }

    }

}
