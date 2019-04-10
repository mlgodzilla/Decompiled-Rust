// Decompiled with JetBrains decompiler
// Type: EngineAudioClip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using JSON;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EngineAudioClip : MonoBehaviour, IClientComponent
{
  public AudioClip granularClip;
  public AudioClip accelerationClip;
  public TextAsset accelerationCyclesJson;
  public List<EngineAudioClip.EngineCycle> accelerationCycles;
  public List<EngineAudioClip.EngineCycleBucket> cycleBuckets;
  public Dictionary<int, EngineAudioClip.EngineCycleBucket> accelerationCyclesByRPM;
  public Dictionary<int, int> rpmBucketLookup;
  public int sampleRate;
  public int samplesUntilNextGrain;
  public int lastCycleId;
  public List<EngineAudioClip.Grain> grains;
  public int currentRPM;
  public int targetRPM;
  public int minRPM;
  public int maxRPM;
  public int cyclePadding;
  [Range(0.0f, 1f)]
  public float RPMControl;
  public AudioSource source;

  private int GetBucketRPM(int RPM)
  {
    return Mathf.RoundToInt((float) (RPM / 25)) * 25;
  }

  public EngineAudioClip()
  {
    base.\u002Ector();
  }

  [Serializable]
  public class EngineCycle
  {
    public int RPM;
    public int startSample;
    public int endSample;
    public float period;
    public int id;

    public EngineCycle(int RPM, int startSample, int endSample, float period, int id)
    {
      this.RPM = RPM;
      this.startSample = startSample;
      this.endSample = endSample;
      this.period = period;
      this.id = id;
    }
  }

  public class EngineCycleBucket
  {
    public List<EngineAudioClip.EngineCycle> cycles = new List<EngineAudioClip.EngineCycle>();
    public List<int> remainingCycles = new List<int>();
    public int RPM;

    public EngineCycleBucket(int RPM)
    {
      this.RPM = RPM;
    }

    public EngineAudioClip.EngineCycle GetCycle(Random random, int lastCycleId)
    {
      if (this.remainingCycles.Count == 0)
        this.ResetRemainingCycles(random);
      int index = (int) Extensions.Pop<int>((List<M0>) this.remainingCycles);
      if (this.cycles[index].id == lastCycleId)
      {
        if (this.remainingCycles.Count == 0)
          this.ResetRemainingCycles(random);
        index = (int) Extensions.Pop<int>((List<M0>) this.remainingCycles);
      }
      return this.cycles[index];
    }

    private void ResetRemainingCycles(Random random)
    {
      for (int index = 0; index < this.cycles.Count; ++index)
        this.remainingCycles.Add(index);
      ListEx.Shuffle<int>((List<M0>) this.remainingCycles, (uint) random.Next());
    }

    public void Add(EngineAudioClip.EngineCycle cycle)
    {
      if (this.cycles.Contains(cycle))
        return;
      this.cycles.Add(cycle);
    }
  }

  public class Grain
  {
    private float[] sourceData;
    private int startSample;
    private int currentSample;
    private int attackTimeSamples;
    private int sustainTimeSamples;
    private int releaseTimeSamples;
    private float gain;
    private float gainPerSampleAttack;
    private float gainPerSampleRelease;
    private int attackEndSample;
    private int releaseStartSample;
    private int endSample;

    public bool finished
    {
      get
      {
        return this.currentSample >= this.endSample;
      }
    }

    public void Init(float[] source, EngineAudioClip.EngineCycle cycle, int cyclePadding)
    {
      this.sourceData = source;
      this.startSample = cycle.startSample - cyclePadding;
      this.currentSample = this.startSample;
      this.attackTimeSamples = cyclePadding;
      this.sustainTimeSamples = cycle.endSample - cycle.startSample;
      this.releaseTimeSamples = cyclePadding;
      this.gainPerSampleAttack = 1f / (float) this.attackTimeSamples;
      this.gainPerSampleRelease = -1f / (float) this.releaseTimeSamples;
      this.attackEndSample = this.startSample + this.attackTimeSamples;
      this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
      this.endSample = this.releaseStartSample + this.releaseTimeSamples;
      this.gain = 0.0f;
    }

    public float GetSample()
    {
      if (this.currentSample >= this.sourceData.Length)
        return 0.0f;
      double num = (double) this.sourceData[this.currentSample];
      if (this.currentSample <= this.attackEndSample)
      {
        this.gain += this.gainPerSampleAttack;
        if ((double) this.gain > 0.800000011920929)
          this.gain = 0.8f;
      }
      else if (this.currentSample >= this.releaseStartSample)
      {
        this.gain += this.gainPerSampleRelease;
        if ((double) this.gain < 0.0)
          this.gain = 0.0f;
      }
      ++this.currentSample;
      double gain = (double) this.gain;
      return (float) (num * gain);
    }
  }
}
