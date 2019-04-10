// Decompiled with JetBrains decompiler
// Type: OcclusionCulling
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using RustNative;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof (Camera))]
[RequireComponent(typeof (Camera))]
public class OcclusionCulling : MonoBehaviour
{
  public ComputeShader computeShader;
  public bool usePixelShaderFallback;
  public bool useAsyncReadAPI;
  private Camera camera;
  private const int ComputeThreadsPerGroup = 64;
  private const int InputBufferStride = 16;
  private const int ResultBufferStride = 4;
  private const int OccludeeMaxSlotsPerPool = 1048576;
  private const int OccludeePoolGranularity = 2048;
  private const int StateBufferGranularity = 2048;
  private const int GridBufferGranularity = 256;
  private static Queue<OccludeeState> statePool;
  private static OcclusionCulling.SimpleList<OccludeeState> staticOccludees;
  private static OcclusionCulling.SimpleList<OccludeeState.State> staticStates;
  private static OcclusionCulling.SimpleList<int> staticVisibilityChanged;
  private static OcclusionCulling.SimpleList<OccludeeState> dynamicOccludees;
  private static OcclusionCulling.SimpleList<OccludeeState.State> dynamicStates;
  private static OcclusionCulling.SimpleList<int> dynamicVisibilityChanged;
  private static List<int> staticChanged;
  private static Queue<int> staticRecycled;
  private static List<int> dynamicChanged;
  private static Queue<int> dynamicRecycled;
  private static OcclusionCulling.BufferSet staticSet;
  private static OcclusionCulling.BufferSet dynamicSet;
  private static OcclusionCulling.BufferSet gridSet;
  private Vector4[] frustumPlanes;
  private string[] frustumPropNames;
  private float[] matrixToFloatTemp;
  private Material fallbackMat;
  private Material depthCopyMat;
  private Matrix4x4 viewMatrix;
  private Matrix4x4 projMatrix;
  private Matrix4x4 viewProjMatrix;
  private Matrix4x4 prevViewProjMatrix;
  private Matrix4x4 invViewProjMatrix;
  private bool useNativePath;
  private static OcclusionCulling instance;
  private static GraphicsDeviceType[] supportedDeviceTypes;
  private static bool _enabled;
  private static bool _safeMode;
  private static OcclusionCulling.DebugFilter _debugShow;
  public OcclusionCulling.DebugSettings debugSettings;
  private Material debugMipMat;
  private const float debugDrawDuration = 0.0334f;
  private Material downscaleMat;
  private Material blitCopyMat;
  private int hiZLevelCount;
  private int hiZWidth;
  private int hiZHeight;
  private RenderTexture depthTexture;
  private RenderTexture hiZTexture;
  private RenderTexture[] hiZLevels;
  private const int GridCellsPerAxis = 2097152;
  private const int GridHalfCellsPerAxis = 1048576;
  private const int GridMinHalfCellsPerAxis = -1048575;
  private const int GridMaxHalfCellsPerAxis = 1048575;
  private const float GridCellSize = 100f;
  private const float GridHalfCellSize = 50f;
  private const float GridRcpCellSize = 0.01f;
  private const int GridPoolCapacity = 16384;
  private const int GridPoolGranularity = 4096;
  private static OcclusionCulling.HashedPool<OcclusionCulling.Cell> grid;
  private static Queue<OcclusionCulling.Cell> gridChanged;

  public static OcclusionCulling Instance
  {
    get
    {
      return OcclusionCulling.instance;
    }
  }

  public static bool Supported
  {
    get
    {
      return ((IEnumerable<GraphicsDeviceType>) OcclusionCulling.supportedDeviceTypes).Contains<GraphicsDeviceType>(SystemInfo.get_graphicsDeviceType());
    }
  }

  public static bool Enabled
  {
    get
    {
      return OcclusionCulling._enabled;
    }
    set
    {
      OcclusionCulling._enabled = value;
      if (!Object.op_Inequality((Object) OcclusionCulling.instance, (Object) null))
        return;
      ((Behaviour) OcclusionCulling.instance).set_enabled(value);
    }
  }

  public static bool SafeMode
  {
    get
    {
      return OcclusionCulling._safeMode;
    }
    set
    {
      OcclusionCulling._safeMode = value;
    }
  }

  public static OcclusionCulling.DebugFilter DebugShow
  {
    get
    {
      return OcclusionCulling._debugShow;
    }
    set
    {
      OcclusionCulling._debugShow = value;
    }
  }

  private static void GrowStatePool()
  {
    for (int index = 0; index < 2048; ++index)
      OcclusionCulling.statePool.Enqueue(new OccludeeState());
  }

  private static OccludeeState Allocate()
  {
    if (OcclusionCulling.statePool.Count == 0)
      OcclusionCulling.GrowStatePool();
    return OcclusionCulling.statePool.Dequeue();
  }

  private static void Release(OccludeeState state)
  {
    OcclusionCulling.statePool.Enqueue(state);
  }

  private void Awake()
  {
    OcclusionCulling.instance = this;
    this.camera = (Camera) ((Component) this).GetComponent<Camera>();
    for (int index = 0; index < 6; ++index)
      this.frustumPropNames[index] = "_FrustumPlane" + (object) index;
  }

  private void OnEnable()
  {
    if (!OcclusionCulling.Enabled)
      OcclusionCulling.Enabled = false;
    else if (!OcclusionCulling.Supported)
    {
      Debug.LogWarning((object) ("[OcclusionCulling] Disabled due to graphics device type " + (object) SystemInfo.get_graphicsDeviceType() + " not supported."));
      OcclusionCulling.Enabled = false;
    }
    else
    {
      this.usePixelShaderFallback = this.usePixelShaderFallback || !SystemInfo.get_supportsComputeShaders() || Object.op_Equality((Object) this.computeShader, (Object) null) || !this.computeShader.HasKernel("compute_cull");
      this.useNativePath = SystemInfo.get_graphicsDeviceType() == 2 && this.SupportsNativePath();
      this.useAsyncReadAPI = !this.useNativePath && SystemInfo.get_supportsAsyncGPUReadback();
      if (!this.useNativePath && !this.useAsyncReadAPI)
      {
        Debug.LogWarning((object) ("[OcclusionCulling] Disabled due to unsupported Async GPU Reads on device " + (object) SystemInfo.get_graphicsDeviceType()));
        OcclusionCulling.Enabled = false;
      }
      else
      {
        for (int index = 0; index < OcclusionCulling.staticOccludees.Count; ++index)
          OcclusionCulling.staticChanged.Add(index);
        for (int index = 0; index < OcclusionCulling.dynamicOccludees.Count; ++index)
          OcclusionCulling.dynamicChanged.Add(index);
        if (this.usePixelShaderFallback)
        {
          Material material = new Material(Shader.Find("Hidden/OcclusionCulling/Culling"));
          ((Object) material).set_hideFlags((HideFlags) 61);
          this.fallbackMat = material;
        }
        OcclusionCulling.staticSet.Attach(this);
        OcclusionCulling.dynamicSet.Attach(this);
        OcclusionCulling.gridSet.Attach(this);
        Material material1 = new Material(Shader.Find("Hidden/OcclusionCulling/DepthCopy"));
        ((Object) material1).set_hideFlags((HideFlags) 61);
        this.depthCopyMat = material1;
        this.InitializeHiZMap();
        this.UpdateCameraMatrices(true);
      }
    }
  }

  private bool SupportsNativePath()
  {
    bool flag = true;
    try
    {
      OccludeeState.State states = new OccludeeState.State();
      Color32 results;
      ((Color32) ref results).\u002Ector((byte) 0, (byte) 0, (byte) 0, (byte) 0);
      Vector4 zero = Vector4.get_zero();
      int bucket = 0;
      int changed = 0;
      int changedCount = 0;
      OcclusionCulling.ProcessOccludees_Native(ref states, ref bucket, 0, ref results, 0, ref changed, ref changedCount, ref zero, 0.0f, 0U);
    }
    catch (EntryPointNotFoundException ex)
    {
      Debug.Log((object) "[OcclusionCulling] Fast native path not available. Reverting to managed fallback.");
      flag = false;
    }
    return flag;
  }

  private void OnDisable()
  {
    if (Object.op_Inequality((Object) this.fallbackMat, (Object) null))
    {
      Object.DestroyImmediate((Object) this.fallbackMat);
      this.fallbackMat = (Material) null;
    }
    if (Object.op_Inequality((Object) this.depthCopyMat, (Object) null))
    {
      Object.DestroyImmediate((Object) this.depthCopyMat);
      this.depthCopyMat = (Material) null;
    }
    OcclusionCulling.staticSet.Dispose(true);
    OcclusionCulling.dynamicSet.Dispose(true);
    OcclusionCulling.gridSet.Dispose(true);
    this.FinalizeHiZMap();
  }

  public static void MakeAllVisible()
  {
    for (int index = 0; index < OcclusionCulling.staticOccludees.Count; ++index)
    {
      if (OcclusionCulling.staticOccludees[index] != null)
        OcclusionCulling.staticOccludees[index].MakeVisible();
    }
    for (int index = 0; index < OcclusionCulling.dynamicOccludees.Count; ++index)
    {
      if (OcclusionCulling.dynamicOccludees[index] != null)
        OcclusionCulling.dynamicOccludees[index].MakeVisible();
    }
  }

  private void Update()
  {
    if (!OcclusionCulling.Enabled)
    {
      ((Behaviour) this).set_enabled(false);
    }
    else
    {
      this.CheckResizeHiZMap();
      this.DebugUpdate();
      this.DebugDraw();
    }
  }

  public static void RecursiveAddOccludees<T>(
    Transform transform,
    float minTimeVisible = 0.1f,
    bool isStatic = true,
    bool stickyGizmos = false)
    where T : Occludee
  {
    Renderer component1 = (Renderer) ((Component) transform).GetComponent<Renderer>();
    Collider component2 = (Collider) ((Component) transform).GetComponent<Collider>();
    if (Object.op_Inequality((Object) component1, (Object) null) && Object.op_Inequality((Object) component2, (Object) null))
    {
      T component3 = ((Component) component1).get_gameObject().GetComponent<T>();
      T obj = Object.op_Equality((Object) (object) component3, (Object) null) ? ((Component) component1).get_gameObject().AddComponent<T>() : component3;
      obj.minTimeVisible = minTimeVisible;
      obj.isStatic = isStatic;
      obj.stickyGizmos = stickyGizmos;
      obj.Register();
    }
    IEnumerator enumerator = transform.GetEnumerator();
    try
    {
      while (enumerator.MoveNext())
        OcclusionCulling.RecursiveAddOccludees<T>((Transform) enumerator.Current, minTimeVisible, isStatic, stickyGizmos);
    }
    finally
    {
      (enumerator as IDisposable)?.Dispose();
    }
  }

  private static int FindFreeSlot(
    OcclusionCulling.SimpleList<OccludeeState> occludees,
    OcclusionCulling.SimpleList<OccludeeState.State> states,
    Queue<int> recycled)
  {
    int num1;
    if (recycled.Count > 0)
    {
      num1 = recycled.Dequeue();
    }
    else
    {
      if (occludees.Count == occludees.Capacity)
      {
        int num2 = Mathf.Min(occludees.Capacity + 2048, 1048576);
        if (num2 > 0)
        {
          occludees.Capacity = num2;
          states.Capacity = num2;
        }
      }
      if (occludees.Count < occludees.Capacity)
      {
        num1 = occludees.Count;
        occludees.Add((OccludeeState) null);
        states.Add(new OccludeeState.State());
      }
      else
        num1 = -1;
    }
    return num1;
  }

  public static OccludeeState GetStateById(int id)
  {
    if (id < 0 || id >= 2097152)
      return (OccludeeState) null;
    int num = id < 1048576 ? 1 : 0;
    int index = num != 0 ? id : id - 1048576;
    if (num != 0)
      return OcclusionCulling.staticOccludees[index];
    return OcclusionCulling.dynamicOccludees[index];
  }

  public static int RegisterOccludee(
    Vector3 center,
    float radius,
    bool isVisible,
    float minTimeVisible,
    bool isStatic,
    int layer,
    OcclusionCulling.OnVisibilityChanged onVisibilityChanged = null)
  {
    int num = !isStatic ? OcclusionCulling.RegisterOccludee(center, radius, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged, OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicRecycled, OcclusionCulling.dynamicChanged, OcclusionCulling.dynamicSet, OcclusionCulling.dynamicVisibilityChanged) : OcclusionCulling.RegisterOccludee(center, radius, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged, OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticRecycled, OcclusionCulling.staticChanged, OcclusionCulling.staticSet, OcclusionCulling.staticVisibilityChanged);
    if (!(num < 0 | isStatic))
      return num + 1048576;
    return num;
  }

  private static int RegisterOccludee(
    Vector3 center,
    float radius,
    bool isVisible,
    float minTimeVisible,
    bool isStatic,
    int layer,
    OcclusionCulling.OnVisibilityChanged onVisibilityChanged,
    OcclusionCulling.SimpleList<OccludeeState> occludees,
    OcclusionCulling.SimpleList<OccludeeState.State> states,
    Queue<int> recycled,
    List<int> changed,
    OcclusionCulling.BufferSet set,
    OcclusionCulling.SimpleList<int> visibilityChanged)
  {
    int freeSlot = OcclusionCulling.FindFreeSlot(occludees, states, recycled);
    if (freeSlot >= 0)
    {
      Vector4 sphereBounds;
      ((Vector4) ref sphereBounds).\u002Ector((float) center.x, (float) center.y, (float) center.z, radius);
      OccludeeState occludee = OcclusionCulling.Allocate().Initialize(states, set, freeSlot, sphereBounds, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged);
      occludee.cell = OcclusionCulling.RegisterToGrid(occludee);
      occludees[freeSlot] = occludee;
      changed.Add(freeSlot);
      if (states.array[freeSlot].isVisible > (byte) 0 != occludee.cell.isVisible)
        visibilityChanged.Add(freeSlot);
    }
    return freeSlot;
  }

  public static void UnregisterOccludee(int id)
  {
    if (id < 0 || id >= 2097152)
      return;
    int num = id < 1048576 ? 1 : 0;
    int slot = num != 0 ? id : id - 1048576;
    if (num != 0)
      OcclusionCulling.UnregisterOccludee(slot, OcclusionCulling.staticOccludees, OcclusionCulling.staticRecycled, OcclusionCulling.staticChanged);
    else
      OcclusionCulling.UnregisterOccludee(slot, OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicRecycled, OcclusionCulling.dynamicChanged);
  }

  private static void UnregisterOccludee(
    int slot,
    OcclusionCulling.SimpleList<OccludeeState> occludees,
    Queue<int> recycled,
    List<int> changed)
  {
    OccludeeState occludee = occludees[slot];
    OcclusionCulling.UnregisterFromGrid(occludee);
    recycled.Enqueue(slot);
    changed.Add(slot);
    OcclusionCulling.Release(occludee);
    occludees[slot] = (OccludeeState) null;
    occludee.Invalidate();
  }

  public static void UpdateDynamicOccludee(int id, Vector3 center, float radius)
  {
    int index = id - 1048576;
    if (index < 0 || index >= 1048576)
      return;
    OcclusionCulling.dynamicStates.array[index].sphereBounds = new Vector4((float) center.x, (float) center.y, (float) center.z, radius);
    OcclusionCulling.dynamicChanged.Add(index);
  }

  private void UpdateBuffers(
    OcclusionCulling.SimpleList<OccludeeState> occludees,
    OcclusionCulling.SimpleList<OccludeeState.State> states,
    OcclusionCulling.BufferSet set,
    List<int> changed,
    bool isStatic)
  {
    int count = occludees.Count;
    bool flag = changed.Count > 0;
    set.CheckResize(count, 2048);
    for (int index1 = 0; index1 < changed.Count; ++index1)
    {
      int index2 = changed[index1];
      OccludeeState occludee = occludees[index2];
      if (occludee != null)
      {
        if (!isStatic)
          OcclusionCulling.UpdateInGrid(occludee);
        set.inputData[index2] = Color.op_Implicit(states[index2].sphereBounds);
      }
      else
        set.inputData[index2] = Color.op_Implicit(Vector4.get_zero());
    }
    changed.Clear();
    if (!flag)
      return;
    set.UploadData();
  }

  private void UpdateCameraMatrices(bool starting = false)
  {
    if (!starting)
      this.prevViewProjMatrix = this.viewProjMatrix;
    Matrix4x4 matrix4x4 = Matrix4x4.Perspective(this.camera.get_fieldOfView(), this.camera.get_aspect(), this.camera.get_nearClipPlane(), this.camera.get_farClipPlane());
    this.viewMatrix = this.camera.get_worldToCameraMatrix();
    this.projMatrix = GL.GetGPUProjectionMatrix(matrix4x4, false);
    this.viewProjMatrix = Matrix4x4.op_Multiply(this.projMatrix, this.viewMatrix);
    this.invViewProjMatrix = Matrix4x4.Inverse(this.viewProjMatrix);
    if (!starting)
      return;
    this.prevViewProjMatrix = this.viewProjMatrix;
  }

  private void OnPreCull()
  {
    this.UpdateCameraMatrices(false);
    this.GenerateHiZMipChain();
    this.PrepareAndDispatch();
    this.IssueRead();
    if (OcclusionCulling.grid.Size <= OcclusionCulling.gridSet.resultData.Length)
      this.RetrieveAndApplyVisibility();
    else
      Debug.LogWarning((object) ("[OcclusionCulling] Grid size and result capacity are out of sync: " + (object) OcclusionCulling.grid.Size + ", " + (object) OcclusionCulling.gridSet.resultData.Length));
  }

  private void OnPostRender()
  {
    int num = GL.get_sRGBWrite() ? 1 : 0;
    RenderBuffer activeColorBuffer = Graphics.get_activeColorBuffer();
    RenderBuffer activeDepthBuffer = Graphics.get_activeDepthBuffer();
    this.GrabDepthTexture();
    RenderBuffer renderBuffer = activeDepthBuffer;
    Graphics.SetRenderTarget(activeColorBuffer, renderBuffer);
    GL.set_sRGBWrite(num != 0);
  }

  private float[] MatrixToFloatArray(Matrix4x4 m)
  {
    int num1 = 0;
    int num2 = 0;
    for (; num1 < 4; ++num1)
    {
      for (int index = 0; index < 4; ++index)
        this.matrixToFloatTemp[num2++] = ((Matrix4x4) ref m).get_Item(index, num1);
    }
    return this.matrixToFloatTemp;
  }

  private void PrepareAndDispatch()
  {
    Vector2 vector2;
    ((Vector2) ref vector2).\u002Ector((float) this.hiZWidth, (float) this.hiZHeight);
    OcclusionCulling.ExtractFrustum(this.viewProjMatrix, ref this.frustumPlanes);
    bool flag = true;
    if (this.usePixelShaderFallback)
    {
      this.fallbackMat.SetTexture("_HiZMap", (Texture) this.hiZTexture);
      this.fallbackMat.SetFloat("_HiZMaxLod", (float) (this.hiZLevelCount - 1));
      this.fallbackMat.SetMatrix("_ViewMatrix", this.viewMatrix);
      this.fallbackMat.SetMatrix("_ProjMatrix", this.projMatrix);
      this.fallbackMat.SetMatrix("_ViewProjMatrix", this.viewProjMatrix);
      this.fallbackMat.SetVector("_CameraWorldPos", Vector4.op_Implicit(((Component) this).get_transform().get_position()));
      this.fallbackMat.SetVector("_ViewportSize", Vector4.op_Implicit(vector2));
      this.fallbackMat.SetFloat("_FrustumCull", flag ? 0.0f : 1f);
      for (int index = 0; index < 6; ++index)
        this.fallbackMat.SetVector(this.frustumPropNames[index], this.frustumPlanes[index]);
    }
    else
    {
      this.computeShader.SetTexture(0, "_HiZMap", (Texture) this.hiZTexture);
      this.computeShader.SetFloat("_HiZMaxLod", (float) (this.hiZLevelCount - 1));
      this.computeShader.SetFloats("_ViewMatrix", this.MatrixToFloatArray(this.viewMatrix));
      this.computeShader.SetFloats("_ProjMatrix", this.MatrixToFloatArray(this.projMatrix));
      this.computeShader.SetFloats("_ViewProjMatrix", this.MatrixToFloatArray(this.viewProjMatrix));
      this.computeShader.SetVector("_CameraWorldPos", Vector4.op_Implicit(((Component) this).get_transform().get_position()));
      this.computeShader.SetVector("_ViewportSize", Vector4.op_Implicit(vector2));
      this.computeShader.SetFloat("_FrustumCull", flag ? 0.0f : 1f);
      for (int index = 0; index < 6; ++index)
        this.computeShader.SetVector(this.frustumPropNames[index], this.frustumPlanes[index]);
    }
    if (OcclusionCulling.staticOccludees.Count > 0)
    {
      this.UpdateBuffers(OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticSet, OcclusionCulling.staticChanged, true);
      OcclusionCulling.staticSet.Dispatch(OcclusionCulling.staticOccludees.Count);
    }
    if (OcclusionCulling.dynamicOccludees.Count > 0)
    {
      this.UpdateBuffers(OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicSet, OcclusionCulling.dynamicChanged, false);
      OcclusionCulling.dynamicSet.Dispatch(OcclusionCulling.dynamicOccludees.Count);
    }
    this.UpdateGridBuffers();
    OcclusionCulling.gridSet.Dispatch(OcclusionCulling.grid.Size);
  }

  private void IssueRead()
  {
    if (OcclusionCulling.staticOccludees.Count > 0)
      OcclusionCulling.staticSet.IssueRead();
    if (OcclusionCulling.dynamicOccludees.Count > 0)
      OcclusionCulling.dynamicSet.IssueRead();
    if (OcclusionCulling.grid.Count > 0)
      OcclusionCulling.gridSet.IssueRead();
    GL.IssuePluginEvent(Graphics.GetRenderEventFunc(), 2);
  }

  public void ResetTiming(OcclusionCulling.SmartList bucket)
  {
    for (int index = 0; index < bucket.Size; ++index)
    {
      OccludeeState occludeeState = bucket[index];
      if (occludeeState != null)
        occludeeState.states.array[occludeeState.slot].waitTime = 0.0f;
    }
  }

  public void ResetTiming()
  {
    for (int index = 0; index < OcclusionCulling.grid.Size; ++index)
    {
      OcclusionCulling.Cell cell = OcclusionCulling.grid[index];
      if (cell != null)
      {
        this.ResetTiming(cell.staticBucket);
        this.ResetTiming(cell.dynamicBucket);
      }
    }
  }

  private static bool FrustumCull(Vector4[] planes, Vector4 testSphere)
  {
    for (int index = 0; index < 6; ++index)
    {
      if (planes[index].x * testSphere.x + planes[index].y * testSphere.y + planes[index].z * testSphere.z + planes[index].w < -testSphere.w)
        return false;
    }
    return true;
  }

  private static int ProcessOccludees_Safe(
    OcclusionCulling.SimpleList<OccludeeState.State> states,
    OcclusionCulling.SmartList bucket,
    Color32[] results,
    OcclusionCulling.SimpleList<int> changed,
    Vector4[] frustumPlanes,
    float time,
    uint frame)
  {
    int num = 0;
    for (int index = 0; index < bucket.Size; ++index)
    {
      OccludeeState occludeeState = bucket[index];
      if (occludeeState != null && occludeeState.slot < results.Length)
      {
        int slot = occludeeState.slot;
        OccludeeState.State state = states[slot];
        bool flag1 = OcclusionCulling.FrustumCull(frustumPlanes, state.sphereBounds);
        bool flag2 = results[slot].r > 0 & flag1;
        if (flag2 || frame < state.waitFrame)
          state.waitTime = time + state.minTimeVisible;
        if (!flag2)
          flag2 = (double) time < (double) state.waitTime;
        if (flag2 != state.isVisible > (byte) 0)
        {
          if (state.callback != (byte) 0)
            changed.Add(slot);
          else
            state.isVisible = flag2 ? (byte) 1 : (byte) 0;
        }
        states[slot] = state;
        num += (int) state.isVisible;
      }
    }
    return num;
  }

  private static int ProcessOccludees_Fast(
    OccludeeState.State[] states,
    int[] bucket,
    int bucketCount,
    Color32[] results,
    int resultCount,
    int[] changed,
    ref int changedCount,
    Vector4[] frustumPlanes,
    float time,
    uint frame)
  {
    int num = 0;
    for (int index1 = 0; index1 < bucketCount; ++index1)
    {
      int index2 = bucket[index1];
      if (index2 >= 0 && index2 < resultCount && states[index2].active != (byte) 0)
      {
        OccludeeState.State state = states[index2];
        bool flag1 = OcclusionCulling.FrustumCull(frustumPlanes, state.sphereBounds);
        bool flag2 = results[index2].r > 0 & flag1;
        if (flag2 || frame < state.waitFrame)
          state.waitTime = time + state.minTimeVisible;
        if (!flag2)
          flag2 = (double) time < (double) state.waitTime;
        if (flag2 != state.isVisible > (byte) 0)
        {
          if (state.callback != (byte) 0)
            changed[changedCount++] = index2;
          else
            state.isVisible = flag2 ? (byte) 1 : (byte) 0;
        }
        states[index2] = state;
        num += flag2 ? 0 : 1;
      }
    }
    return num;
  }

  [DllImport("Renderer", EntryPoint = "CULL_ProcessOccludees")]
  private static extern int ProcessOccludees_Native(
    ref OccludeeState.State states,
    ref int bucket,
    int bucketCount,
    ref Color32 results,
    int resultCount,
    ref int changed,
    ref int changedCount,
    ref Vector4 frustumPlanes,
    float time,
    uint frame);

  private void ApplyVisibility_Safe(float time, uint frame)
  {
    bool ready1 = OcclusionCulling.staticSet.Ready;
    bool ready2 = OcclusionCulling.dynamicSet.Ready;
    for (int index = 0; index < OcclusionCulling.grid.Size; ++index)
    {
      OcclusionCulling.Cell cell = OcclusionCulling.grid[index];
      if (cell != null && OcclusionCulling.gridSet.resultData.Length != 0)
      {
        bool flag1 = OcclusionCulling.FrustumCull(this.frustumPlanes, cell.sphereBounds);
        bool flag2 = OcclusionCulling.gridSet.resultData[index].r > 0 & flag1;
        if (cell.isVisible | flag2)
        {
          int num1 = 0;
          int num2 = 0;
          if (ready1 && cell.staticBucket.Count > 0)
            num1 = OcclusionCulling.ProcessOccludees_Safe(OcclusionCulling.staticStates, cell.staticBucket, OcclusionCulling.staticSet.resultData, OcclusionCulling.staticVisibilityChanged, this.frustumPlanes, time, frame);
          if (ready2 && cell.dynamicBucket.Count > 0)
            num2 = OcclusionCulling.ProcessOccludees_Safe(OcclusionCulling.dynamicStates, cell.dynamicBucket, OcclusionCulling.dynamicSet.resultData, OcclusionCulling.dynamicVisibilityChanged, this.frustumPlanes, time, frame);
          cell.isVisible = flag2 || num1 < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count;
        }
      }
    }
  }

  private void ApplyVisibility_Fast(float time, uint frame)
  {
    bool ready1 = OcclusionCulling.staticSet.Ready;
    bool ready2 = OcclusionCulling.dynamicSet.Ready;
    for (int index = 0; index < OcclusionCulling.grid.Size; ++index)
    {
      OcclusionCulling.Cell cell = OcclusionCulling.grid[index];
      if (cell != null && OcclusionCulling.gridSet.resultData.Length != 0)
      {
        bool flag1 = OcclusionCulling.FrustumCull(this.frustumPlanes, cell.sphereBounds);
        bool flag2 = OcclusionCulling.gridSet.resultData[index].r > 0 & flag1;
        if (cell.isVisible | flag2)
        {
          int num1 = 0;
          int num2 = 0;
          if (ready1 && cell.staticBucket.Count > 0)
            num1 = OcclusionCulling.ProcessOccludees_Fast(OcclusionCulling.staticStates.array, cell.staticBucket.Slots, cell.staticBucket.Size, OcclusionCulling.staticSet.resultData, OcclusionCulling.staticSet.resultData.Length, OcclusionCulling.staticVisibilityChanged.array, ref OcclusionCulling.staticVisibilityChanged.count, this.frustumPlanes, time, frame);
          if (ready2 && cell.dynamicBucket.Count > 0)
            num2 = OcclusionCulling.ProcessOccludees_Fast(OcclusionCulling.dynamicStates.array, cell.dynamicBucket.Slots, cell.dynamicBucket.Size, OcclusionCulling.dynamicSet.resultData, OcclusionCulling.dynamicSet.resultData.Length, OcclusionCulling.dynamicVisibilityChanged.array, ref OcclusionCulling.dynamicVisibilityChanged.count, this.frustumPlanes, time, frame);
          cell.isVisible = flag2 || num1 < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count;
        }
      }
    }
  }

  private void ApplyVisibility_Native(float time, uint frame)
  {
    bool ready1 = OcclusionCulling.staticSet.Ready;
    bool ready2 = OcclusionCulling.dynamicSet.Ready;
    for (int index = 0; index < OcclusionCulling.grid.Size; ++index)
    {
      OcclusionCulling.Cell cell = OcclusionCulling.grid[index];
      if (cell != null && OcclusionCulling.gridSet.resultData.Length != 0)
      {
        bool flag1 = OcclusionCulling.FrustumCull(this.frustumPlanes, cell.sphereBounds);
        bool flag2 = OcclusionCulling.gridSet.resultData[index].r > 0 & flag1;
        if (cell.isVisible | flag2)
        {
          int num1 = 0;
          int num2 = 0;
          if (ready1 && cell.staticBucket.Count > 0)
            num1 = OcclusionCulling.ProcessOccludees_Native(ref OcclusionCulling.staticStates.array[0], ref cell.staticBucket.Slots[0], cell.staticBucket.Size, ref OcclusionCulling.staticSet.resultData[0], OcclusionCulling.staticSet.resultData.Length, ref OcclusionCulling.staticVisibilityChanged.array[0], ref OcclusionCulling.staticVisibilityChanged.count, ref this.frustumPlanes[0], time, frame);
          if (ready2 && cell.dynamicBucket.Count > 0)
            num2 = OcclusionCulling.ProcessOccludees_Native(ref OcclusionCulling.dynamicStates.array[0], ref cell.dynamicBucket.Slots[0], cell.dynamicBucket.Size, ref OcclusionCulling.dynamicSet.resultData[0], OcclusionCulling.dynamicSet.resultData.Length, ref OcclusionCulling.dynamicVisibilityChanged.array[0], ref OcclusionCulling.dynamicVisibilityChanged.count, ref this.frustumPlanes[0], time, frame);
          cell.isVisible = flag2 || num1 < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count;
        }
      }
    }
  }

  private void ProcessCallbacks(
    OcclusionCulling.SimpleList<OccludeeState> occludees,
    OcclusionCulling.SimpleList<OccludeeState.State> states,
    OcclusionCulling.SimpleList<int> changed)
  {
    for (int index1 = 0; index1 < changed.Count; ++index1)
    {
      int index2 = changed[index1];
      OccludeeState occludee = occludees[index2];
      if (occludee != null)
      {
        bool visible = states.array[index2].isVisible == (byte) 0;
        OcclusionCulling.OnVisibilityChanged visibilityChanged = occludee.onVisibilityChanged;
        if (visibilityChanged != null && Object.op_Inequality((Object) visibilityChanged.Target, (Object) null))
          visibilityChanged(visible);
        if (occludee.slot >= 0)
          states.array[occludee.slot].isVisible = visible ? (byte) 1 : (byte) 0;
      }
    }
    changed.Clear();
  }

  public void RetrieveAndApplyVisibility()
  {
    if (OcclusionCulling.staticOccludees.Count > 0)
      OcclusionCulling.staticSet.GetResults();
    if (OcclusionCulling.dynamicOccludees.Count > 0)
      OcclusionCulling.dynamicSet.GetResults();
    if (OcclusionCulling.grid.Count > 0)
      OcclusionCulling.gridSet.GetResults();
    if (this.debugSettings.showAllVisible)
    {
      for (int index = 0; index < OcclusionCulling.staticSet.resultData.Length; ++index)
        OcclusionCulling.staticSet.resultData[index].r = (__Null) 1;
      for (int index = 0; index < OcclusionCulling.dynamicSet.resultData.Length; ++index)
        OcclusionCulling.dynamicSet.resultData[index].r = (__Null) 1;
      for (int index = 0; index < OcclusionCulling.gridSet.resultData.Length; ++index)
        OcclusionCulling.gridSet.resultData[index].r = (__Null) 1;
    }
    OcclusionCulling.staticVisibilityChanged.EnsureCapacity(OcclusionCulling.staticOccludees.Count);
    OcclusionCulling.dynamicVisibilityChanged.EnsureCapacity(OcclusionCulling.dynamicOccludees.Count);
    float time = Time.get_time();
    uint frameCount = (uint) Time.get_frameCount();
    if (this.useNativePath)
      this.ApplyVisibility_Native(time, frameCount);
    else
      this.ApplyVisibility_Fast(time, frameCount);
    this.ProcessCallbacks(OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticVisibilityChanged);
    this.ProcessCallbacks(OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicVisibilityChanged);
  }

  public static bool DebugFilterIsDynamic(int filter)
  {
    if (filter != 1)
      return filter == 4;
    return true;
  }

  public static bool DebugFilterIsStatic(int filter)
  {
    if (filter != 2)
      return filter == 4;
    return true;
  }

  public static bool DebugFilterIsGrid(int filter)
  {
    if (filter != 3)
      return filter == 4;
    return true;
  }

  private void DebugInitialize()
  {
    Material material = new Material(Shader.Find("Hidden/OcclusionCulling/DebugMip"));
    ((Object) material).set_hideFlags((HideFlags) 61);
    this.debugMipMat = material;
  }

  private void DebugShutdown()
  {
    if (!Object.op_Inequality((Object) this.debugMipMat, (Object) null))
      return;
    Object.DestroyImmediate((Object) this.debugMipMat);
    this.debugMipMat = (Material) null;
  }

  private void DebugUpdate()
  {
    if (!this.HiZReady)
      return;
    this.debugSettings.showMainLod = Mathf.Clamp(this.debugSettings.showMainLod, 0, this.hiZLevels.Length - 1);
  }

  private void DebugDraw()
  {
  }

  public static void NormalizePlane(ref Vector4 plane)
  {
    float num = Mathf.Sqrt((float) (plane.x * plane.x + plane.y * plane.y + plane.z * plane.z));
    ref __Null local1 = ref plane.x;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local1 = ^(float&) ref local1 / num;
    ref __Null local2 = ref plane.y;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local2 = ^(float&) ref local2 / num;
    ref __Null local3 = ref plane.z;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local3 = ^(float&) ref local3 / num;
    ref __Null local4 = ref plane.w;
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(float&) ref local4 = ^(float&) ref local4 / num;
  }

  public static void ExtractFrustum(Matrix4x4 viewProjMatrix, ref Vector4[] planes)
  {
    planes[0].x = viewProjMatrix.m30 + viewProjMatrix.m00;
    planes[0].y = viewProjMatrix.m31 + viewProjMatrix.m01;
    planes[0].z = viewProjMatrix.m32 + viewProjMatrix.m02;
    planes[0].w = viewProjMatrix.m33 + viewProjMatrix.m03;
    OcclusionCulling.NormalizePlane(ref planes[0]);
    planes[1].x = viewProjMatrix.m30 - viewProjMatrix.m00;
    planes[1].y = viewProjMatrix.m31 - viewProjMatrix.m01;
    planes[1].z = viewProjMatrix.m32 - viewProjMatrix.m02;
    planes[1].w = viewProjMatrix.m33 - viewProjMatrix.m03;
    OcclusionCulling.NormalizePlane(ref planes[1]);
    planes[2].x = viewProjMatrix.m30 - viewProjMatrix.m10;
    planes[2].y = viewProjMatrix.m31 - viewProjMatrix.m11;
    planes[2].z = viewProjMatrix.m32 - viewProjMatrix.m12;
    planes[2].w = viewProjMatrix.m33 - viewProjMatrix.m13;
    OcclusionCulling.NormalizePlane(ref planes[2]);
    planes[3].x = viewProjMatrix.m30 + viewProjMatrix.m10;
    planes[3].y = viewProjMatrix.m31 + viewProjMatrix.m11;
    planes[3].z = viewProjMatrix.m32 + viewProjMatrix.m12;
    planes[3].w = viewProjMatrix.m33 + viewProjMatrix.m13;
    OcclusionCulling.NormalizePlane(ref planes[3]);
    planes[4].x = viewProjMatrix.m20;
    planes[4].y = viewProjMatrix.m21;
    planes[4].z = viewProjMatrix.m22;
    planes[4].w = viewProjMatrix.m23;
    OcclusionCulling.NormalizePlane(ref planes[4]);
    planes[5].x = viewProjMatrix.m30 - viewProjMatrix.m20;
    planes[5].y = viewProjMatrix.m31 - viewProjMatrix.m21;
    planes[5].z = viewProjMatrix.m32 - viewProjMatrix.m22;
    planes[5].w = viewProjMatrix.m33 - viewProjMatrix.m23;
    OcclusionCulling.NormalizePlane(ref planes[5]);
  }

  public bool HiZReady
  {
    get
    {
      if (Object.op_Inequality((Object) this.hiZTexture, (Object) null) && this.hiZWidth > 0)
        return this.hiZHeight > 0;
      return false;
    }
  }

  public void CheckResizeHiZMap()
  {
    int pixelWidth = this.camera.get_pixelWidth();
    int pixelHeight = this.camera.get_pixelHeight();
    if (pixelWidth <= 0 || pixelHeight <= 0)
      return;
    int width = pixelWidth / 4;
    int height = pixelHeight / 4;
    if (this.hiZLevels != null && this.hiZWidth == width && this.hiZHeight == height)
      return;
    this.InitializeHiZMap(width, height);
    this.hiZWidth = width;
    this.hiZHeight = height;
    if (!this.debugSettings.log)
      return;
    Debug.Log((object) ("[OcclusionCulling] Resized HiZ Map to " + (object) this.hiZWidth + " x " + (object) this.hiZHeight));
  }

  private void InitializeHiZMap()
  {
    Shader shader1 = Shader.Find("Hidden/OcclusionCulling/DepthDownscale");
    Shader shader2 = Shader.Find("Hidden/OcclusionCulling/BlitCopy");
    Material material1 = new Material(shader1);
    ((Object) material1).set_hideFlags((HideFlags) 61);
    this.downscaleMat = material1;
    Material material2 = new Material(shader2);
    ((Object) material2).set_hideFlags((HideFlags) 61);
    this.blitCopyMat = material2;
    this.CheckResizeHiZMap();
  }

  private void FinalizeHiZMap()
  {
    this.DestroyHiZMap();
    if (Object.op_Inequality((Object) this.downscaleMat, (Object) null))
    {
      Object.DestroyImmediate((Object) this.downscaleMat);
      this.downscaleMat = (Material) null;
    }
    if (!Object.op_Inequality((Object) this.blitCopyMat, (Object) null))
      return;
    Object.DestroyImmediate((Object) this.blitCopyMat);
    this.blitCopyMat = (Material) null;
  }

  private void InitializeHiZMap(int width, int height)
  {
    this.DestroyHiZMap();
    width = Mathf.Clamp(width, 1, 65536);
    height = Mathf.Clamp(height, 1, 65536);
    this.hiZLevelCount = (int) ((double) Mathf.Log((float) Mathf.Min(width, height), 2f) + 1.0);
    this.hiZLevels = new RenderTexture[this.hiZLevelCount];
    this.depthTexture = this.CreateDepthTexture("DepthTex", width, height, false);
    this.hiZTexture = this.CreateDepthTexture("HiZMapTex", width, height, true);
    for (int mip = 0; mip < this.hiZLevelCount; ++mip)
      this.hiZLevels[mip] = this.CreateDepthTextureMip("HiZMap" + (object) mip, width, height, mip);
  }

  private void DestroyHiZMap()
  {
    if (Object.op_Inequality((Object) this.depthTexture, (Object) null))
    {
      RenderTexture.set_active((RenderTexture) null);
      Object.DestroyImmediate((Object) this.depthTexture);
      this.depthTexture = (RenderTexture) null;
    }
    if (Object.op_Inequality((Object) this.hiZTexture, (Object) null))
    {
      RenderTexture.set_active((RenderTexture) null);
      Object.DestroyImmediate((Object) this.hiZTexture);
      this.hiZTexture = (RenderTexture) null;
    }
    if (this.hiZLevels == null)
      return;
    for (int index = 0; index < this.hiZLevels.Length; ++index)
      Object.DestroyImmediate((Object) this.hiZLevels[index]);
    this.hiZLevels = (RenderTexture[]) null;
  }

  private RenderTexture CreateDepthTexture(
    string name,
    int width,
    int height,
    bool mips = false)
  {
    RenderTexture renderTexture = new RenderTexture(width, height, 0, (RenderTextureFormat) 14, (RenderTextureReadWrite) 1);
    ((Object) renderTexture).set_name(name);
    renderTexture.set_useMipMap(mips);
    renderTexture.set_autoGenerateMips(false);
    ((Texture) renderTexture).set_wrapMode((TextureWrapMode) 1);
    ((Texture) renderTexture).set_filterMode((FilterMode) 0);
    renderTexture.Create();
    return renderTexture;
  }

  private RenderTexture CreateDepthTextureMip(
    string name,
    int width,
    int height,
    int mip)
  {
    int num1 = width >> mip;
    int num2 = height >> mip;
    int num3 = mip == 0 ? 24 : 0;
    int num4 = num2;
    int num5 = num3;
    RenderTexture renderTexture = new RenderTexture(num1, num4, num5, (RenderTextureFormat) 14, (RenderTextureReadWrite) 1);
    ((Object) renderTexture).set_name(name);
    renderTexture.set_useMipMap(false);
    ((Texture) renderTexture).set_wrapMode((TextureWrapMode) 1);
    ((Texture) renderTexture).set_filterMode((FilterMode) 0);
    renderTexture.Create();
    return renderTexture;
  }

  public void GrabDepthTexture()
  {
    if (!Object.op_Inequality((Object) this.depthTexture, (Object) null))
      return;
    Graphics.Blit((Texture) null, this.depthTexture, this.depthCopyMat, 0);
  }

  public void GenerateHiZMipChain()
  {
    if (!this.HiZReady)
      return;
    bool flag = true;
    this.depthCopyMat.SetMatrix("_CameraReprojection", Matrix4x4.op_Multiply(this.prevViewProjMatrix, this.invViewProjMatrix));
    this.depthCopyMat.SetFloat("_FrustumNoDataDepth", flag ? 1f : 0.0f);
    Graphics.Blit((Texture) this.depthTexture, this.hiZLevels[0], this.depthCopyMat, 1);
    for (int index = 1; index < this.hiZLevels.Length; ++index)
    {
      RenderTexture hiZlevel1 = this.hiZLevels[index - 1];
      RenderTexture hiZlevel2 = this.hiZLevels[index];
      int num = (((Texture) hiZlevel1).get_width() & 1) != 0 || (((Texture) hiZlevel1).get_height() & 1) != 0 ? 1 : 0;
      this.downscaleMat.SetTexture("_MainTex", (Texture) hiZlevel1);
      Graphics.Blit((Texture) hiZlevel1, hiZlevel2, this.downscaleMat, num);
    }
    for (int index = 0; index < this.hiZLevels.Length; ++index)
    {
      Graphics.SetRenderTarget(this.hiZTexture, index);
      Graphics.Blit((Texture) this.hiZLevels[index], this.blitCopyMat);
    }
  }

  private void DebugDrawGizmos()
  {
    Camera component = (Camera) ((Component) this).GetComponent<Camera>();
    Gizmos.set_color(new Color(0.75f, 0.75f, 0.0f, 0.5f));
    Gizmos.set_matrix(Matrix4x4.TRS(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_rotation(), Vector3.get_one()));
    Gizmos.DrawFrustum(Vector3.get_zero(), component.get_fieldOfView(), component.get_farClipPlane(), component.get_nearClipPlane(), component.get_aspect());
    Gizmos.set_color(Color.get_red());
    Gizmos.set_matrix(Matrix4x4.get_identity());
    Matrix4x4 worldToCameraMatrix = component.get_worldToCameraMatrix();
    Matrix4x4 viewProjMatrix = Matrix4x4.op_Multiply(GL.GetGPUProjectionMatrix(component.get_projectionMatrix(), false), worldToCameraMatrix);
    Vector4[] vector4Array = new Vector4[6];
    ref Vector4[] local = ref vector4Array;
    OcclusionCulling.ExtractFrustum(viewProjMatrix, ref local);
    for (int index = 0; index < vector4Array.Length; ++index)
    {
      Vector3 vector3 = Vector3.op_Multiply(Vector3.op_UnaryNegation(new Vector3((float) vector4Array[index].x, (float) vector4Array[index].y, (float) vector4Array[index].z)), (float) vector4Array[index].w);
      Gizmos.DrawLine(vector3, Vector3.op_Multiply(vector3, 2f));
    }
  }

  private static int floor(float x)
  {
    int num = (int) x;
    if ((double) x >= (double) num)
      return num;
    return num - 1;
  }

  public static OcclusionCulling.Cell RegisterToGrid(OccludeeState occludee)
  {
    int x = OcclusionCulling.floor((float) (occludee.states.array[occludee.slot].sphereBounds.x * 0.00999999977648258));
    int y = OcclusionCulling.floor((float) (occludee.states.array[occludee.slot].sphereBounds.y * 0.00999999977648258));
    int z = OcclusionCulling.floor((float) (occludee.states.array[occludee.slot].sphereBounds.z * 0.00999999977648258));
    int num1 = Mathf.Clamp(x, -1048575, 1048575);
    int num2 = Mathf.Clamp(y, -1048575, 1048575);
    int num3 = Mathf.Clamp(z, -1048575, 1048575);
    ulong key = (ulong) ((num1 >= 0 ? (long) num1 : (long) (num1 + 1048575)) << 42 | (long) (num2 >= 0 ? (ulong) num2 : (ulong) (num2 + 1048575)) << 21) | (num3 >= 0 ? (ulong) num3 : (ulong) (num3 + 1048575));
    OcclusionCulling.Cell cell;
    int num4 = OcclusionCulling.grid.TryGetValue(key, out cell) ? 1 : 0;
    if (num4 == 0)
    {
      Vector3 vector3_1 = (Vector3) null;
      vector3_1.x = (__Null) ((double) x * 100.0 + 50.0);
      vector3_1.y = (__Null) ((double) y * 100.0 + 50.0);
      vector3_1.z = (__Null) ((double) z * 100.0 + 50.0);
      Vector3 vector3_2;
      ((Vector3) ref vector3_2).\u002Ector(100f, 100f, 100f);
      cell = OcclusionCulling.grid.Add(key, 16).Initialize(x, y, z, new Bounds(vector3_1, vector3_2));
    }
    OcclusionCulling.SmartList smartList = occludee.isStatic ? cell.staticBucket : cell.dynamicBucket;
    if (num4 == 0 || !smartList.Contains(occludee))
    {
      occludee.cell = cell;
      smartList.Add(occludee, 16);
      OcclusionCulling.gridChanged.Enqueue(cell);
    }
    return cell;
  }

  public static void UpdateInGrid(OccludeeState occludee)
  {
    int num1 = OcclusionCulling.floor((float) (occludee.states.array[occludee.slot].sphereBounds.x * 0.00999999977648258));
    int num2 = OcclusionCulling.floor((float) (occludee.states.array[occludee.slot].sphereBounds.y * 0.00999999977648258));
    int num3 = OcclusionCulling.floor((float) (occludee.states.array[occludee.slot].sphereBounds.z * 0.00999999977648258));
    int x = occludee.cell.x;
    if (num1 == x && num2 == occludee.cell.y && num3 == occludee.cell.z)
      return;
    OcclusionCulling.UnregisterFromGrid(occludee);
    OcclusionCulling.RegisterToGrid(occludee);
  }

  public static void UnregisterFromGrid(OccludeeState occludee)
  {
    OcclusionCulling.Cell cell = occludee.cell;
    OcclusionCulling.SmartList smartList = occludee.isStatic ? cell.staticBucket : cell.dynamicBucket;
    OcclusionCulling.gridChanged.Enqueue(cell);
    OccludeeState occludeeState = occludee;
    smartList.Remove(occludeeState);
    if (cell.staticBucket.Count == 0 && cell.dynamicBucket.Count == 0)
    {
      OcclusionCulling.grid.Remove(cell);
      cell.Reset();
    }
    occludee.cell = (OcclusionCulling.Cell) null;
  }

  public void UpdateGridBuffers()
  {
    if (OcclusionCulling.gridSet.CheckResize(OcclusionCulling.grid.Size, 256))
    {
      if (this.debugSettings.log)
        Debug.Log((object) ("[OcclusionCulling] Resized grid to " + (object) OcclusionCulling.grid.Size));
      for (int index = 0; index < OcclusionCulling.grid.Size; ++index)
      {
        if (OcclusionCulling.grid[index] != null)
          OcclusionCulling.gridChanged.Enqueue(OcclusionCulling.grid[index]);
      }
    }
    bool flag = OcclusionCulling.gridChanged.Count > 0;
    while (OcclusionCulling.gridChanged.Count > 0)
    {
      OcclusionCulling.Cell cell = OcclusionCulling.gridChanged.Dequeue();
      OcclusionCulling.gridSet.inputData[cell.hashedPoolIndex] = Color.op_Implicit(cell.sphereBounds);
    }
    if (!flag)
      return;
    OcclusionCulling.gridSet.UploadData();
  }

  public OcclusionCulling()
  {
    base.\u002Ector();
  }

  static OcclusionCulling()
  {
    // ISSUE: unable to decompile the method.
  }

  public class BufferSet
  {
    public Color[] inputData = new Color[0];
    public Color32[] resultData = new Color32[0];
    private Queue<AsyncGPUReadbackRequest> asyncRequests = new Queue<AsyncGPUReadbackRequest>();
    public IntPtr readbackInst = IntPtr.Zero;
    public ComputeBuffer inputBuffer;
    public ComputeBuffer resultBuffer;
    public int width;
    public int height;
    public int capacity;
    public int count;
    public Texture2D inputTexture;
    public RenderTexture resultTexture;
    public Texture2D resultReadTexture;
    private OcclusionCulling culling;
    private const int MaxAsyncGPUReadbackRequests = 10;

    public bool Ready
    {
      get
      {
        return (uint) this.resultData.Length > 0U;
      }
    }

    public void Attach(OcclusionCulling culling)
    {
      this.culling = culling;
    }

    public void Dispose(bool data = true)
    {
      if (this.inputBuffer != null)
      {
        this.inputBuffer.Dispose();
        this.inputBuffer = (ComputeBuffer) null;
      }
      if (this.resultBuffer != null)
      {
        this.resultBuffer.Dispose();
        this.resultBuffer = (ComputeBuffer) null;
      }
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
      if (Object.op_Inequality((Object) this.resultReadTexture, (Object) null))
      {
        Object.DestroyImmediate((Object) this.resultReadTexture);
        this.resultReadTexture = (Texture2D) null;
      }
      if (this.readbackInst != IntPtr.Zero)
      {
        Graphics.BufferReadback.Destroy(this.readbackInst);
        this.readbackInst = IntPtr.Zero;
      }
      if (!data)
        return;
      this.inputData = new Color[0];
      this.resultData = new Color32[0];
      this.capacity = 0;
      this.count = 0;
    }

    public bool CheckResize(int count, int granularity)
    {
      if (count <= this.capacity && (!this.culling.usePixelShaderFallback || !Object.op_Inequality((Object) this.resultTexture, (Object) null) || this.resultTexture.IsCreated()))
        return false;
      this.Dispose(false);
      int capacity = this.capacity;
      int num = count / granularity * granularity + granularity;
      if (this.culling.usePixelShaderFallback)
      {
        this.width = Mathf.CeilToInt(Mathf.Sqrt((float) num));
        this.height = Mathf.CeilToInt((float) num / (float) this.width);
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
        this.resultReadTexture = new Texture2D(this.width, this.height, (TextureFormat) 5, false, true);
        ((Object) this.resultReadTexture).set_name("_ResultRead");
        ((Texture) this.resultReadTexture).set_filterMode((FilterMode) 0);
        ((Texture) this.resultReadTexture).set_wrapMode((TextureWrapMode) 1);
        if (!this.culling.useAsyncReadAPI)
          this.readbackInst = Graphics.BufferReadback.CreateForTexture(((Texture) this.resultTexture).GetNativeTexturePtr(), (uint) this.width, (uint) this.height, (uint) this.resultTexture.get_format());
        this.capacity = this.width * this.height;
      }
      else
      {
        this.inputBuffer = new ComputeBuffer(num, 16);
        this.resultBuffer = new ComputeBuffer(num, 4);
        if (!this.culling.useAsyncReadAPI)
          this.readbackInst = Graphics.BufferReadback.CreateForBuffer(this.resultBuffer.GetNativeBufferPtr(), (uint) (this.capacity * 4));
        this.capacity = num;
      }
      Array.Resize<Color>(ref this.inputData, this.capacity);
      Array.Resize<Color32>(ref this.resultData, this.capacity);
      Color32 color32;
      ((Color32) ref color32).\u002Ector(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
      for (int index = capacity; index < this.capacity; ++index)
        this.resultData[index] = color32;
      this.count = count;
      return true;
    }

    public void UploadData()
    {
      if (this.culling.usePixelShaderFallback)
      {
        this.inputTexture.SetPixels(this.inputData);
        this.inputTexture.Apply();
      }
      else
        this.inputBuffer.SetData((Array) this.inputData);
    }

    private int AlignDispatchSize(int dispatchSize)
    {
      return (dispatchSize + 63) / 64;
    }

    public void Dispatch(int count)
    {
      if (this.culling.usePixelShaderFallback)
      {
        RenderBuffer activeColorBuffer = Graphics.get_activeColorBuffer();
        RenderBuffer activeDepthBuffer = Graphics.get_activeDepthBuffer();
        this.culling.fallbackMat.SetTexture("_Input", (Texture) this.inputTexture);
        Graphics.Blit((Texture) this.inputTexture, this.resultTexture, this.culling.fallbackMat, 0);
        RenderBuffer renderBuffer = activeDepthBuffer;
        Graphics.SetRenderTarget(activeColorBuffer, renderBuffer);
      }
      else
      {
        this.culling.computeShader.SetBuffer(0, "_Input", this.inputBuffer);
        this.culling.computeShader.SetBuffer(0, "_Result", this.resultBuffer);
        this.culling.computeShader.Dispatch(0, this.AlignDispatchSize(count), 1, 1);
      }
    }

    public void IssueRead()
    {
      if (OcclusionCulling.SafeMode)
        return;
      if (this.culling.useAsyncReadAPI)
      {
        if (this.asyncRequests.Count >= 10)
          return;
        this.asyncRequests.Enqueue(!this.culling.usePixelShaderFallback ? AsyncGPUReadback.Request(this.resultBuffer, (Action<AsyncGPUReadbackRequest>) null) : AsyncGPUReadback.Request((Texture) this.resultTexture, 0, (Action<AsyncGPUReadbackRequest>) null));
      }
      else
      {
        if (!(this.readbackInst != IntPtr.Zero))
          return;
        Graphics.BufferReadback.IssueRead(this.readbackInst);
      }
    }

    public void GetResults()
    {
      if (this.resultData == null || this.resultData.Length == 0)
        return;
      if (!OcclusionCulling.SafeMode)
      {
        if (this.culling.useAsyncReadAPI)
        {
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
        else
        {
          if (!(this.readbackInst != IntPtr.Zero))
            return;
          Graphics.BufferReadback.GetData(this.readbackInst, ref this.resultData[0]);
        }
      }
      else if (this.culling.usePixelShaderFallback)
      {
        RenderTexture.set_active(this.resultTexture);
        this.resultReadTexture.ReadPixels(new Rect(0.0f, 0.0f, (float) this.width, (float) this.height), 0, 0);
        this.resultReadTexture.Apply();
        Array.Copy((Array) this.resultReadTexture.GetPixels32(), (Array) this.resultData, this.resultData.Length);
      }
      else
        this.resultBuffer.GetData((Array) this.resultData);
    }
  }

  public delegate void OnVisibilityChanged(bool visible);

  public enum DebugFilter
  {
    Off,
    Dynamic,
    Static,
    Grid,
    All,
  }

  [System.Flags]
  public enum DebugMask
  {
    Off = 0,
    Dynamic = 1,
    Static = 2,
    Grid = 4,
    All = Grid | Static | Dynamic, // 0x00000007
  }

  [Serializable]
  public class DebugSettings
  {
    public LayerMask layerFilter = LayerMask.op_Implicit(-1);
    public bool log;
    public bool showAllVisible;
    public bool showMipChain;
    public bool showMain;
    public int showMainLod;
    public bool showFallback;
    public bool showStats;
    public bool showScreenBounds;
    public OcclusionCulling.DebugMask showMask;
  }

  public class HashedPoolValue
  {
    public ulong hashedPoolKey = ulong.MaxValue;
    public int hashedPoolIndex = -1;
  }

  public class HashedPool<ValueType> where ValueType : OcclusionCulling.HashedPoolValue, new()
  {
    private int granularity;
    private Dictionary<ulong, ValueType> dict;
    private List<ValueType> pool;
    private List<ValueType> list;
    private Queue<ValueType> recycled;

    public int Size
    {
      get
      {
        return this.list.Count;
      }
    }

    public int Count
    {
      get
      {
        return this.dict.Count;
      }
    }

    public ValueType this[int i]
    {
      get
      {
        return this.list[i];
      }
      set
      {
        this.list[i] = value;
      }
    }

    public HashedPool(int capacity, int granularity)
    {
      this.granularity = granularity;
      this.dict = new Dictionary<ulong, ValueType>(capacity);
      this.pool = new List<ValueType>(capacity);
      this.list = new List<ValueType>(capacity);
      this.recycled = new Queue<ValueType>();
    }

    public void Clear()
    {
      this.dict.Clear();
      this.pool.Clear();
      this.list.Clear();
      this.recycled.Clear();
    }

    public ValueType Add(ulong key, int capacityGranularity = 16)
    {
      ValueType valueType;
      if (this.recycled.Count > 0)
      {
        valueType = this.recycled.Dequeue();
        this.list[valueType.hashedPoolIndex] = valueType;
      }
      else
      {
        int count = this.pool.Count;
        if (count == this.pool.Capacity)
          this.pool.Capacity += this.granularity;
        valueType = new ValueType();
        valueType.hashedPoolIndex = count;
        this.pool.Add(valueType);
        this.list.Add(valueType);
      }
      valueType.hashedPoolKey = key;
      this.dict.Add(key, valueType);
      return valueType;
    }

    public void Remove(ValueType value)
    {
      this.dict.Remove(value.hashedPoolKey);
      this.list[value.hashedPoolIndex] = default (ValueType);
      this.recycled.Enqueue(value);
      value.hashedPoolKey = ulong.MaxValue;
    }

    public bool TryGetValue(ulong key, out ValueType value)
    {
      return this.dict.TryGetValue(key, out value);
    }

    public bool ContainsKey(ulong key)
    {
      return this.dict.ContainsKey(key);
    }
  }

  public class SimpleList<T>
  {
    private static readonly T[] emptyArray = new T[0];
    private const int defaultCapacity = 16;
    public T[] array;
    public int count;

    public int Count
    {
      get
      {
        return this.count;
      }
    }

    public int Capacity
    {
      get
      {
        return this.array.Length;
      }
      set
      {
        if (value == this.array.Length)
          return;
        if (value > 0)
        {
          T[] objArray = new T[value];
          if (this.count > 0)
            Array.Copy((Array) this.array, 0, (Array) objArray, 0, this.count);
          this.array = objArray;
        }
        else
          this.array = OcclusionCulling.SimpleList<T>.emptyArray;
      }
    }

    public T this[int index]
    {
      get
      {
        return this.array[index];
      }
      set
      {
        this.array[index] = value;
      }
    }

    public SimpleList()
    {
      this.array = OcclusionCulling.SimpleList<T>.emptyArray;
    }

    public SimpleList(int capacity)
    {
      this.array = capacity == 0 ? OcclusionCulling.SimpleList<T>.emptyArray : new T[capacity];
    }

    public void Add(T item)
    {
      if (this.count == this.array.Length)
        this.EnsureCapacity(this.count + 1);
      this.array[this.count++] = item;
    }

    public void Clear()
    {
      if (this.count <= 0)
        return;
      Array.Clear((Array) this.array, 0, this.count);
      this.count = 0;
    }

    public bool Contains(T item)
    {
      for (int index = 0; index < this.count; ++index)
      {
        if (this.array[index].Equals((object) item))
          return true;
      }
      return false;
    }

    public void CopyTo(T[] array)
    {
      Array.Copy((Array) this.array, 0, (Array) array, 0, this.count);
    }

    public void EnsureCapacity(int min)
    {
      if (this.array.Length >= min)
        return;
      int num = this.array.Length == 0 ? 16 : this.array.Length * 2;
      this.Capacity = num < min ? min : num;
    }
  }

  public class SmartListValue
  {
    public int hashedListIndex = -1;
  }

  public class SmartList
  {
    private static readonly OccludeeState[] emptyList = new OccludeeState[0];
    private static readonly int[] emptySlots = new int[0];
    private const int defaultCapacity = 16;
    private OccludeeState[] list;
    private int[] slots;
    private Queue<int> recycled;
    private int count;

    public OccludeeState[] List
    {
      get
      {
        return this.list;
      }
    }

    public int[] Slots
    {
      get
      {
        return this.slots;
      }
    }

    public int Size
    {
      get
      {
        return this.count;
      }
    }

    public int Count
    {
      get
      {
        return this.count - this.recycled.Count;
      }
    }

    public OccludeeState this[int i]
    {
      get
      {
        return this.list[i];
      }
      set
      {
        this.list[i] = value;
      }
    }

    public int Capacity
    {
      get
      {
        return this.list.Length;
      }
      set
      {
        if (value == this.list.Length)
          return;
        if (value > 0)
        {
          OccludeeState[] occludeeStateArray = new OccludeeState[value];
          int[] numArray = new int[value];
          if (this.count > 0)
          {
            Array.Copy((Array) this.list, (Array) occludeeStateArray, this.count);
            Array.Copy((Array) this.slots, (Array) numArray, this.count);
          }
          this.list = occludeeStateArray;
          this.slots = numArray;
        }
        else
        {
          this.list = OcclusionCulling.SmartList.emptyList;
          this.slots = OcclusionCulling.SmartList.emptySlots;
        }
      }
    }

    public SmartList(int capacity)
    {
      this.list = new OccludeeState[capacity];
      this.slots = new int[capacity];
      this.recycled = new Queue<int>();
      this.count = 0;
    }

    public void Add(OccludeeState value, int capacityGranularity = 16)
    {
      int index;
      if (this.recycled.Count > 0)
      {
        index = this.recycled.Dequeue();
        this.list[index] = value;
        this.slots[index] = value.slot;
      }
      else
      {
        index = this.count;
        if (index == this.list.Length)
          this.EnsureCapacity(this.count + 1);
        this.list[index] = value;
        this.slots[index] = value.slot;
        ++this.count;
      }
      value.hashedListIndex = index;
    }

    public void Remove(OccludeeState value)
    {
      int hashedListIndex = value.hashedListIndex;
      this.list[hashedListIndex] = (OccludeeState) null;
      this.slots[hashedListIndex] = -1;
      this.recycled.Enqueue(hashedListIndex);
      value.hashedListIndex = -1;
    }

    public bool Contains(OccludeeState value)
    {
      int hashedListIndex = value.hashedListIndex;
      if (hashedListIndex >= 0)
        return this.list[hashedListIndex] != null;
      return false;
    }

    public void EnsureCapacity(int min)
    {
      if (this.list.Length >= min)
        return;
      int num = this.list.Length == 0 ? 16 : this.list.Length * 2;
      this.Capacity = num < min ? min : num;
    }
  }

  [Serializable]
  public class Cell : OcclusionCulling.HashedPoolValue
  {
    public int x;
    public int y;
    public int z;
    public Bounds bounds;
    public Vector4 sphereBounds;
    public bool isVisible;
    public OcclusionCulling.SmartList staticBucket;
    public OcclusionCulling.SmartList dynamicBucket;

    public void Reset()
    {
      this.x = this.y = this.z = 0;
      this.bounds = (Bounds) null;
      this.sphereBounds = Vector4.get_zero();
      this.isVisible = true;
      this.staticBucket = (OcclusionCulling.SmartList) null;
      this.dynamicBucket = (OcclusionCulling.SmartList) null;
    }

    public OcclusionCulling.Cell Initialize(int x, int y, int z, Bounds bounds)
    {
      this.x = x;
      this.y = y;
      this.z = z;
      this.bounds = bounds;
      // ISSUE: variable of the null type
      __Null x1 = ((Bounds) ref bounds).get_center().x;
      // ISSUE: variable of the null type
      __Null y1 = ((Bounds) ref bounds).get_center().y;
      // ISSUE: variable of the null type
      __Null z1 = ((Bounds) ref bounds).get_center().z;
      Vector3 extents = ((Bounds) ref bounds).get_extents();
      double magnitude = (double) ((Vector3) ref extents).get_magnitude();
      this.sphereBounds = new Vector4((float) x1, (float) y1, (float) z1, (float) magnitude);
      this.isVisible = true;
      this.staticBucket = new OcclusionCulling.SmartList(32);
      this.dynamicBucket = new OcclusionCulling.SmartList(32);
      return this;
    }
  }

  public struct Sphere
  {
    public Vector3 position;
    public float radius;

    public bool IsValid()
    {
      return (double) this.radius > 0.0;
    }

    public Sphere(Vector3 position, float radius)
    {
      this.position = position;
      this.radius = radius;
    }
  }
}
