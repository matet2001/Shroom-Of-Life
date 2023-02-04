using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] Transform[] resourceAmountTransforms;
    [Space] public Image[] resourceAmountSliders;

    /// <summary>
    /// Below: debug variables
    /// TODO: delete later
    /// </summary>
    public TMPro.TextMeshProUGUI[] currentResourcesText;

    public TMPro.TextMeshProUGUI[] maximumResourcesText;

    private ResourceData resourceData;

    private void Start()
    {
        SetUpResourceAmountUI();

        ResourceManager.Instance.OnResourceAmountChange += Instance_OnResourceAmountChange;

        resourceData = ResourceManager.Instance.resourceData;
    }

    private void SetUpResourceAmountUI()
    {
        string[] resourceNames = ResourceManager.Instance.GetResourceNames();
        float[] currentResourceAmounts = ResourceManager.Instance.GetResourceAmounts();
        float[] currentMaximumResourceAmounts = ResourceManager.Instance.GetCurrentMaximumResourceAmounts();
        float[] maximumPossibleResourceAmounts = ResourceManager.Instance.GetMaximumResourceAmounts();

        for (int i = 0; i < resourceAmountTransforms.Length; i++)
        {
            currentResourcesText[i].text =$"current res. {ResourceManager.Instance.GetResourceAmounts()[i].ToString()}" ;
            maximumResourcesText[i].text = $"max res. {ResourceManager.Instance.GetMaximumResourceAmounts()[i].ToString()}";
            TextMeshProUGUI[] resourceAmountTexts = resourceAmountTransforms[i].GetComponentsInChildren<TextMeshProUGUI>();
            resourceAmountTexts[i].text = resourceNames[i];
            resourceAmountSliders[i].fillAmount = 
                // ReSharper disable once PossibleLossOfFraction
                (currentResourceAmounts[i] / currentMaximumResourceAmounts[i]) /
                                                   maximumPossibleResourceAmounts[i];
        }
    }
    private void RefreshResourceAmountUI()
    {

    }
    private void Instance_OnResourceAmountChange(ResourceTypeSO[] arg1, float[] resourceAmount)
    {
        string[] resourceNames = ResourceManager.Instance.GetResourceNames();
        float[] currentResourceAmounts = ResourceManager.Instance.GetResourceAmounts();
        float[] currentMaximumResourceAmounts = ResourceManager.Instance.GetCurrentMaximumResourceAmounts();
        // int[] maximumPossibleResourceAmounts = ResourceManager.Instance.GetMaximumResourceAmounts();
        
        for (int i = 0; i < resourceAmountTransforms.Length; i++)
        {
            currentResourcesText[i].text =$"current res. {ResourceManager.Instance.GetResourceAmounts()[i].ToString()}" ;
            maximumResourcesText[i].text = $"max res. {ResourceManager.Instance.GetMaximumResourceAmounts()[i].ToString()}";
            TextMeshProUGUI[] resourceAmountTexts = resourceAmountTransforms[i].GetComponentsInChildren<TextMeshProUGUI>();
            resourceAmountTexts[0].text = resourceNames[i];
            resourceAmountSliders[i].fillAmount = 
               (currentResourceAmounts[i] / currentMaximumResourceAmounts[i]);
        }
    }
}