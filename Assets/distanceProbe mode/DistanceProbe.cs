using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceProbe : MonoBehaviour
{
    //如果修改场景 或光照 需要在 不播放时 执行这个菜单 会创建探针 烘焙 并创建3dtexture
    [ContextMenu("createProbeAndBake")]
    void createProbeAndBake()
    {
        Utils.createProbeAndTexure(gameObject,false,false);
        var VolCount = Utils.VolCount;
        var VolDensity = Utils.VolDensity;
        var volTex = new Texture3D(VolCount, VolCount, VolCount, TextureFormat.RGBAFloat, false);
        volTex.wrapMode = TextureWrapMode.Clamp;
        volTex.filterMode = FilterMode.Point;
        var volColors = volTex.GetPixels();
        for (int x = 0; x < VolCount; x++)
        {
            for (int y = 0; y < VolCount; y++)
            {
                for (int z = 0; z < VolCount; z++)
                {
                    var pos = new Vector3(x, y, z) / VolDensity ;
                    Vector3 fixedPos;
                    var offset = getOffset(pos, out fixedPos);
                   
                 
                    volColors[z * VolCount * VolCount + y * VolCount + x] = new Color(offset.x, offset.y, offset.z);

                }

            }
        }
        volTex.SetPixels(volColors);
        volTex.Apply();
        Shader.SetGlobalTexture("volOffsetTex", volTex);
    }


    // Use this for initialization
    void Start()
    {
        createProbeAndBake();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnDrawGizmos()
    {
        //Vector3 pos;
        //var offset = getOffset(transform.position,out pos);
        ////Gizmos.DrawSphere(pos, 1.0f / VoxPerUnit/2);
        //var VoxPerUnit = Utils.VolDensity;
        //Vector3 finalPos = transform.position * VoxPerUnit;
        //Vector3 fracPos = finalPos - pos * VoxPerUnit;
        //print(fracPos.x + ":::" + offset.x);
        //if (offset.x > 0) finalPos.x = fracPos.x - 0.5f > offset.x ? (int)finalPos.x + 1 : (int)finalPos.x;

        //if (offset.y > 0) finalPos.y = fracPos.y - 0.5f > offset.y ? (int)finalPos.y + 1 : (int)finalPos.y;

        //if (offset.z > 0) finalPos.z = fracPos.z - 0.5f > offset.z ? (int)finalPos.z + 1 : (int)finalPos.z;
        //Gizmos.color = Color.red;
        //if (offset.sqrMagnitude != 0)
        //{
        //    Gizmos.DrawWireSphere(finalPos / VoxPerUnit, 1.0f / VoxPerUnit / 2);
        //}
        //else {
        //    Gizmos.DrawWireSphere(pos, 1.0f / VoxPerUnit / 2);
        //    Gizmos.DrawWireSphere(pos+Vector3.right/ VoxPerUnit, 1.0f / VoxPerUnit / 2);
        //    Gizmos.DrawWireSphere(pos+Vector3.forward/ VoxPerUnit, 1.0f / VoxPerUnit / 2);
        //    Gizmos.DrawWireSphere(pos+(Vector3.forward+Vector3.right)/ VoxPerUnit, 1.0f / VoxPerUnit / 2);
        //}
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(transform.position, 1.0f / VoxPerUnit/2);

    }

    private Vector3 getOffset( Vector3 wpos,out Vector3 fixedWpos)
    {
        var VoxPerUnit = Utils.VolDensity;
        var offset = Vector3.zero;
        int x = (int)(wpos.x * VoxPerUnit);
        int y = (int)(wpos.y * VoxPerUnit);
        int z = (int)(wpos.z * VoxPerUnit);
        var pos= fixedWpos = new Vector3(x, y, z) / VoxPerUnit;
        RaycastHit hitInfo;

        if (Physics.Raycast(pos, Vector3.right, out hitInfo, 1.0f / VoxPerUnit))
        {
            offset.x = hitInfo.distance;
        }
        else if (Physics.Raycast(pos + Vector3.right / VoxPerUnit, -Vector3.right, out hitInfo, 1.0f / VoxPerUnit))
        {
            offset.x = 1.0f / VoxPerUnit - hitInfo.distance;
        }
        if (Physics.Raycast(pos, Vector3.up, out hitInfo, 1.0f / VoxPerUnit))
        {
            offset.y = hitInfo.distance;
        }
        else if (Physics.Raycast(pos + Vector3.up / VoxPerUnit, -Vector3.up, out hitInfo, 1.0f / VoxPerUnit))
        {
            offset.y = 1.0f / VoxPerUnit - hitInfo.distance;
        }
        if (Physics.Raycast(pos, Vector3.forward , out hitInfo, 1.0f / VoxPerUnit))
        {
            offset.z = hitInfo.distance;
           
        }
        else if (Physics.Raycast(pos + Vector3.forward / VoxPerUnit, -Vector3.forward, out hitInfo, 1.0f / VoxPerUnit))
        {
            offset.z = 1.0f / VoxPerUnit - hitInfo.distance;
           
        }
        return offset;
    }
}
