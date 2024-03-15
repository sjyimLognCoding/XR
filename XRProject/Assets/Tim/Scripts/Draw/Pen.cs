using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pen : MonoBehaviour
{
    [SerializeField] private Transform penTip;
    [SerializeField] private int tipSize;
    [SerializeField] private float tipLength;
    private Vector2 lastPosition;
    private Quaternion lastRotation;
    public Renderer r;

    [SerializeField] Color[] inkColour;

    RaycastHit hit;

    public Paper paper;


    public bool collidedLastFrame;

    private void Start()
    {
        r = penTip.GetComponent<Renderer>();
        tipLength = penTip.localScale.y;
        paper = GameObject.FindGameObjectWithTag("Paper").GetComponent<Paper>();
        inkColour = Enumerable.Repeat(r.material.color, tipSize * tipSize).ToArray();
    }

    private void Update()
    {
        Draw();
    }

    private void Draw()
    {
        if (Physics.Raycast(penTip.position, transform.up, out hit, tipLength))
        {
            if (hit.transform.CompareTag("Paper"))
            {
                // null safety check
                if (paper == null)
                {
                    // this is fine as long as the number of GameObjects with "Paper" tag
                    // is reasonably low
                    // paper = GameObject.FindGameObjectWithTag("Paper").GetComponent<Paper>();

                    // this will make sure that you get the reference of Paper
                    //script that your pen is pointing at
                    paper = hit.transform.GetComponent<Paper>();
                }

                // Draw on texture

                Vector2 hitPos = new Vector2(hit.textureCoord.x, hit.textureCoord.y);

                int x = (int)(hitPos.x * paper.textureSize.x - (tipSize / 2));
                int y = (int)(hitPos.y * paper.textureSize.y - (tipSize / 2));

                if (x < 0 || y < 0 || x > paper.textureSize.x || y > paper.textureSize.y) return;

                if (collidedLastFrame)
                {
                    paper.texture.SetPixels(x, y, tipSize, tipSize, inkColour);

                    for (float f = 0.01f; f < 1.00f; f += 0.03f)
                    {
                        int newX = (int)Mathf.Lerp(lastPosition.x, x, f);
                        int newY = (int)Mathf.Lerp(lastPosition.y, y, f);

                        paper.texture.SetPixels(newX, newY, tipSize, tipSize, inkColour);
                    }

                    transform.rotation = lastRotation;

                    paper.texture.Apply();
                }

                lastPosition = new Vector2(x, y);
                lastRotation = transform.rotation;
                collidedLastFrame = true;
                return;
            }
        }

        paper = null;
        collidedLastFrame = false;
    }
}
