using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat", menuName = "ScriptableObjects/Stats/Stat")]
public class Stat : ScriptableObject
{
    [SerializeField]
    private int value;
    public int Value => value;

    public void SetStatValue(int value) {
        this.value = value;
    }
}
