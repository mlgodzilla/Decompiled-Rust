// Decompiled with JetBrains decompiler
// Type: Facepunch.Unity.RenderInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Facepunch.Unity
{
  public static class RenderInfo
  {
    public static void GenerateReport()
    {
      M0[] objectsOfType = Object.FindObjectsOfType<Renderer>();
      List<RenderInfo.RendererInstance> rendererInstanceList = new List<RenderInfo.RendererInstance>();
      foreach (Renderer renderer in (Renderer[]) objectsOfType)
      {
        RenderInfo.RendererInstance rendererInstance = RenderInfo.RendererInstance.From(renderer);
        rendererInstanceList.Add(rendererInstance);
      }
      string path = string.Format(Application.get_dataPath() + "/../RenderInfo-{0:yyyy-MM-dd_hh-mm-ss-tt}.txt", (object) DateTime.Now);
      string contents = JsonConvert.SerializeObject((object) rendererInstanceList, (Formatting) 1);
      File.WriteAllText(path, contents);
      string fileName = Application.get_streamingAssetsPath() + "/RenderInfo.exe";
      string arguments = "\"" + path + "\"";
      Debug.Log((object) ("Launching " + fileName + " " + arguments));
      Process.Start(fileName, arguments);
    }

    public struct RendererInstance
    {
      public bool IsVisible;
      public bool CastShadows;
      public bool Enabled;
      public bool RecieveShadows;
      public float Size;
      public float Distance;
      public int BoneCount;
      public int MaterialCount;
      public int VertexCount;
      public int TriangleCount;
      public int SubMeshCount;
      public int BlendShapeCount;
      public string RenderType;
      public string MeshName;
      public string ObjectName;
      public string EntityName;
      public uint EntityId;
      public bool UpdateWhenOffscreen;
      public int ParticleCount;

      public static RenderInfo.RendererInstance From(Renderer renderer)
      {
        RenderInfo.RendererInstance rendererInstance = new RenderInfo.RendererInstance();
        rendererInstance.IsVisible = renderer.get_isVisible();
        rendererInstance.CastShadows = renderer.get_shadowCastingMode() > 0;
        rendererInstance.RecieveShadows = renderer.get_receiveShadows();
        rendererInstance.Enabled = renderer.get_enabled() && ((Component) renderer).get_gameObject().get_activeInHierarchy();
        ref RenderInfo.RendererInstance local1 = ref rendererInstance;
        Bounds bounds = renderer.get_bounds();
        Vector3 size = ((Bounds) ref bounds).get_size();
        double magnitude = (double) ((Vector3) ref size).get_magnitude();
        local1.Size = (float) magnitude;
        ref RenderInfo.RendererInstance local2 = ref rendererInstance;
        bounds = renderer.get_bounds();
        double num = (double) Vector3.Distance(((Bounds) ref bounds).get_center(), ((Component) Camera.get_main()).get_transform().get_position());
        local2.Distance = (float) num;
        rendererInstance.MaterialCount = renderer.get_sharedMaterials().Length;
        rendererInstance.RenderType = ((object) renderer).GetType().Name;
        BaseEntity baseEntity = ((Component) renderer).get_gameObject().ToBaseEntity();
        if (Object.op_Implicit((Object) baseEntity))
        {
          rendererInstance.EntityName = baseEntity.PrefabName;
          if (baseEntity.net != null)
            rendererInstance.EntityId = (uint) baseEntity.net.ID;
        }
        else
          rendererInstance.ObjectName = ((Component) renderer).get_transform().GetRecursiveName("");
        if (renderer is MeshRenderer)
        {
          rendererInstance.BoneCount = 0;
          MeshFilter component = (MeshFilter) ((Component) renderer).GetComponent<MeshFilter>();
          if (Object.op_Implicit((Object) component))
            rendererInstance.ReadMesh(component.get_sharedMesh());
        }
        if (renderer is SkinnedMeshRenderer)
        {
          SkinnedMeshRenderer skinnedMeshRenderer = renderer as SkinnedMeshRenderer;
          rendererInstance.ReadMesh(skinnedMeshRenderer.get_sharedMesh());
          rendererInstance.UpdateWhenOffscreen = skinnedMeshRenderer.get_updateWhenOffscreen();
        }
        if (renderer is ParticleSystemRenderer)
        {
          ParticleSystem component = (ParticleSystem) ((Component) renderer).GetComponent<ParticleSystem>();
          if (Object.op_Implicit((Object) component))
          {
            rendererInstance.MeshName = ((Object) component).get_name();
            rendererInstance.ParticleCount = component.get_particleCount();
          }
        }
        return rendererInstance;
      }

      public void ReadMesh(Mesh mesh)
      {
        if (Object.op_Equality((Object) mesh, (Object) null))
        {
          this.MeshName = "<NULL>";
        }
        else
        {
          this.VertexCount = mesh.get_vertexCount();
          this.SubMeshCount = mesh.get_subMeshCount();
          this.BlendShapeCount = mesh.get_blendShapeCount();
          this.MeshName = ((Object) mesh).get_name();
        }
      }
    }
  }
}
