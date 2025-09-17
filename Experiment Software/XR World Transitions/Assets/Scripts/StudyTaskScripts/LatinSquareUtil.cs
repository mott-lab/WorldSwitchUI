using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Condition {
    public int ID;
    public TransitionUIType UIType;
}

public class LatinSquareUtil : MonoBehaviour
{

    public List<List<int>> ConditionsLatinSquare = new List<List<int>>();
    public List<(int, TransitionUIType)> ConditionOrder = new List<(int, TransitionUIType)>();
    public List<Condition> Conditions = new List<Condition>();
    private List<int> ConditionOrderList = new List<int>();

    public void InitConditionsLatinSquare()
    {
        ConditionsLatinSquare.Add(new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        ConditionsLatinSquare.Add(new List<int> { 1, 2, 9, 3, 8, 4, 7, 5, 6 });
        ConditionsLatinSquare.Add(new List<int> { 7, 6, 8, 5, 9, 4, 1, 3, 2 });
        ConditionsLatinSquare.Add(new List<int> { 3, 4, 2, 5, 1, 6, 9, 7, 8 });
        ConditionsLatinSquare.Add(new List<int> { 9, 8, 1, 7, 2, 6, 3, 5, 4 });
        ConditionsLatinSquare.Add(new List<int> { 5, 6, 4, 7, 3, 8, 2, 9, 1 });
        ConditionsLatinSquare.Add(new List<int> { 2, 1, 3, 9, 4, 8, 5, 7, 6 });
        ConditionsLatinSquare.Add(new List<int> { 7, 8, 6, 9, 5, 1, 4, 2, 3 });
        ConditionsLatinSquare.Add(new List<int> { 4, 3, 5, 2, 6, 1, 7, 9, 8 });
        ConditionsLatinSquare.Add(new List<int> { 9, 1, 8, 2, 7, 3, 6, 4, 5 });
        ConditionsLatinSquare.Add(new List<int> { 6, 5, 7, 4, 8, 3, 9, 2, 1 });
        ConditionsLatinSquare.Add(new List<int> { 2, 3, 1, 4, 9, 5, 8, 6, 7 });
        ConditionsLatinSquare.Add(new List<int> { 8, 7, 9, 6, 1, 5, 2, 4, 3 });
        ConditionsLatinSquare.Add(new List<int> { 4, 5, 3, 6, 2, 7, 1, 8, 9 });
        ConditionsLatinSquare.Add(new List<int> { 1, 9, 2, 8, 3, 7, 4, 6, 5 });
        ConditionsLatinSquare.Add(new List<int> { 6, 7, 5, 8, 4, 9, 3, 1, 2 });
        ConditionsLatinSquare.Add(new List<int> { 3, 2, 4, 1, 5, 9, 6, 8, 7 });
        ConditionsLatinSquare.Add(new List<int> { 8, 9, 7, 1, 6, 2, 5, 3, 4 });
        ConditionsLatinSquare.Add(new List<int> { 5, 4, 6, 3, 7, 2, 8, 1, 9 });
    }

    void Awake()
    {
    }

    public void SetConditionOrderForPID(int pid)
    {
        
        ConditionOrderList = ConditionsLatinSquare[pid % ConditionsLatinSquare.Count];
        Conditions.Add(new Condition { ID = ConditionOrderList[0], UIType = (TransitionUIType)ConditionOrderList[0] });
        Conditions.Add(new Condition { ID = ConditionOrderList[1], UIType = (TransitionUIType)ConditionOrderList[1] });
        Conditions.Add(new Condition { ID = ConditionOrderList[2], UIType = (TransitionUIType)ConditionOrderList[2] });
        Conditions.Add(new Condition { ID = ConditionOrderList[3], UIType = (TransitionUIType)ConditionOrderList[3] });
        Conditions.Add(new Condition { ID = ConditionOrderList[4], UIType = (TransitionUIType)ConditionOrderList[4] });
        Conditions.Add(new Condition { ID = ConditionOrderList[5], UIType = (TransitionUIType)ConditionOrderList[5] });
        Conditions.Add(new Condition { ID = ConditionOrderList[6], UIType = (TransitionUIType)ConditionOrderList[6] });
        Conditions.Add(new Condition { ID = ConditionOrderList[7], UIType = (TransitionUIType)ConditionOrderList[7] });
        Conditions.Add(new Condition { ID = ConditionOrderList[8], UIType = (TransitionUIType)ConditionOrderList[8] });
        // ConditionOrder = new List<(int, TransitionUIType)> {
        //     (ConditionOrderList[0], (TransitionUIType)ConditionOrderList[0]),
        //     (ConditionOrderList[1], (TransitionUIType)ConditionOrderList[1]),
        //     (ConditionOrderList[2], (TransitionUIType)ConditionOrderList[2]),
        //     (ConditionOrderList[3], (TransitionUIType)ConditionOrderList[3]),
        //     (ConditionOrderList[4], (TransitionUIType)ConditionOrderList[4]),
        //     (ConditionOrderList[5], (TransitionUIType)ConditionOrderList[5]),
        //     (ConditionOrderList[6], (TransitionUIType)ConditionOrderList[6]),
        //     (ConditionOrderList[7], (TransitionUIType)ConditionOrderList[7]),
        //     (ConditionOrderList[8], (TransitionUIType)ConditionOrderList[8])
        // };
    }
}
