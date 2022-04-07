using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DecalOffset : MonoBehaviour {

    public RenderTexture rtProbeOffset;

    //如果修改场景 或光照 需要在 不播放时 执行这个菜单 会创建探针 烘焙 并创建3dtexture
    [ContextMenu("createProbeAndBake")]
    void createProbeAndBake()
    {
        Utils.createProbeAndTexure(gameObject, true, true);
    }
        // Use this for initialization
        void Start () {
        createProbeAndBake();

         CommandBuffer cmd = new CommandBuffer();
        cmd.name = "DecalOffset";


        rtProbeOffset = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);

        Shader.SetGlobalTexture("rtProbeOffset", rtProbeOffset);


     

        cmd.SetRenderTarget(rtProbeOffset, BuiltinRenderTextureType.CameraTarget); // 设置渲染目标
        cmd.ClearRenderTarget(false, true, Color.black);

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            Vector3 dir;

            var pos =r.transform.position;
            r.enabled = false;
            if (calOffsetDir(pos, out dir))
            {
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                r.GetPropertyBlock(mpb);
                mpb.SetVector("offsetDir", dir);
                r.SetPropertyBlock(mpb);
                cmd.DrawRenderer(r, r.sharedMaterial); // 绘制指令
            }
        }

       
        Camera.main.AddCommandBuffer(CameraEvent.AfterGBuffer, cmd);
    }
    bool calOffsetDir(Vector3 pos,out Vector3 dir) {
          dir = Vector3.zero;
        for (int i = 0; i < 10000; i++)
        {
            var ray = Random.onUnitSphere;
            if (Physics.Raycast(pos, ray, 2f))
            {
                dir += -ray;
            }
        }
        if (dir.magnitude > 0.5f)
        {
            dir.Normalize();
            return true;
        }
        dir = Vector3.zero;
        return false;
           
    }
	// Update is called once per frame
	void Update () {
        Shader.SetGlobalMatrix("mainVP", GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false) * Camera.main.worldToCameraMatrix);
	}
	void OnDrawGizmos() {
        Vector3 dir;

        var pos = transform.position;

        if (calOffsetDir(pos, out dir)) {
            Gizmos.DrawRay(pos, dir);
        }

    }
}
