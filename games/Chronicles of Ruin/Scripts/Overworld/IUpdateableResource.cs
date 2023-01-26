using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdateableResource
{
    void UpdateResourceAmount(int amount);
    void UpdateText(string text);
}
