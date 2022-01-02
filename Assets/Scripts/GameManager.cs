﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Singleton
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    [Range(0f, 1f)] public float AutoCollectPercecntage = 0.1f;
    public float SaveDelay = 5f;
    public ResourceConfig[] ResourceConfigs;
    public Sprite[] ResourceSprites;
    public Transform ResourceParent;
    public ResourceController ResourcePrefarbs;
    public TapText TapTextPrefab;

    public Transform CoinIcon;
    public Text GoldInfo;
    public Text AutoCollectInfo;

    private List<ResourceController> _activeResources = new List<ResourceController>();
    private List<TapText> _tapTextPool = new List<TapText>();
    private float _collectSecond;
    private float _saveDelayCounter;
    /*public double TotalGold { get; private set; }*/

    private void AddAllResources()
    {
        bool showResources = true;
        int index = 0;

        foreach(ResourceConfig config in ResourceConfigs)
        {
            GameObject obj = Instantiate(ResourcePrefarbs.gameObject, ResourceParent, false);
            ResourceController resource = obj.GetComponent<ResourceController>();

            resource.SetConfig(index, config);
            obj.gameObject.SetActive(showResources);
            if(showResources && !resource.isUnlocked)
            {
                showResources = false;
            }
            _activeResources.Add(resource);
            index++;
        }
    }

    public void AddGold(double value)

    {
        UserDataManager.Progress.gold += value;
        GoldInfo.text = $"Gold: { UserDataManager.Progress.gold.ToString("0") }";

        UserDataManager.Save(_saveDelayCounter < 0f);

        if(_saveDelayCounter < 0f)
        {
            _saveDelayCounter = SaveDelay;
        }
    }

    private void CollectPerSecond()
    {
        double output = 0;
        foreach(ResourceController resource in _activeResources)
        {
            if(resource.isUnlocked)
            {
                output += resource.GetOutput();
            }
        }

        output *= AutoCollectPercecntage;
        //ToString("F1") for 1 decimal place
        AutoCollectInfo.text = $"Auto Collect: { output.ToString("F1") } / second";
        AddGold(output);
    }

    private TapText GetOrCreateTapText()
    {
        TapText tapText = _tapTextPool.Find(t => !t.gameObject.activeSelf);
        if(tapText == null)
        {
            tapText = Instantiate(TapTextPrefab).GetComponent<TapText>();
            _tapTextPool.Add(tapText);
        }

        return tapText;
    }
    
    public void CollectByTap(Vector3 tapPosition, Transform parent)
    {
        double output = 0;
        foreach(ResourceController resource in _activeResources)
        {
            if(resource.isUnlocked)
            {
                output += resource.GetOutput();
            }
        }

        TapText tapText = GetOrCreateTapText();
        tapText.transform.SetParent(parent, false);
        tapText.transform.position = tapPosition;

        tapText.Text.text = $"+{ output.ToString("0") }";
        tapText.gameObject.SetActive(true);
        CoinIcon.transform.localScale = Vector3.one * 1.75f;

        AddGold(output);
    }

    private void CheckResourceCost()
    {
        foreach(ResourceController resource in _activeResources)
        {
            bool isBuyable = false;
            if(resource.isUnlocked)
            {
                isBuyable = UserDataManager.Progress.gold >= resource.GetUpgradeCost();
            }
            else
            {
                isBuyable = UserDataManager.Progress.gold >= resource.GetUnlockCost();
            }
            resource.ResourceImage.sprite = ResourceSprites[isBuyable ? 1 : 0];
        }
    }

    public void ShowNextResourcec()
    {
        foreach(ResourceController resource in _activeResources)
        {
            if(!resource.gameObject.activeSelf)
            {
                resource.gameObject.SetActive(true);
                break;
            }
        }
    }

    private void Start()
    {
        AddAllResources();
        GoldInfo.text = $"Gold: { UserDataManager.Progress.gold.ToString("0") }";
    }

    private void Update()
    {
        float deltaTime = Time.unscaledDeltaTime;
        _saveDelayCounter -= deltaTime;

        _collectSecond += deltaTime;
        if(_collectSecond >= 1f)
        {
            CollectPerSecond();
            _collectSecond = 0f;
        }

        CheckResourceCost();

        CoinIcon.transform.localScale = Vector3.LerpUnclamped(CoinIcon.transform.localScale, Vector3.one * 2f, 0.15f);
        CoinIcon.transform.Rotate(0f, 0f, Time.deltaTime * -100f);
    }
}
//Serializable so it can be serialize and value can be set from inspector
[System.Serializable]
public struct ResourceConfig
{
    public string Name;
    public double UnlockCost;
    public double UpgradedCost;
    public double Output;
}
