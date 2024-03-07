using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BurnVFXController : MonoBehaviour
{
    public MeshRenderer m;
    private Material[] materials2;
    public VisualEffect VFXGraph;

    [Range(0.001f, 0.05f)] public float burnspeed = 0.01f;
    public float refreshRate = 0.05f;

    private void Start()
    {
        if (m != null)
        {
            materials2 = m.materials;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(BurningCoroutine());
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            materials2[0].SetFloat("_BurnAmount", 0);
        }
    }

    private IEnumerator BurningCoroutine()
    {
        if (VFXGraph != null)
        {
            VFXGraph.Play();
        }

        float counter = 0f;

        while (materials2[0].GetFloat("_BurnAmount") < 1)
        {
            counter += burnspeed;
            for (int i = 0; i < materials2.Length; i++)
            {
                materials2[i].SetFloat("_BurnAmount", counter);
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
