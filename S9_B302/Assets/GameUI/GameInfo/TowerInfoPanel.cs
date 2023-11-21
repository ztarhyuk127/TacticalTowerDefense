using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerInfoPanel : MonoBehaviour
{
    public Image towerImage;
    public TextMeshProUGUI towerName;
    public TextMeshProUGUI towerDescription;
    public Image towerProperty;
    public Image towerRole;
    public Image towerAttackType;
    public Image towerShotType;
    public TextMeshProUGUI towerCost;
    public TextMeshProUGUI towerDamage;
    public TextMeshProUGUI towerAttackSpeed;
    public TextMeshProUGUI towerCriticalRate;
    public TextMeshProUGUI towerCriticalDamage;
    public Inventory[] towerInventory;
    public GameObject[] rankStar;
}
