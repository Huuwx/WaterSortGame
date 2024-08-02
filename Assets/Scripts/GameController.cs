using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GlassBottleController FirstBottle;
    public GlassBottleController SecondBottle;

    public bool canPick;

    // Start is called before the first frame update
    void Start()
    {
        canPick = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) && canPick)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousPos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousPos2D, Vector2.zero);

            if(hit.collider != null)
            {
                if(hit.collider.GetComponent<GlassBottleController>() != null)
                {
                    if(FirstBottle == null)
                    {
                        FirstBottle = hit.collider.GetComponent<GlassBottleController>();
                        StartCoroutine(FirstBottle.PickBottle());
                    }
                    else
                    {
                        if(FirstBottle == hit.collider.GetComponent<GlassBottleController>())
                        {
                            StartCoroutine(FirstBottle.UnPickBottle());
                            FirstBottle = null;
                        }
                        else
                        {
                            canPick = false;
                            SecondBottle = hit.collider.GetComponent<GlassBottleController>();
                            FirstBottle.bottleControllerRef = SecondBottle;

                            FirstBottle.UpdateTopColorValues();
                            SecondBottle.UpdateTopColorValues();

                            if (SecondBottle.FillBottleCheck(FirstBottle.topColor) == true)
                            {
                                FirstBottle.StartColorTransfer();
                                FirstBottle = null;
                                SecondBottle = null;
                            }
                            else
                            {
                                StartCoroutine(FirstBottle.UnPickBottle());
                                FirstBottle = null;
                                SecondBottle = null;
                            }
                        }
                    }
                }
            }
        }
    }

    public void SetTrueCanPick()
    {
        canPick = true;
    }
}
