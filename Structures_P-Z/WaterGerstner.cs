// Decompiled with JetBrains decompiler
// Type: WaterGerstner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class WaterGerstner
{
  public static WaterGerstner.Wave[] SetupWaves(
    Vector3 wind,
    WaterGerstner.WaveSettings settings)
  {
    Random.State state = Random.get_state();
    Random.InitState(settings.RandomSeed);
    int waveCount = settings.WaveCount;
    float num1 = Mathf.Atan2((float) wind.z, (float) wind.x);
    float num2 = (float) (1 / waveCount);
    float amplitude1 = settings.Amplitude;
    float length1 = settings.Length;
    float steepness1 = settings.Steepness;
    WaterGerstner.Wave[] waveArray = new WaterGerstner.Wave[waveCount];
    for (int index = 0; index < waveCount; ++index)
    {
      float num3 = Mathf.Lerp(0.5f, 1.5f, (float) index * num2);
      float num4 = num1 + (float) Math.PI / 180f * Random.Range(-settings.AngleSpread, settings.AngleSpread);
      Vector2 direction;
      ((Vector2) ref direction).\u002Ector(-Mathf.Cos(num4), -Mathf.Sin(num4));
      float amplitude2 = amplitude1 * num3 * Random.Range(0.8f, 1.2f);
      float length2 = length1 * num3 * Random.Range(0.6f, 1.4f);
      float steepness2 = Mathf.Clamp01(steepness1 * num3 * Random.Range(0.6f, 1.4f));
      waveArray[index] = new WaterGerstner.Wave(waveCount, direction, amplitude2, length2, steepness2);
      Random.InitState(settings.RandomSeed + index + 1);
    }
    Random.set_state(state);
    return waveArray;
  }

  public static void SampleWaves(
    WaterGerstner.Wave[] waves,
    Vector3 location,
    out Vector3 position,
    out Vector3 normal)
  {
    Vector2 vector2;
    ((Vector2) ref vector2).\u002Ector((float) location.x, (float) location.z);
    float waveTime = WaterSystem.WaveTime;
    Vector3 zero1 = Vector3.get_zero();
    Vector3 zero2 = Vector3.get_zero();
    for (uint index = 0; (long) index < (long) waves.Length; ++index)
    {
      double wi = (double) waves[(int) index].wi;
      float phi = waves[(int) index].phi;
      float wa = waves[(int) index].WA;
      Vector2 di = waves[(int) index].Di;
      float ai = waves[(int) index].Ai;
      float qi = waves[(int) index].Qi;
      double num1 = (double) Vector2.Dot(di, vector2);
      double num2 = wi * num1 + (double) phi * (double) waveTime;
      float num3 = Mathf.Sin((float) num2);
      float num4 = Mathf.Cos((float) num2);
      ref __Null local1 = ref zero1.x;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + (float) ((double) qi * (double) ai * di.x) * num4;
      ref __Null local2 = ref zero1.y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 + (float) ((double) qi * (double) ai * di.y) * num4;
      ref __Null local3 = ref zero1.z;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local3 = ^(float&) ref local3 + ai * num3;
      ref __Null local4 = ref zero2.x;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local4 = ^(float&) ref local4 + (float) di.x * wa * num4;
      ref __Null local5 = ref zero2.y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local5 = ^(float&) ref local5 + (float) di.y * wa * num4;
      ref __Null local6 = ref zero2.z;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local6 = ^(float&) ref local6 + qi * wa * num3;
    }
    position = new Vector3((float) zero1.x, (float) zero1.z, (float) zero1.y);
    normal = new Vector3((float) -zero2.x, (float) (1.0 - zero2.z), (float) -zero2.y);
  }

  public static float SampleHeight(WaterGerstner.Wave[] waves, Vector3 location)
  {
    Vector2 vector2;
    ((Vector2) ref vector2).\u002Ector((float) location.x, (float) location.z);
    float waveTime = WaterSystem.WaveTime;
    float num1 = 0.0f;
    for (uint index = 0; (long) index < (long) waves.Length; ++index)
    {
      double wi = (double) waves[(int) index].wi;
      float phi = waves[(int) index].phi;
      Vector2 di = waves[(int) index].Di;
      float ai = waves[(int) index].Ai;
      double num2 = (double) Vector2.Dot(di, vector2);
      float num3 = Mathf.Sin((float) (wi * num2 + (double) phi * (double) waveTime));
      num1 += ai * num3;
    }
    return num1;
  }

  public static void SampleHeightArray(
    WaterGerstner.Wave[] waves,
    Vector2[] location,
    float[] height)
  {
    Debug.Assert(location.Length == height.Length);
    float waveTime = WaterSystem.WaveTime;
    for (uint index1 = 0; (long) index1 < (long) waves.Length; ++index1)
    {
      float wi = waves[(int) index1].wi;
      float phi = waves[(int) index1].phi;
      Vector2 di = waves[(int) index1].Di;
      float ai = waves[(int) index1].Ai;
      for (int index2 = 0; index2 < location.Length; ++index2)
      {
        float num1 = Mathf.Sin((float) ((double) wi * (di.x * location[index2].x + di.y * location[index2].y) + (double) phi * (double) waveTime));
        float num2 = ai * num1;
        height[index2] = index1 > 0U ? height[index2] + num2 : num2;
      }
    }
  }

  [Serializable]
  public class WaveSettings
  {
    [Range(1f, 8f)]
    public int WaveCount = 6;
    public float Amplitude = 0.33f;
    public float Length = 10f;
    public float AngleSpread = 45f;
    [NonSerialized]
    public float Steepness = 1f;
    public int RandomSeed = 1234;
  }

  [Serializable]
  public struct Wave
  {
    private const float MaxFrequency = 5f;
    public float wi;
    public float phi;
    public float WA;
    public Vector2 Di;
    public float Ai;
    public float Qi;

    public Wave(int waveCount, Vector2 direction, float amplitude, float length, float steepness)
    {
      this.wi = 2f / length;
      this.phi = Mathf.Min(5f, Mathf.Sqrt(30.81903f * this.wi)) * this.wi;
      this.WA = this.wi * amplitude;
      this.Di = direction;
      this.Ai = amplitude;
      this.Qi = steepness / (this.WA * (float) waveCount);
    }
  }
}
