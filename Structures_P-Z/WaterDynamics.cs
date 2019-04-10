// Decompiled with JetBrains decompiler
// Type: WaterDynamics
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[ExecuteInEditMode]
public class WaterDynamics : MonoBehaviour
{
  private static HashSet<WaterInteraction> interactions = new HashSet<WaterInteraction>();
  public bool ForceFallback;
  private WaterDynamics.Target target;
  private bool useNativePath;
  private const int maxRasterSize = 1024;
  private const int subStep = 256;
  private const int subShift = 8;
  private const int subMask = 255;
  private const float oneOverSubStep = 0.00390625f;
  private const float interp_subStep = 65536f;
  private const int interp_subShift = 16;
  private const int interp_subFracMask = 65535;
  private WaterDynamics.ImageDesc imageDesc;
  private byte[] imagePixels;
  private WaterDynamics.TargetDesc targetDesc;
  private byte[] targetPixels;
  private byte[] targetDrawTileTable;
  private SimpleList<ushort> targetDrawTileList;
  public bool ShowDebug;

  public bool IsInitialized { private set; get; }

  public static void RegisterInteraction(WaterInteraction interaction)
  {
    WaterDynamics.interactions.Add(interaction);
  }

  public static void UnregisterInteraction(WaterInteraction interaction)
  {
    WaterDynamics.interactions.Remove(interaction);
  }

  private bool SupportsNativePath()
  {
    bool flag = true;
    try
    {
      WaterDynamics.ImageDesc desc = new WaterDynamics.ImageDesc();
      byte[] numArray = new byte[1];
      WaterDynamics.RasterBindImage_Native(ref desc, ref numArray[0]);
    }
    catch (EntryPointNotFoundException ex)
    {
      Debug.Log((object) "[WaterDynamics] Fast native path not available. Reverting to managed fallback.");
      flag = false;
    }
    return flag;
  }

  public void Initialize(Vector3 areaPosition, Vector3 areaSize)
  {
    this.target = new WaterDynamics.Target(this, areaPosition, areaSize);
    this.useNativePath = this.SupportsNativePath();
    this.IsInitialized = true;
  }

  public bool TryInitialize()
  {
    if (this.IsInitialized || !Object.op_Inequality((Object) TerrainMeta.Data, (Object) null))
      return false;
    this.Initialize(TerrainMeta.Position, TerrainMeta.Data.get_size());
    return true;
  }

  public void Shutdown()
  {
    if (this.target != null)
    {
      this.target.Destroy();
      this.target = (WaterDynamics.Target) null;
    }
    this.IsInitialized = false;
  }

  public void OnEnable()
  {
    this.TryInitialize();
  }

  public void OnDisable()
  {
    this.Shutdown();
  }

  public void Update()
  {
    if (Object.op_Equality((Object) WaterSystem.Instance, (Object) null) || this.IsInitialized)
      return;
    this.TryInitialize();
  }

  private void ProcessInteractions()
  {
    foreach (WaterInteraction interaction in WaterDynamics.interactions)
    {
      if (!Object.op_Equality((Object) interaction, (Object) null))
        interaction.UpdateTransform();
    }
  }

  public float SampleHeight(Vector3 pos)
  {
    return 0.0f;
  }

  private void RasterBindImage(WaterDynamics.Image image)
  {
    this.imageDesc = image.desc;
    this.imagePixels = image.pixels;
  }

  private void RasterBindTarget(WaterDynamics.Target target)
  {
    this.targetDesc = target.Desc;
    this.targetPixels = target.Pixels;
    this.targetDrawTileTable = target.DrawTileTable;
    this.targetDrawTileList = target.DrawTileList;
  }

  private void RasterInteraction(
    Vector2 pos,
    Vector2 scale,
    float rotation,
    float disp,
    float dist)
  {
    Vector2 raster = this.targetDesc.WorldToRaster(pos);
    double num1 = -(double) rotation * (Math.PI / 180.0);
    float s = Mathf.Sin((float) num1);
    float c = Mathf.Cos((float) num1);
    float num2 = Mathf.Min((float) this.imageDesc.width * (float) scale.x, 1024f) * 0.5f;
    float num3 = Mathf.Min((float) this.imageDesc.height * (float) scale.y, 1024f) * 0.5f;
    Vector2 vector2_1 = Vector2.op_Addition(raster, this.Rotate2D(new Vector2(-num2, -num3), s, c));
    Vector2 vector2_2 = Vector2.op_Addition(raster, this.Rotate2D(new Vector2(num2, -num3), s, c));
    Vector2 vector2_3 = Vector2.op_Addition(raster, this.Rotate2D(new Vector2(num2, num3), s, c));
    Vector2 vector2_4 = Vector2.op_Addition(raster, this.Rotate2D(new Vector2(-num2, num3), s, c));
    WaterDynamics.Point2D p0 = new WaterDynamics.Point2D((float) (vector2_1.x * 256.0), (float) (vector2_1.y * 256.0));
    WaterDynamics.Point2D p1 = new WaterDynamics.Point2D((float) (vector2_2.x * 256.0), (float) (vector2_2.y * 256.0));
    WaterDynamics.Point2D point2D = new WaterDynamics.Point2D((float) (vector2_3.x * 256.0), (float) (vector2_3.y * 256.0));
    WaterDynamics.Point2D p2 = new WaterDynamics.Point2D((float) (vector2_4.x * 256.0), (float) (vector2_4.y * 256.0));
    Vector2 uv0;
    ((Vector2) ref uv0).\u002Ector(-0.5f, -0.5f);
    Vector2 uv1;
    ((Vector2) ref uv1).\u002Ector((float) this.imageDesc.width - 0.5f, -0.5f);
    Vector2 vector2_5;
    ((Vector2) ref vector2_5).\u002Ector((float) this.imageDesc.width - 0.5f, (float) this.imageDesc.height - 0.5f);
    Vector2 uv2;
    ((Vector2) ref uv2).\u002Ector(-0.5f, (float) this.imageDesc.height - 0.5f);
    byte disp1 = (byte) ((double) disp * (double) byte.MaxValue);
    byte dist1 = (byte) ((double) dist * (double) byte.MaxValue);
    this.RasterizeTriangle(p0, p1, point2D, uv0, uv1, vector2_5, disp1, dist1);
    this.RasterizeTriangle(p0, point2D, p2, uv0, vector2_5, uv2, disp1, dist1);
  }

  private float Frac(float x)
  {
    return x - (float) (int) x;
  }

  private Vector2 Rotate2D(Vector2 v, float s, float c)
  {
    Vector2 vector2;
    vector2.x = (__Null) (v.x * (double) c - v.y * (double) s);
    vector2.y = (__Null) (v.y * (double) c + v.x * (double) s);
    return vector2;
  }

  private int Min3(int a, int b, int c)
  {
    return Mathf.Min(a, Mathf.Min(b, c));
  }

  private int Max3(int a, int b, int c)
  {
    return Mathf.Max(a, Mathf.Max(b, c));
  }

  private int EdgeFunction(
    WaterDynamics.Point2D a,
    WaterDynamics.Point2D b,
    WaterDynamics.Point2D c)
  {
    return (int) (((long) (b.x - a.x) * (long) (c.y - a.y) >> 8) - ((long) (b.y - a.y) * (long) (c.x - a.x) >> 8));
  }

  private bool IsTopLeft(WaterDynamics.Point2D a, WaterDynamics.Point2D b)
  {
    if (a.y != b.y || a.x >= b.x)
      return a.y > b.y;
    return true;
  }

  private void RasterizeTriangle(
    WaterDynamics.Point2D p0,
    WaterDynamics.Point2D p1,
    WaterDynamics.Point2D p2,
    Vector2 uv0,
    Vector2 uv1,
    Vector2 uv2,
    byte disp,
    byte dist)
  {
    int width = this.imageDesc.width;
    int widthShift = this.imageDesc.widthShift;
    int maxWidth = this.imageDesc.maxWidth;
    int maxHeight = this.imageDesc.maxHeight;
    int size = this.targetDesc.size;
    int tileCount = this.targetDesc.tileCount;
    int num1 = Mathf.Max(this.Min3(p0.x, p1.x, p2.x), 0);
    int num2 = Mathf.Max(this.Min3(p0.y, p1.y, p2.y), 0);
    int num3 = Mathf.Min(this.Max3(p0.x, p1.x, p2.x), this.targetDesc.maxSizeSubStep);
    int num4 = Mathf.Min(this.Max3(p0.y, p1.y, p2.y), this.targetDesc.maxSizeSubStep);
    int num5 = Mathf.Max(num1 >> 8 >> this.targetDesc.tileSizeShift, 0);
    int num6 = Mathf.Min(num3 >> 8 >> this.targetDesc.tileSizeShift, this.targetDesc.tileMaxCount);
    int num7 = Mathf.Max(num2 >> 8 >> this.targetDesc.tileSizeShift, 0);
    int num8 = Mathf.Min(num4 >> 8 >> this.targetDesc.tileSizeShift, this.targetDesc.tileMaxCount);
    for (int index1 = num7; index1 <= num8; ++index1)
    {
      int num9 = index1 * tileCount;
      for (int index2 = num5; index2 <= num6; ++index2)
      {
        int index3 = num9 + index2;
        if (this.targetDrawTileTable[index3] == (byte) 0)
        {
          this.targetDrawTileTable[index3] = (byte) 1;
          this.targetDrawTileList.Add((ushort) index3);
        }
      }
    }
    int x = num1 + (int) byte.MaxValue & -256;
    int y = num2 + (int) byte.MaxValue & -256;
    int num10 = this.IsTopLeft(p1, p2) ? 0 : -1;
    int num11 = this.IsTopLeft(p2, p0) ? 0 : -1;
    int num12 = this.IsTopLeft(p0, p1) ? 0 : -1;
    WaterDynamics.Point2D c = new WaterDynamics.Point2D(x, y);
    int num13 = this.EdgeFunction(p1, p2, c) + num10;
    int num14 = this.EdgeFunction(p2, p0, c) + num11;
    int num15 = this.EdgeFunction(p0, p1, c) + num12;
    int num16 = p1.y - p2.y;
    int num17 = p2.y - p0.y;
    int num18 = p0.y - p1.y;
    int num19 = p2.x - p1.x;
    int num20 = p0.x - p2.x;
    int num21 = p1.x - p0.x;
    float num22 = 1.677722E+07f / (float) this.EdgeFunction(p0, p1, p2);
    float num23 = (float) (uv0.x * 65536.0);
    float num24 = (float) (uv0.y * 65536.0);
    float num25 = (float) (uv1.x - uv0.x) * num22;
    float num26 = (float) (uv1.y - uv0.y) * num22;
    float num27 = (float) (uv2.x - uv0.x) * num22;
    float num28 = (float) (uv2.y - uv0.y) * num22;
    int num29 = (int) ((double) num17 * (1.0 / 256.0) * (double) num25 + (double) num18 * (1.0 / 256.0) * (double) num27);
    int num30 = (int) ((double) num17 * (1.0 / 256.0) * (double) num26 + (double) num18 * (1.0 / 256.0) * (double) num28);
    for (int index1 = y; index1 <= num4; index1 += 256)
    {
      int num9 = num13;
      int num31 = num14;
      int num32 = num15;
      int num33 = (int) ((double) num23 + (double) num25 * (1.0 / 256.0) * (double) num31 + (double) num27 * (1.0 / 256.0) * (double) num32);
      int num34 = (int) ((double) num24 + (double) num26 * (1.0 / 256.0) * (double) num31 + (double) num28 * (1.0 / 256.0) * (double) num32);
      for (int index2 = x; index2 <= num3; index2 += 256)
      {
        if ((num9 | num31 | num32) >= 0)
        {
          int num35 = num33 > 0 ? num33 : 0;
          int num36 = num34 > 0 ? num34 : 0;
          int num37 = num35 >> 16;
          int num38 = num36 >> 16;
          byte num39 = (byte) ((num35 & (int) ushort.MaxValue) >> 8);
          byte num40 = (byte) ((num36 & (int) ushort.MaxValue) >> 8);
          int num41 = num37 > 0 ? num37 : 0;
          int num42 = num38 > 0 ? num38 : 0;
          int num43 = num41 < maxWidth ? num41 : maxWidth;
          int num44 = num42 < maxHeight ? num42 : maxHeight;
          int num45 = num43 < maxWidth ? 1 : 0;
          int num46 = num44 < maxHeight ? width : 0;
          int index3 = (num44 << widthShift) + num43;
          int index4 = index3 + num45;
          int index5 = index3 + num46;
          int index6 = index5 + num45;
          byte imagePixel1 = this.imagePixels[index3];
          byte imagePixel2 = this.imagePixels[index4];
          byte imagePixel3 = this.imagePixels[index5];
          byte imagePixel4 = this.imagePixels[index6];
          int num47 = (int) imagePixel1 + ((int) num39 * ((int) imagePixel2 - (int) imagePixel1) >> 8);
          int num48 = (int) imagePixel3 + ((int) num39 * ((int) imagePixel4 - (int) imagePixel3) >> 8);
          int num49 = (num47 + ((int) num40 * (num48 - num47) >> 8)) * (int) disp >> 8;
          int index7 = (index1 >> 8) * size + (index2 >> 8);
          int num50 = (int) this.targetPixels[index7] + num49;
          int num51 = num50 < (int) byte.MaxValue ? num50 : (int) byte.MaxValue;
          this.targetPixels[index7] = (byte) num51;
        }
        num9 += num16;
        num31 += num17;
        num32 += num18;
        num33 += num29;
        num34 += num30;
      }
      num13 += num19;
      num14 += num20;
      num15 += num21;
    }
  }

  [DllImport("RustNative", EntryPoint = "Water_RasterClearTile")]
  private static extern void RasterClearTile_Native(
    ref byte pixels,
    int offset,
    int stride,
    int width,
    int height);

  [DllImport("RustNative", EntryPoint = "Water_RasterBindImage")]
  private static extern void RasterBindImage_Native(
    ref WaterDynamics.ImageDesc desc,
    ref byte pixels);

  [DllImport("RustNative", EntryPoint = "Water_RasterBindTarget")]
  private static extern void RasterBindTarget_Native(
    ref WaterDynamics.TargetDesc desc,
    ref byte pixels,
    ref byte drawTileTable,
    ref ushort drawTileList,
    ref int drawTileCount);

  [DllImport("RustNative", EntryPoint = "Water_RasterInteraction")]
  private static extern void RasterInteraction_Native(
    Vector2 pos,
    Vector2 scale,
    float rotation,
    float disp,
    float dist);

  public static void SafeDestroy<T>(ref T obj) where T : Object
  {
    if (!Object.op_Inequality((Object) (object) obj, (Object) null))
      return;
    Object.DestroyImmediate((Object) (object) obj);
    obj = default (T);
  }

  public static T SafeDestroy<T>(T obj) where T : Object
  {
    if (Object.op_Inequality((Object) (object) obj, (Object) null))
      Object.DestroyImmediate((Object) (object) obj);
    return default (T);
  }

  public static void SafeRelease<T>(ref T obj) where T : class, IDisposable
  {
    if ((object) obj == null)
      return;
    obj.Dispose();
    obj = default (T);
  }

  public static T SafeRelease<T>(T obj) where T : class, IDisposable
  {
    if ((object) obj != null)
      obj.Dispose();
    return default (T);
  }

  public WaterDynamics()
  {
    base.\u002Ector();
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct ImageDesc
  {
    public int width;
    public int height;
    public int maxWidth;
    public int maxHeight;
    public int widthShift;

    public ImageDesc(Texture2D tex)
    {
      this.width = ((Texture) tex).get_width();
      this.height = ((Texture) tex).get_height();
      this.maxWidth = ((Texture) tex).get_width() - 1;
      this.maxHeight = ((Texture) tex).get_height() - 1;
      this.widthShift = (int) Mathf.Log((float) ((Texture) tex).get_width(), 2f);
    }

    public void Clear()
    {
      this.width = 0;
      this.height = 0;
      this.maxWidth = 0;
      this.maxHeight = 0;
      this.widthShift = 0;
    }
  }

  public class Image
  {
    public WaterDynamics.ImageDesc desc;
    public byte[] pixels;

    public Texture2D texture { get; private set; }

    public Image(Texture2D tex)
    {
      this.desc = new WaterDynamics.ImageDesc(tex);
      this.texture = tex;
      this.pixels = this.GetDisplacementPixelsFromTexture(tex);
    }

    public void Destroy()
    {
      this.desc.Clear();
      this.texture = (Texture2D) null;
      this.pixels = (byte[]) null;
    }

    private byte[] GetDisplacementPixelsFromTexture(Texture2D tex)
    {
      Color32[] pixels32 = tex.GetPixels32();
      byte[] numArray = new byte[pixels32.Length];
      for (int index = 0; index < pixels32.Length; ++index)
        numArray[index] = (byte) pixels32[index].b;
      return numArray;
    }
  }

  private struct Point2D
  {
    public int x;
    public int y;

    public Point2D(int x, int y)
    {
      this.x = x;
      this.y = y;
    }

    public Point2D(float x, float y)
    {
      this.x = (int) x;
      this.y = (int) y;
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct TargetDesc
  {
    public int size;
    public int maxSize;
    public int maxSizeSubStep;
    public Vector2 areaOffset;
    public Vector2 areaToMapUV;
    public Vector2 areaToMapXY;
    public int tileSize;
    public int tileSizeShift;
    public int tileCount;
    public int tileMaxCount;

    public TargetDesc(Vector3 areaPosition, Vector3 areaSize)
    {
      this.size = 512;
      this.maxSize = this.size - 1;
      this.maxSizeSubStep = this.maxSize * 256;
      this.areaOffset = new Vector2((float) areaPosition.x, (float) areaPosition.z);
      this.areaToMapUV = new Vector2((float) (1.0 / areaSize.x), (float) (1.0 / areaSize.z));
      this.areaToMapXY = Vector2.op_Multiply(this.areaToMapUV, (float) this.size);
      this.tileSize = Mathf.NextPowerOfTwo(Mathf.Max(this.size, 4096)) / 256;
      this.tileSizeShift = (int) Mathf.Log((float) this.tileSize, 2f);
      this.tileCount = Mathf.CeilToInt((float) this.size / (float) this.tileSize);
      this.tileMaxCount = this.tileCount - 1;
    }

    public void Clear()
    {
      this.areaOffset = Vector2.get_zero();
      this.areaToMapUV = Vector2.get_zero();
      this.areaToMapXY = Vector2.get_zero();
      this.size = 0;
      this.maxSize = 0;
      this.maxSizeSubStep = 0;
      this.tileSize = 0;
      this.tileSizeShift = 0;
      this.tileCount = 0;
      this.tileMaxCount = 0;
    }

    public ushort TileOffsetToXYOffset(ushort tileOffset, out int x, out int y, out int offset)
    {
      int num1 = (int) tileOffset % this.tileCount;
      int num2 = (int) tileOffset / this.tileCount;
      x = num1 * this.tileSize;
      y = num2 * this.tileSize;
      offset = y * this.size + x;
      return tileOffset;
    }

    public ushort TileOffsetToTileXYIndex(
      ushort tileOffset,
      out int tileX,
      out int tileY,
      out ushort tileIndex)
    {
      tileX = (int) tileOffset % this.tileCount;
      tileY = (int) tileOffset / this.tileCount;
      tileIndex = (ushort) (tileY * this.tileCount + tileX);
      return tileOffset;
    }

    public Vector2 WorldToRaster(Vector2 pos)
    {
      Vector2 vector2;
      vector2.x = (pos.x - this.areaOffset.x) * this.areaToMapXY.x;
      vector2.y = (pos.y - this.areaOffset.y) * this.areaToMapXY.y;
      return vector2;
    }

    public Vector3 WorldToRaster(Vector3 pos)
    {
      Vector2 vector2;
      vector2.x = (pos.x - this.areaOffset.x) * this.areaToMapXY.x;
      vector2.y = (pos.z - this.areaOffset.y) * this.areaToMapXY.y;
      return Vector2.op_Implicit(vector2);
    }
  }

  public class Target
  {
    public WaterDynamics owner;
    public WaterDynamics.TargetDesc desc;
    private byte[] pixels;
    private byte[] clearTileTable;
    private SimpleList<ushort> clearTileList;
    private byte[] drawTileTable;
    private SimpleList<ushort> drawTileList;
    private const int MaxInteractionOffset = 100;
    private Vector3 prevCameraWorldPos;
    private Vector2i interactionOffset;

    public WaterDynamics.TargetDesc Desc
    {
      get
      {
        return this.desc;
      }
    }

    public byte[] Pixels
    {
      get
      {
        return this.pixels;
      }
    }

    public byte[] DrawTileTable
    {
      get
      {
        return this.drawTileTable;
      }
    }

    public SimpleList<ushort> DrawTileList
    {
      get
      {
        return this.drawTileList;
      }
    }

    public Target(WaterDynamics owner, Vector3 areaPosition, Vector3 areaSize)
    {
      this.owner = owner;
      this.desc = new WaterDynamics.TargetDesc(areaPosition, areaSize);
    }

    public void Destroy()
    {
      this.desc.Clear();
    }

    private Texture2D CreateDynamicTexture(int size)
    {
      Texture2D texture2D = new Texture2D(size, size, (TextureFormat) 5, false, true);
      ((Texture) texture2D).set_filterMode((FilterMode) 1);
      ((Texture) texture2D).set_wrapMode((TextureWrapMode) 1);
      return texture2D;
    }

    private RenderTexture CreateRenderTexture(int size)
    {
      RenderTextureFormat renderTextureFormat = SystemInfoEx.SupportsRenderTextureFormat((RenderTextureFormat) 15) ? (RenderTextureFormat) 15 : (RenderTextureFormat) 14;
      RenderTexture renderTexture = new RenderTexture(size, size, 0, renderTextureFormat, (RenderTextureReadWrite) 1);
      ((Texture) renderTexture).set_filterMode((FilterMode) 1);
      ((Texture) renderTexture).set_wrapMode((TextureWrapMode) 1);
      renderTexture.Create();
      return renderTexture;
    }

    public void ClearTiles()
    {
      for (int index1 = 0; index1 < this.clearTileList.Count; ++index1)
      {
        int x;
        int y;
        int offset;
        int xyOffset = (int) this.desc.TileOffsetToXYOffset(this.clearTileList[index1], out x, out y, out offset);
        int num = Mathf.Min(x + this.desc.tileSize, this.desc.size) - x;
        int height = Mathf.Min(y + this.desc.tileSize, this.desc.size) - y;
        if (this.owner.useNativePath)
        {
          WaterDynamics.RasterClearTile_Native(ref this.pixels[0], offset, this.desc.size, num, height);
        }
        else
        {
          for (int index2 = 0; index2 < height; ++index2)
          {
            Array.Clear((Array) this.pixels, offset, num);
            offset += this.desc.size;
          }
        }
      }
    }

    public void ProcessTiles()
    {
      int tileX;
      int tileY;
      ushort tileIndex;
      for (int index = 0; index < this.clearTileList.Count; ++index)
      {
        this.clearTileTable[(int) this.desc.TileOffsetToTileXYIndex(this.clearTileList[index], out tileX, out tileY, out tileIndex)] = (byte) 0;
        this.clearTileList[index] = ushort.MaxValue;
      }
      this.clearTileList.Clear();
      for (int index = 0; index < this.drawTileList.Count; ++index)
      {
        ushort tileXyIndex = this.desc.TileOffsetToTileXYIndex(this.drawTileList[index], out tileX, out tileY, out tileIndex);
        if (this.clearTileTable[(int) tileIndex] == (byte) 0)
        {
          this.clearTileTable[(int) tileIndex] = (byte) 1;
          this.clearTileList.Add(tileIndex);
        }
        this.drawTileTable[(int) tileXyIndex] = (byte) 0;
        this.drawTileList[index] = ushort.MaxValue;
      }
      this.drawTileList.Clear();
    }

    public void UpdateTiles()
    {
    }

    public void Prepare()
    {
    }

    public void Update()
    {
    }

    public void UpdateGlobalShaderProperties()
    {
    }
  }
}
