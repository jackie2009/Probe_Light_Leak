using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils  {
	public	const int VolSize = 8;
	public const int VolDensity = 2; 
	public const int VolCount = VolSize * VolDensity;
	// Use this for initialization
	public static void createProbeAndTexure (GameObject gameObject,bool createProbesAuto,bool useCenterPos) {
        if (!Application.isPlaying&& createProbesAuto)
        {
            var lpg = GameObject.FindObjectOfType<LightProbeGroup>();

            if (lpg == null) lpg = new GameObject("LightProbeGroup").AddComponent<LightProbeGroup>();
            var posList = new Vector3[VolCount * VolCount * VolCount];
            for (int x = 0; x < VolCount; x++)
            {
                for (int y = 0; y < VolCount; y++)
                {
                    for (int z = 0; z < VolCount; z++)
                    {
                        posList[z * VolCount * VolCount + y * VolCount + x] = new Vector3(x, y, z) / VolDensity;// + Vector3.one * 0.5f / VolDensity;

                    }

                }
            }
            lpg.probePositions = posList;
            UnityEditor.Lightmapping.Bake();
        }
        var volTex = new Texture3D(VolCount, VolCount, VolCount, TextureFormat.RGB24, false);
		volTex.wrapMode = TextureWrapMode.Clamp;
		volTex.filterMode = FilterMode.Trilinear;
		var volColors = volTex.GetPixels();
		for (int x = 0; x < VolCount; x++)
		{
			for (int y = 0; y < VolCount; y++)
			{
				for (int z = 0; z < VolCount; z++)
				{
					var pos = new Vector3(x, y, z) / VolDensity + (useCenterPos?Vector3.one * 0.5f / VolDensity:Vector3.zero);
					UnityEngine.Rendering.SphericalHarmonicsL2 probe;
					LightProbes.GetInterpolatedProbe(pos, null, out probe);
					volColors[z * VolCount * VolCount + y * VolCount + x] = new Color(probe[0, 0], probe[1, 0], probe[2, 0]);

				}

			}
		}
		volTex.SetPixels(volColors);
		volTex.Apply();
		Shader.SetGlobalTexture("volTex", volTex);
		Shader.SetGlobalInt("VolDensity", VolDensity);
		Shader.SetGlobalInt("VolSize", VolSize);
	}
	
	 
}
