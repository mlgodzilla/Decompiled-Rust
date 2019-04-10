// Decompiled with JetBrains decompiler
// Type: CoverageQueries
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof (Camera))]
[RequireComponent(typeof (Camera))]
[RequireComponent(typeof (Camera))]
public class CoverageQueries : MonoBehaviour
{
  public float depthBias;
  public bool debug;

  public CoverageQueries()
  {
    base.\u002Ector();
  }

  public class BufferSet
  {
    public Color[] inputData = new Color[0];
    public Color32[] resultData = new Color32[0];
    private Queue<AsyncGPUReadbackRequest> asyncRequests = new Queue<AsyncGPUReadbackRequest>();
    public int width;
    public int height;
    public Texture2D inputTexture;
    public RenderTexture resultTexture;
    private Material coverageMat;
    private const int MaxAsyncGPUReadbackRequests = 10;

    public void Attach(Material coverageMat)
    {
      this.coverageMat = coverageMat;
    }

    public void Dispose(bool data = true)
    {
      if (Object.op_Inequality((Object) this.inputTexture, (Object) null))
      {
        Object.DestroyImmediate((Object) this.inputTexture);
        this.inputTexture = (Texture2D) null;
      }
      if (Object.op_Inequality((Object) this.resultTexture, (Object) null))
      {
        RenderTexture.set_active((RenderTexture) null);
        Object.DestroyImmediate((Object) this.resultTexture);
        this.resultTexture = (RenderTexture) null;
      }
      if (!data)
        return;
      this.inputData = new Color[0];
      this.resultData = new Color32[0];
    }

    public bool CheckResize(int count)
    {
      if (count <= this.inputData.Length && (!Object.op_Inequality((Object) this.resultTexture, (Object) null) || this.resultTexture.IsCreated()))
        return false;
      this.Dispose(false);
      this.width = Mathf.CeilToInt(Mathf.Sqrt((float) count));
      this.height = Mathf.CeilToInt((float) count / (float) this.width);
      this.inputTexture = new Texture2D(this.width, this.height, (TextureFormat) 20, false, true);
      ((Object) this.inputTexture).set_name("_Input");
      ((Texture) this.inputTexture).set_filterMode((FilterMode) 0);
      ((Texture) this.inputTexture).set_wrapMode((TextureWrapMode) 1);
      this.resultTexture = new RenderTexture(this.width, this.height, 0, (RenderTextureFormat) 0, (RenderTextureReadWrite) 1);
      ((Object) this.resultTexture).set_name("_Result");
      ((Texture) this.resultTexture).set_filterMode((FilterMode) 0);
      ((Texture) this.resultTexture).set_wrapMode((TextureWrapMode) 1);
      this.resultTexture.set_useMipMap(false);
      this.resultTexture.Create();
      int length = this.resultData.Length;
      int newSize = this.width * this.height;
      Array.Resize<Color>(ref this.inputData, newSize);
      Array.Resize<Color32>(ref this.resultData, newSize);
      Color32 color32;
      ((Color32) ref color32).\u002Ector(byte.MaxValue, (byte) 0, (byte) 0, (byte) 0);
      for (int index = length; index < newSize; ++index)
        this.resultData[index] = color32;
      return true;
    }

    public void UploadData()
    {
      if (this.inputData.Length == 0)
        return;
      this.inputTexture.SetPixels(this.inputData);
      this.inputTexture.Apply();
    }

    public void Dispatch(int count)
    {
      if (this.inputData.Length == 0)
        return;
      RenderBuffer activeColorBuffer = Graphics.get_activeColorBuffer();
      RenderBuffer activeDepthBuffer = Graphics.get_activeDepthBuffer();
      this.coverageMat.SetTexture("_Input", (Texture) this.inputTexture);
      Graphics.Blit((Texture) this.inputTexture, this.resultTexture, this.coverageMat, 0);
      RenderBuffer renderBuffer = activeDepthBuffer;
      Graphics.SetRenderTarget(activeColorBuffer, renderBuffer);
    }

    public void IssueRead()
    {
      if (this.asyncRequests.Count >= 10)
        return;
      this.asyncRequests.Enqueue(AsyncGPUReadback.Request((Texture) this.resultTexture, 0, (Action<AsyncGPUReadbackRequest>) null));
    }

    public void GetResults()
    {
      if (this.resultData.Length == 0)
        return;
      while (this.asyncRequests.Count > 0)
      {
        AsyncGPUReadbackRequest gpuReadbackRequest = this.asyncRequests.Peek();
        if (((AsyncGPUReadbackRequest) ref gpuReadbackRequest).get_hasError())
        {
          this.asyncRequests.Dequeue();
        }
        else
        {
          if (!((AsyncGPUReadbackRequest) ref gpuReadbackRequest).get_done())
            break;
          NativeArray<Color32> data = (NativeArray<Color32>) ((AsyncGPUReadbackRequest) ref gpuReadbackRequest).GetData<Color32>(0);
          for (int index = 0; index < ((NativeArray<Color32>) ref data).get_Length(); ++index)
            this.resultData[index] = ((NativeArray<Color32>) ref data).get_Item(index);
          this.asyncRequests.Dequeue();
        }
      }
    }
  }

  public enum RadiusSpace
  {
    ScreenNormalized,
    World,
  }

  public class Query
  {
    public CoverageQueries.Query.Input input;
    public CoverageQueries.Query.Internal intern;
    public CoverageQueries.Query.Result result;

    public bool IsRegistered
    {
      get
      {
        return this.intern.id >= 0;
      }
    }

    public struct Input
    {
      public Vector3 position;
      public CoverageQueries.RadiusSpace radiusSpace;
      public float radius;
      public int sampleCount;
      public float smoothingSpeed;
    }

    public struct Internal
    {
      public int id;

      public void Reset()
      {
        this.id = -1;
      }
    }

    public struct Result
    {
      public int passed;
      public float coverage;
      public float smoothCoverage;
      public float weightedCoverage;
      public float weightedSmoothCoverage;
      public bool originOccluded;
      public int frame;
      public float originVisibility;
      public float originSmoothVisibility;

      public void Reset()
      {
        this.passed = 0;
        this.coverage = 1f;
        this.smoothCoverage = 1f;
        this.weightedCoverage = 1f;
        this.weightedSmoothCoverage = 1f;
        this.originOccluded = false;
        this.frame = -1;
        this.originVisibility = 0.0f;
        this.originSmoothVisibility = 0.0f;
      }
    }
  }
}
