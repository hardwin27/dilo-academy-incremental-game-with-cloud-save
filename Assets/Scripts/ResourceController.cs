using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceController : MonoBehaviour
{
    public Button ResourceButton;
    public Image ResourceImage;
    public Text ResourceDescription;
    public Text ResourceUpgradeCost;
    public Text ResourceUnlockCost;

    private ResourceConfig _config;

    private int _index;
    private int _level
    {
        set
        {
            //value: value the have been set to the _level variable
            UserDataManager.Progress.ResourcesLevels[_index] = value;
            UserDataManager.Save(true);
        }
        get
        {
            if(!UserDataManager.HasResources(_index))
            {
                //if level not exsist, return level 1
                return 1;
            }

            //if exist, return that level
            return UserDataManager.Progress.ResourcesLevels[_index];
        }
    }

    public bool isUnlocked { get; private set; }

    public double GetOutput()
    {
        return _config.Output * _level;
    }

    public double GetUnlockCost()

    {
        return _config.UnlockCost;
    }

    public double GetUpgradeCost()
    {
        return _config.UpgradedCost * _level;
    }

    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;

        if (unlocked)
        { 
            //If new resource unlocked but not on the Progress Data, add to data
            if(!UserDataManager.HasResources(_index))
            {
                UserDataManager.Progress.ResourcesLevels.Add(_level);
                UserDataManager.Save(true);
            }
        }

        ResourceImage.color = isUnlocked ? Color.white : Color.grey;
        ResourceUnlockCost.gameObject.SetActive(!unlocked);
        ResourceUpgradeCost.gameObject.SetActive(unlocked);
    }

    public void UnlockResource()
    {
        double unlockCost = GetUnlockCost();
        if (UserDataManager.Progress.gold < unlockCost)
        {
            return;
        }
        SetUnlocked(true);
        GameManager.Instance.ShowNextResourcec();
        AchievementController.Instance.UnlockAchievement(AchievementType.UnlockResource, _config.Name);

        AnalyticsManager.LogUnlockEvent(_index);
    }

    public void SetConfig(int index, ResourceConfig config)
    {
        _index = index;
        _config = config;

        //ToString("0") makes no decimal
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput().ToString("0") }";
        ResourceUnlockCost.text = $"Unlock Cost\n{ _config.UnlockCost }";
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost() }";

        SetUnlocked(_config.UnlockCost == 0 || UserDataManager.HasResources(_index));
    }

    public void UpgradeLevel()
    {
        double upgradeCost = GetUpgradeCost();
        if(UserDataManager.Progress.gold < upgradeCost)
        {
            return;
        }

        GameManager.Instance.AddGold(-upgradeCost);
        _level++;
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost() }";
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput().ToString("0") }";

        AnalyticsManager.LogUpgradeEvent(_index, _level);
    }

    private void Start()
    {
        ResourceButton.onClick.AddListener(() =>
        {
            if(isUnlocked)
            {
                UpgradeLevel();
            }
            else
            {
                UnlockResource();
            }
        });
    }
}

