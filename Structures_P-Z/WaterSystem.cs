// Decompiled with JetBrains decompiler
// Type: WaterSystem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaterSystem : MonoBehaviour
{
  public WaterQuality Quality;
  public bool ShowDebug;
  public bool ShowGizmos;
  public bool ProgressTime;
  public WaterSystem.SimulationSettings Simulation;
  public WaterSystem.RenderingSettings Rendering;
  [HideInInspector]
  public WaterGerstner.Wave[] GerstnerWaves;
  private static WaterSystem instance;

  public bool IsInitialized { private set; get; }

  public static WaterCollision Collision { get; private set; }

  public static WaterDynamics Dynamics { private set; get; }

  public static WaterBody Ocean { private set; get; } = (WaterBody) null;

  public static HashSet<WaterBody> WaterBodies { private set; get; } = new HashSet<WaterBody>();

  public static float OceanLevel { get; private set; } = 0.0f;

  public static float WaveTime { get; private set; } = 0.0f;

  public static WaterSystem Instance
  {
    get
    {
      return WaterSystem.instance;
    }
  }

  private void CheckInstance()
  {
    WaterSystem.instance = Object.op_Inequality((Object) WaterSystem.instance, (Object) null) ? WaterSystem.instance : this;
    WaterSystem.Collision = Object.op_Inequality((Object) WaterSystem.Collision, (Object) null) ? WaterSystem.Collision : (WaterCollision) ((Component) this).GetComponent<WaterCollision>();
    WaterSystem.Dynamics = Object.op_Inequality((Object) WaterSystem.Dynamics, (Object) null) ? WaterSystem.Dynamics : (WaterDynamics) ((Component) this).GetComponent<WaterDynamics>();
  }

  public void Awake()
  {
    this.CheckInstance();
  }

  public static float GetHeight(Vector3 pos)
  {
    float terrainHeight;
    return WaterSystem.GetHeight(pos, out terrainHeight);
  }

  public static float GetHeight(Vector3 pos, out float terrainHeight)
  {
    Vector2 posUV;
    posUV.x = (pos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
    posUV.y = (pos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
    return WaterSystem.GetHeight(pos, posUV, out terrainHeight);
  }

  public static float GetHeight(Vector3 pos, Vector2 posUV, out float terrainHeight)
  {
    float num1 = Object.op_Inequality((Object) TerrainMeta.WaterMap, (Object) null) ? TerrainMeta.WaterMap.GetHeightFast(posUV) : 0.0f;
    float num2 = Object.op_Inequality((Object) WaterSystem.Instance, (Object) null) ? (float) (double) WaterSystem.Ocean.Transform.get_position().y : 0.0f;
    terrainHeight = Object.op_Inequality((Object) TerrainMeta.HeightMap, (Object) null) ? TerrainMeta.HeightMap.GetHeight(pos) : 0.0f;
    if (Object.op_Inequality((Object) WaterSystem.instance, (Object) null) && WaterSystem.instance.GerstnerWaves != null && (double) num1 <= (double) num2 + 0.01)
    {
      float num3 = Mathf.Clamp01(Mathf.Abs(num2 - terrainHeight) * 0.1f);
      num1 = WaterGerstner.SampleHeight(WaterSystem.instance.GerstnerWaves, pos) * num3;
    }
    return num1;
  }

  public static void GetHeight(
    Vector2[] pos,
    Vector2[] posUV,
    float[] terrainHeight,
    float[] waterHeight)
  {
    Debug.Assert(pos.Length == posUV.Length);
    Debug.Assert(pos.Length == terrainHeight.Length);
    Debug.Assert(pos.Length == waterHeight.Length);
    float num1 = Object.op_Inequality((Object) WaterSystem.Instance, (Object) null) ? (float) (double) WaterSystem.Ocean.Transform.get_position().y : 0.0f;
    bool flag = Object.op_Inequality((Object) WaterSystem.instance, (Object) null) && WaterSystem.instance.GerstnerWaves != null;
    if (flag)
      WaterGerstner.SampleHeightArray(WaterSystem.instance.GerstnerWaves, pos, waterHeight);
    for (int index = 0; index < pos.Length; ++index)
    {
      Vector2 uv = posUV[index];
      terrainHeight[index] = Object.op_Inequality((Object) TerrainMeta.HeightMap, (Object) null) ? TerrainMeta.HeightMap.GetHeightFast(uv) : 0.0f;
      float num2 = Object.op_Inequality((Object) TerrainMeta.WaterMap, (Object) null) ? TerrainMeta.WaterMap.GetHeightFast(uv) : 0.0f;
      if (flag && (double) num2 <= (double) num1 + 0.01)
      {
        float num3 = Mathf.Clamp01(Mathf.Abs(num1 - terrainHeight[index]) * 0.1f);
        waterHeight[index] *= num3;
      }
      else
        waterHeight[index] = num2;
    }
  }

  public static Vector3 GetNormal(Vector3 pos)
  {
    Vector3 vector3 = Object.op_Inequality((Object) TerrainMeta.WaterMap, (Object) null) ? TerrainMeta.WaterMap.GetNormal(pos) : Vector3.get_up();
    return ((Vector3) ref vector3).get_normalized();
  }

  public void GenerateWaves()
  {
    this.GerstnerWaves = WaterGerstner.SetupWaves(this.Simulation.Wind, this.Simulation.GerstnerWaves);
  }

  public static void RegisterBody(WaterBody body)
  {
    if (body.Type == WaterBodyType.Ocean)
    {
      if (Object.op_Equality((Object) WaterSystem.Ocean, (Object) null))
      {
        WaterSystem.Ocean = body;
        WaterSystem.OceanLevel = (float) body.Transform.get_position().y;
      }
      else if (Object.op_Inequality((Object) WaterSystem.Ocean, (Object) body))
      {
        Debug.LogWarning((object) "[Water] Ocean body is already registered. Ignoring call because only one is allowed.");
        return;
      }
    }
    WaterSystem.WaterBodies.Add(body);
  }

  public static void UnregisterBody(WaterBody body)
  {
    WaterSystem.WaterBodies.Remove(body);
  }

  private void UpdateWaveTime()
  {
    WaterSystem.WaveTime = this.ProgressTime ? Time.get_realtimeSinceStartup() : WaterSystem.WaveTime;
  }

  private void Update()
  {
    this.UpdateWaveTime();
  }

  public WaterSystem()
  {
    base.\u002Ector();
  }

  public struct WaveSample
  {
    public Vector3 position;
    public Vector3 normal;
  }

  [Serializable]
  public class SimulationSettings
  {
    public Vector3 Wind = new Vector3(3f, 0.0f, 3f);
    public int SolverResolution = 64;
    public float SolverSizeInWorld = 18f;
    public float Gravity = 9.81f;
    public float Amplitude = 0.0001f;
    public TextAsset PerlinNoiseData;
    public WaterGerstner.WaveSettings GerstnerWaves;
  }

  [Serializable]
  public class RenderingSettings
  {
    public float MaxDisplacementDistance = 50f;
    public WaterSystem.RenderingSettings.SkyProbe SkyReflections;
    public WaterSystem.RenderingSettings.SSR ScreenSpaceReflections;
    public WaterSystem.RenderingSettings.Caustics CausticsAnimation;

    [Serializable]
    public class SkyProbe
    {
      public float ProbeUpdateInterval = 1f;
      public bool TimeSlicing = true;
    }

    [Serializable]
    public class SSR
    {
      public float FresnelCutoff = 0.02f;
      public float ThicknessMin = 1f;
      public float ThicknessMax = 20f;
      public float ThicknessStartDist = 40f;
      public float ThicknessEndDist = 100f;
    }

    [Serializable]
    public class Caustics
    {
      public float FrameRate = 15f;
      public Texture2D[] FramesShallow = new Texture2D[0];
      public Texture2D[] FramesDeep = new Texture2D[0];
    }
  }
}
