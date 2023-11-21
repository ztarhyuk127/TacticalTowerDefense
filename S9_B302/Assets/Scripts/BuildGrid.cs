using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildGrid : MonoBehaviour
{
    public enum BuildType { Tower, Deck };
    public bool canBuild = true;
    public BuildType type;
    public GameObject nowBuild;
}
