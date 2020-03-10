using System.Collections.Generic;
using UnityEngine;

public class ClothPoints : MonoBehaviour
{
    public Transform LeftScarfIK, RightScarfIK;
    public int ball1, ball2;
    public Transform LeftScarfBase, RightScarfBase;
    Cloth cloth;
    int[] LeftClothIndices = {15, 20, 25, 30};
    int[] RightClothIndices = {131, 125, 120, 115, 110, 105, 100, 95, 90, 83, 76};
    List<Transform> LeftScarfBones, RightScarfBones;
    // Start is called before the first frame update
    void Start()
    {
        cloth = GetComponent<Cloth>();
        LeftScarfBones = new List<Transform>();
        RightScarfBones = new List<Transform>();
        Transform temp = LeftScarfBase;
        while (temp.childCount != 0)
        {
            LeftScarfBones.Add(temp.GetChild(0));
            temp = temp.GetChild(0);
        }
        temp = RightScarfBase;
        while (temp.childCount != 0)
        {
            RightScarfBones.Add(temp.GetChild(0));
            temp = temp.GetChild(0);
        }

    }

    // Update is called once per frame
    void Update()
    {
        // LeftScarfIK.position = transform.TransformPoint(cloth.vertices[61]); //4
        // RightScarfIK.position = transform.TransformPoint(cloth.vertices[95]); //140
        for(int i = 0; i < 8; ++i)
        {
            LeftScarfBones[i].position = transform.TransformPoint(cloth.vertices[15 + 5 * i]);
            RightScarfBones[i].position = transform.TransformPoint(cloth.vertices[RightClothIndices[i]]);
        }
    }

    void OnDrawGizmos()
    { //4 and
        for(int i = 0; i < cloth.vertices.Length; ++i)
        {
            Gizmos.color = Color.red;
            if (i == ball1 || i == ball2)
                Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.TransformPoint(cloth.vertices[i]), 0.07f);
        }
        // foreach(Transform t in LeftScarfBones)
        //     Gizmos.DrawSphere(t.position, 0.07f);
        // foreach(Transform t in RightScarfBones)
        //     Gizmos.DrawSphere(t.position, 0.07f);
    }
}
