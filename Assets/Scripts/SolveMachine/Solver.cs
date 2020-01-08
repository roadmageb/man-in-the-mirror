using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Solver
{
    /// <summary>
    /// 해의 집합
    /// </summary>
    public List<List<string>> solutions;

    public List<List<string>> GetSolution(SolveMap map)
    {
        solutions = new List<List<string>>();
        return solutions;
    }
    void Recursive(List<string> solution, SolveMap map)
    {
        if(CheckClear(map))
        {
            solutions.Add(solution);
            return;
        }
        SolveMap originMap = new SolveMap(map);
        //move
        foreach(KeyValuePair<Vector2Int, bool> player in map.playerFloors)
        {
            if(player.Value)
            {

            }
        }
        //shoot

        //getBox
    }
    bool CheckClear(SolveMap map)
    {
        return true;
    }
}
