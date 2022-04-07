using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BentNormal : MonoBehaviour {

	// Use this for initialization
	void Start() {
		createProbeAndBake();
		makeBentNormal();
	}
	//如果修改场景 或光照 需要在 不播放时 执行这个菜单 会创建探针 烘焙 并创建3dtexture
	[ContextMenu("createProbeAndBake")]
	void createProbeAndBake()
	{
		Utils.createProbeAndTexure(gameObject,true,true);

	}
	 
	 
	[ContextMenu("makeBentNormal")]
	void makeBentNormal() {
 
 	 
		foreach (var item in FindObjectsOfType<MeshFilter>())
		{
			//if (item.lightmapIndex < 0 || item.lightmapIndex >= 65535) continue;
			//
		 
			var vts = item.mesh.vertices;
			var normals = item.mesh.normals;
			var colors = new Color[vts.Length];
			for (int i = 0; i < vts.Length; i++)
			{
				var wpos = item.transform.TransformPoint(vts[i]);
				var wnormal = item.transform.TransformDirection(normals[i]);
				Vector3 bentNormal = getBentNormal(wpos, wnormal);

				colors[i] = (Vector4)(bentNormal);// * 0.5f + Vector3.one * 0.5f);

			}
			item.mesh.colors = colors;


		}
	}
	void OnDrawGizmos(){
		// 比较卡 电脑不好不要开 12900k+3090的请随意
        //foreach (var item in FindObjectsOfType<MeshFilter>())
        //{

        //    var vts = item.sharedMesh.vertices;
        //    var normals = item.sharedMesh.normals;

        //    for (int i = 0; i < vts.Length; i++)
        //    {
        //        var wpos = item.transform.TransformPoint(vts[i]);
        //        var wnormal = item.transform.TransformDirection(normals[i]);
        //        Vector3 bentNormal = getBentNormal(wpos, wnormal);
        //        Gizmos.color = (Vector4)(bentNormal * 0.5f + Vector3.one * 0.5f);
        //        Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 1);
        //        Gizmos.DrawRay(wpos, bentNormal * 0.5f);

        //    }



        //}
    }
    private Vector3 getBentNormal(Vector3 wpos, Vector3 wnormal)
    {
		Vector3 bentN = wnormal;
        for (int i = 0; i < 1000; i++)
        {
			var dir = UnityEngine.Random.onUnitSphere;
			if (Vector3.Dot(dir, wnormal) < 0) continue;
			 if (!Physics.Raycast(wpos + dir * 1.0f / Utils.VolDensity / 4, dir, 1.0f) && !Physics.Raycast(wpos + dir * 1.0f / Utils.VolDensity / 4 + dir, -dir, 1.0f)) {
			bentN += dir;
			}
        }
		return bentN.normalized;
    }
}


