using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlassBottleController : MonoBehaviour
{
    public GameController gameController;
    public GlassBottleController bottleControllerRef;
    private int numberOfColorsToTransfer = 0;

    public Color[] bottleColors;
    public SpriteRenderer bottleMaskSR;

    public AnimationCurve ScaleAndRotationMultiplierCurve;
    public AnimationCurve FillAmountCurve;

    public AnimationCurve RotationSpeedMultiplier;

    public float[] fillAmounts;
    public float[] rotationValues;

    public int rotationIndex = 0;

    [Range(0, 3)]
    public int numberOfColorsInBottle = 3;

    public Color topColor;
    public int numberOfTopColorLayers = 1;

    public Transform leftRotationPoint;
    public Transform rightRotationPoint;
    private Transform chosenRotationPoint;

    private float directionMultiplier = 1.0f;

    Vector3 originalPos;
    Vector3 startPos;
    Vector3 endPos;

    public LineRenderer lineRenderer;

    public bool canPick;

    //moi lan cong or tru 0.65

    void Start()
    {
        bottleMaskSR.material.SetFloat("_FillAmount", fillAmounts[numberOfColorsInBottle]);

        originalPos = transform.position;

        UpdateColorFromShader();

        UpdateTopColorValues();

        canPick = true;
    }

    private void Update()
    {
        //if(Input.GetKeyUp(KeyCode.P) && justThisBottle == true)
        //{
        //    UpdateTopColorValues();

        //    if (bottleControllerRef.FillBottleCheck(topColor))
        //    {
        //        ChoseRotationPointAndDirection();

        //        numberOfColorsToTransfer = Mathf.Min(numberOfTopColorLayers, 3 - bottleControllerRef.numberOfColorsInBottle);

        //        for(int i = 0; i < numberOfColorsToTransfer; i++)
        //        {
        //            bottleControllerRef.bottleColors[bottleControllerRef.numberOfColorsInBottle + i] = topColor;
        //        }

        //        bottleControllerRef.UpdateColorFromShader();
        //        CalculateRotationIndex(3 - bottleControllerRef.numberOfColorsInBottle);
        //        StartCoroutine(RotateBottle());
        //    }
        //    else
        //    {
        //        Debug.Log("can't fill");
        //    }
        //}
    }

    public IEnumerator PickBottle()
    {
        startPos = transform.position;
        endPos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

        float t = 0;

        while (t <= 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);

            t += Time.deltaTime * 2;

            yield return new WaitForEndOfFrame();
        }

        transform.position = endPos;
    }

    public IEnumerator UnPickBottle()
    {
        startPos = transform.position;
        endPos = originalPos;

        float t = 0;

        while (t <= 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);

            t += Time.deltaTime * 2;

            yield return new WaitForEndOfFrame();
        }

        transform.position = endPos;
    }

    public void StartColorTransfer()
    {

        ChoseRotationPointAndDirection();

        numberOfColorsToTransfer = Mathf.Min(numberOfTopColorLayers, 3 - bottleControllerRef.numberOfColorsInBottle);

        for (int i = 0; i < numberOfColorsToTransfer; i++)
        {
            bottleControllerRef.bottleColors[bottleControllerRef.numberOfColorsInBottle + i] = topColor;
        }

        bottleControllerRef.UpdateColorFromShader();

        CalculateRotationIndex(3 - bottleControllerRef.numberOfColorsInBottle);

        transform.GetComponent<SpriteRenderer>().sortingLayerName = "BottleFill";
        bottleMaskSR.sortingLayerName = "BottleFill";

        StartCoroutine(MoveBottle());
    }

    IEnumerator MoveBottle()
    {
        startPos = transform.position;
        if (chosenRotationPoint == leftRotationPoint)
        {
            endPos = bottleControllerRef.rightRotationPoint.position;
        }
        else
        {
            endPos = bottleControllerRef.leftRotationPoint.position;
        }

        float t = 0;

        while (t <= 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);

            t += Time.deltaTime * 2;

            yield return new WaitForEndOfFrame();
        }

        transform.position = endPos;

        StartCoroutine(RotateBottle());
    }

    IEnumerator MoveBottleBack()
    {
        startPos = transform.position;
        endPos = originalPos;

        float t = 0;

        while (t <= 1)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t);

            t += Time.deltaTime * 2;

            yield return new WaitForEndOfFrame();
        }

        transform.position = endPos;

        transform.GetComponent<SpriteRenderer>().sortingLayerName = "Bottle";
        bottleMaskSR.sortingLayerName = "Bottle";

        gameController.SetTrueCanPick();
    }

    void UpdateColorFromShader()
    {
        bottleMaskSR.material.SetColor("_C1", bottleColors[0]);
        bottleMaskSR.material.SetColor("_C2", bottleColors[1]);
        bottleMaskSR.material.SetColor("_C3", bottleColors[2]);
    }

    public float TimeToRotate = 1.0f;
    IEnumerator RotateBottle()
    {
        float t = 0;
        float lerpValue;
        float angleValue;

        float lastAngleValue = 0;

        while (t < TimeToRotate)
        {
            lerpValue = t / TimeToRotate;
            Debug.Log("Lerp Value" + lerpValue);
            angleValue = Mathf.Lerp(0.0f, directionMultiplier * rotationValues[rotationIndex], lerpValue);
            Debug.Log("Angle Value" + angleValue);
            Debug.Log("Last Angle Value" + lastAngleValue);

            //transform.eulerAngles = new Vector3(0, 0, angleValue);

            transform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);

            bottleMaskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));

            if (fillAmounts[numberOfColorsInBottle] > FillAmountCurve.Evaluate(angleValue) + 0.005f)
            {
                if (lineRenderer.enabled == false)
                {
                    lineRenderer.startColor = topColor;
                    lineRenderer.endColor = topColor;

                    lineRenderer.SetPosition(0, chosenRotationPoint.position);
                    lineRenderer.SetPosition(1, chosenRotationPoint.position - Vector3.up * 1.45f);

                    lineRenderer.enabled = true;
                }

                bottleMaskSR.material.SetFloat("_FillAmount", FillAmountCurve.Evaluate(angleValue));

                bottleControllerRef.FillUp(FillAmountCurve.Evaluate(lastAngleValue) - FillAmountCurve.Evaluate(angleValue));
            }

            t += Time.deltaTime * RotationSpeedMultiplier.Evaluate(angleValue);
            lastAngleValue = angleValue;
            yield return new WaitForEndOfFrame();
        }

        angleValue = directionMultiplier * rotationValues[rotationIndex];
        //transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMaskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));
        bottleMaskSR.material.SetFloat("_FillAmount", FillAmountCurve.Evaluate(angleValue));

        numberOfColorsInBottle -= numberOfColorsToTransfer;
        bottleControllerRef.numberOfColorsInBottle += numberOfColorsToTransfer;

        lineRenderer.enabled = false;

        StartCoroutine(RotateBottleBack());
    }

    IEnumerator RotateBottleBack()
    {
        float t = 0;
        float lerpValue;
        float angleValue;

        float lastAngleValue = directionMultiplier * rotationValues[rotationIndex];

        while (t < TimeToRotate)
        {
            lerpValue = t / TimeToRotate;
            angleValue = Mathf.Lerp(directionMultiplier * rotationValues[rotationIndex], 0.0f, lerpValue);

            //transform.eulerAngles = new Vector3(0, 0, angleValue);

            transform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);

            bottleMaskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));

            lastAngleValue = angleValue;

            t += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        UpdateTopColorValues();
        angleValue = 0.0f;
        transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMaskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));

        StartCoroutine(MoveBottleBack());
    }

    public void UpdateTopColorValues()
    {
        if (numberOfColorsInBottle != 0)
        {
            numberOfTopColorLayers = 1;

            topColor = bottleColors[numberOfColorsInBottle - 1];

            if (numberOfColorsInBottle == 3)
            {
                if (bottleColors[2].Equals(bottleColors[1]))
                {
                    numberOfTopColorLayers = 2;
                    if (bottleColors[1].Equals(bottleColors[0]))
                    {
                        numberOfTopColorLayers = 3;
                    }
                }
            }
            else if (numberOfColorsInBottle == 2)
            {
                if (bottleColors[1].Equals(bottleColors[0]))
                {
                    numberOfTopColorLayers = 2;
                }
            }

            rotationIndex = 2 - (numberOfColorsInBottle - numberOfTopColorLayers);
        }
    }

    public bool FillBottleCheck(Color colorToCheck)
    {
        if (numberOfColorsInBottle == 0)
        {
            return true;
        }
        else
        {
            if (numberOfColorsInBottle == 3)
            {
                return false;
            }
            else
            {
                if (topColor.Equals(colorToCheck))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    private void CalculateRotationIndex(int numberOfEmptySpacesInSecondBottle)
    {
        rotationIndex = 2 - (numberOfColorsInBottle - Mathf.Min(numberOfEmptySpacesInSecondBottle, numberOfTopColorLayers));
    }

    private void FillUp(float fillAmountToAdd)
    {
        bottleMaskSR.material.SetFloat("_FillAmount", bottleMaskSR.material.GetFloat("_FillAmount") + fillAmountToAdd);
    }

    private void ChoseRotationPointAndDirection()
    {
        if (transform.position.x > bottleControllerRef.transform.position.x)
        {
            chosenRotationPoint = leftRotationPoint;
            directionMultiplier = -1.0f;
        }
        else
        {
            chosenRotationPoint = rightRotationPoint;
            directionMultiplier = 1.0f;
        }
    }
}
