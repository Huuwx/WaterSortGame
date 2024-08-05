using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GlassBottleController[] ListBottle;

    public GlassBottleController FirstBottle;
    public GlassBottleController SecondBottle;

    public GameObject completedPanel;

    public bool canPick;

    // Start is called before the first frame update
    void Start()
    {
        canPick = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canPick)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousPos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousPos2D, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.GetComponent<GlassBottleController>() != null)
                {
                    if (FirstBottle == null)
                    {
                        FirstBottle = hit.collider.GetComponent<GlassBottleController>();
                        if (FirstBottle.getCompletedBottle() == true || FirstBottle.numberOfColorsInBottle == 0)
                        {
                            FirstBottle = null;
                            return;
                        }  
                        StartCoroutine(FirstBottle.PickBottle());
                    }
                    else
                    {
                        Debug.Log("skdjhf");
                        if (FirstBottle.getCompletedBottle() == false)
                        {
                            if (FirstBottle == hit.collider.GetComponent<GlassBottleController>())
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
                                    canPick = true;
                                    FirstBottle = null;
                                    SecondBottle = null;
                                }
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

    public void CheckCompleted()
    {
        int check = 0;
        for(int i = 0; i < ListBottle.Length; i++)
        {
            if (ListBottle[i].completedBottle == true)
            {
                check++;
            }
        }
        if(check == 6)
        {
            completedPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void Replay()
    {
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
