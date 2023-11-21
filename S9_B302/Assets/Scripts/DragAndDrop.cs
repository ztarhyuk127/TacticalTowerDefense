using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    public bool isDragging = false;
    private Vector3 offset;
    private Vector3 nowTransform;

    public GameObject prebuildTarget;
    private bool isBuildGrid = false;
    public GameObject nowbuildTarget;

    Tower tower;

    void Awake()
    {
        tower = GetComponent<Tower>();
    }

    private void OnMouseDown()
    {
        if (PlayManager.instance.isPauseGame) return;

        isDragging = true;
        nowTransform = transform.position;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        PlayManager.instance.BuildGridSet(0.5f);
        PlayManager.towerRadiusObject.transform.localScale = 2 * tower.fireRadius * Vector3.one;
        PlayManager.towerRadiusObject.transform.position = nowTransform;
    }

    private void OnMouseUp()
    {
        if (PlayManager.instance.isPauseGame) return;

        PlayManager.instance.BuildGridSet(0);
        PlayManager.towerRadiusObject.transform.localScale = Vector3.zero;

        // 타워 판매할 떄
        if (nowbuildTarget != null && nowbuildTarget.CompareTag("Trash") && tower.property != PlayManager.Property.Special)
        {
            // 타워 랭크에 따른 판매가격 조정
            if (tower.rank > 0)
            {
                PlayManager.instance.gold += (int)Mathf.Round(tower.towerCost * 0.75f);
            }
            else
            {
                PlayManager.instance.gold += tower.towerCost;
            }
            // 전체 타워 리스트에서 타워 제거
            PlayManager.instance.towerList.Remove(gameObject);

            // 현재 타워가 점유한 위치의 그리드에서 타워 제거
            if (prebuildTarget != null)
            {
                BuildGrid prebuildGrid = prebuildTarget.GetComponent<BuildGrid>();
                prebuildGrid.canBuild = true;
            }

            // 타워가 보유한 아이템을 인벤토리로 옮김
            PlayManager.instance.MoveItemToInventory(tower);

            // 타워의 시너지 효과 제거
            if (tower.isTower)
            {
                SynergyManager.instance.SynergyCheck();
                SynergyManager.instance.propertySynergy((int)tower.property, false);
                SynergyManager.instance.roleSynergy((int)tower.role, false);
            }

            Destroy(gameObject);
            return;
        }

        if (Mathf.Abs(Vector3.Distance(transform.position, nowTransform)) < 1)
        {
            PlayManager.instance.OpenTowerInfoUI(tower);
        }

        isDragging = false;
        if (isBuildGrid)
        {
            BuildGrid buildGrid = nowbuildTarget.GetComponent<BuildGrid>();
            if (!buildGrid.canBuild && buildGrid.nowBuild != gameObject)
            {
                DragAndDrop otherDragTower =  buildGrid.nowBuild.GetComponent<DragAndDrop>();
                Tower otherTower = buildGrid.nowBuild.GetComponent<Tower>();
                otherDragTower.transform.position = new Vector3(prebuildTarget.transform.position.x, prebuildTarget.transform.position.y, transform.position.z); ;

                buildGrid.nowBuild = gameObject;
                prebuildTarget.GetComponent<BuildGrid>().nowBuild = otherDragTower.gameObject;
                
                GameObject tmpObject = otherDragTower.prebuildTarget;
                otherDragTower.prebuildTarget = prebuildTarget;
                prebuildTarget = tmpObject;


                if (otherTower.isTower && !tower.isTower)
                {
                    otherTower.isTower = false;
                    tower.isTower = true;

                    SynergyManager.instance.SynergyCheck();

                    SynergyManager.instance.propertySynergy((int)otherTower.property, false);
                    SynergyManager.instance.roleSynergy((int)otherTower.role, false);

                    if (SynergyManager.instance.towerPropertyCount[tower.towerCost - 1][tower.towerNum - 1] == 1)
                    {
                        SynergyManager.instance.propertySynergy((int)tower.property, true);
                    }

                    if (SynergyManager.instance.towerRoleCount[tower.towerCost - 1][tower.towerNum - 1] == 1)
                    {
                        SynergyManager.instance.roleSynergy((int)tower.role, true);
                    }
                }
                else if (!otherTower.isTower && tower.isTower)
                {
                    otherTower.isTower = true;
                    tower.isTower = false;

                    SynergyManager.instance.SynergyCheck();

                    SynergyManager.instance.propertySynergy((int)tower.property, false);
                    SynergyManager.instance.roleSynergy((int)tower.role, false);

                    if (SynergyManager.instance.towerPropertyCount[otherTower.towerCost - 1][otherTower.towerNum - 1] == 1)
                    {
                        SynergyManager.instance.propertySynergy((int)otherTower.property, true);
                    }

                    if (SynergyManager.instance.towerRoleCount[otherTower.towerCost - 1][otherTower.towerNum - 1] == 1)
                    {
                        SynergyManager.instance.roleSynergy((int)otherTower.role, true);
                    }
                }
            }
            else if (buildGrid.canBuild)
            {
                buildGrid.canBuild = false;
                buildGrid.nowBuild = gameObject;

                if (buildGrid.type == BuildGrid.BuildType.Tower)
                {
                    tower.isTower = true;
                    SynergyManager.instance.SynergyCheck();
                    if (SynergyManager.instance.towerPropertyCount[tower.towerCost - 1][tower.towerNum - 1] == 1)
                    {
                        SynergyManager.instance.propertySynergy((int)tower.property, true);
                    }

                    if (SynergyManager.instance.towerRoleCount[tower.towerCost - 1][tower.towerNum - 1] == 1)
                    {
                        SynergyManager.instance.roleSynergy((int)tower.role, true);
                    }
                }
                else
                {
                    tower.isTower = false;
                    SynergyManager.instance.SynergyCheck();
                    SynergyManager.instance.propertySynergy((int)tower.property, false);
                    SynergyManager.instance.roleSynergy((int)tower.role, false);
                }

                if (prebuildTarget != null)
                {
                    BuildGrid prebuildGrid = prebuildTarget.GetComponent<BuildGrid>();
                    prebuildGrid.canBuild = true;
                }
            }
            transform.position = new Vector3(nowbuildTarget.transform.position.x, nowbuildTarget.transform.position.y, transform.position.z); ;
            prebuildTarget = nowbuildTarget;
        }
        else
        {
            if (prebuildTarget != null)
            {
                transform.position = prebuildTarget.transform.position;
            }
            else
            {
                transform.position = nowTransform;
            }
        }
    }

    private void Update()
    {
        if (PlayManager.instance.isGameOver || PlayManager.instance.isPauseGame)
        {
            return;
        }

        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, transform.position.z);
            
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero);
            bool haveCanBuild = false;

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.CompareTag("CanBuild"))
                {
                    haveCanBuild = true;

                    BuildGrid buildGrid = hit.collider.GetComponent<BuildGrid>();
                    if (buildGrid != null)
                    {
                        nowbuildTarget = hit.collider.gameObject;
                        isBuildGrid = true;
                    }
                }
                else if (hit.collider.gameObject.CompareTag("Deck"))
                {
                    haveCanBuild = true;

                    BuildGrid buildGrid = hit.collider.GetComponent<BuildGrid>();
                    if (buildGrid != null)
                    {
                        nowbuildTarget = hit.collider.gameObject;
                        isBuildGrid = true;
                    }
                }
                else if (hit.collider.gameObject.CompareTag("Trash"))
                {
                    haveCanBuild = true;
                    nowbuildTarget = hit.collider.gameObject;
                }

                if (!haveCanBuild)
                {
                    nowbuildTarget = null;
                    isBuildGrid = false;
                }
            }

            PlayManager.towerRadiusObject.transform.position = transform.position;
        }
    }

}
