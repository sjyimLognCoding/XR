using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
public class BurnEvent : MonoBehaviour
{
    [Range(0.001f, 0.05f)] public float burnspeed = 0.01f;
    public float refreshRate = 0.05f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Paper"))
        {
            Material[] materials = other.GetComponent<MeshRenderer>().materials;
            VisualEffect vfx = other.GetComponent<VisualEffect>();
            StartCoroutine(BurningCoroutine(vfx, materials, other.gameObject));
        }
    }

    private IEnumerator BurningCoroutine(VisualEffect VFXGraph, Material[] materials, GameObject paper)
    {
        if (VFXGraph != null) VFXGraph.Play();
        float counter = 0f;
        while (materials[0].GetFloat("_BurnAmount") < 1)
        {
            counter += burnspeed;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i].SetFloat("_BurnAmount", counter);
            }
            yield return new WaitForSeconds(refreshRate);
        }
        Destroy(paper);
    }
}
