using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipPositions : MonoBehaviour
{
    public Vector3 Helmet;
    public Vector3 Body;
    public Vector3 Boots;
    public Vector3 Ring;
    public Vector3 Amulet;
    public Vector3 Weapon;
    public Vector3 OffHand;

    void Start() {
        Helmet = new Vector3(0.0017f, 0.1207f, 0);
        Body = new Vector3(0.00128f, 0.00141f, 0);
        Boots = new Vector3(0.0017f, -0.1174f, 0);
        Ring = new Vector3(-0.116f, 0.1207f, 0);
        Amulet = new Vector3(0.119f, 0.1204f, 0);
        Weapon = new Vector3(-0.1163f, 0.0017f, 0);
        OffHand = new Vector3(0.119f, 0.0017f, 0);
    }

    void Update() {

    }
}
