using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public List<TreeController> treeControllerList = new List<TreeController>();
    public List<MushroomController> mushroomControllerList = new List<MushroomController>();
    public List<Vector3> mushroomPositions;
    [Space, HideInInspector] public YarnController yarn;

    private void Awake()
    {
        mushroomPositions = new List<Vector3>();
        yarn = GameObject.FindGameObjectWithTag("Player").GetComponent<YarnController>();
        AwakeMushroomCheck();
    }

    private void AwakeMushroomCheck()
    {
        //mushroomControllerList.Add(
            //GameObject.FindGameObjectWithTag("Mushrooms").GetComponent<MushroomController>());

        foreach (var VARIABLE in mushroomControllerList)
        {
            mushroomPositions.Add(VARIABLE.transform.position - (Vector3.up));
        }
        yarn.SetNewYarnPosition(mushroomPositions[0]);
        yarn.positions = mushroomPositions;
    }
    public void AddToTreeControllerList(TreeController treeController)
    {
        if (treeControllerList.Contains(treeController)) return;
        
        treeControllerList.Add(treeController);
    }
    public void RemoveFromTreeControllerList(TreeController treeController)
    {
        if (!treeControllerList.Contains(treeController)) return;

        treeControllerList.Remove(treeController);
    }
    public void AddToMushroomControllerList(MushroomController mushroomController)
    {
        if (mushroomControllerList.Contains(mushroomController)) return;

        mushroomControllerList.Add(mushroomController);

        yarn.positions.Clear();

        foreach (var VARIABLE in mushroomControllerList)
        {
            mushroomPositions.Add(VARIABLE.transform.position - (VARIABLE.transform.up.normalized));
        }
    }
}
