// Decompiled with JetBrains decompiler
// Type: GranularAudioClip
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GranularAudioClip : MonoBehaviour
{
  public AudioClip sourceClip;
  private float[] sourceAudioData;
  private int sourceChannels;
  public AudioClip granularClip;
  public int sampleRate;
  public float sourceTime;
  public float sourceTimeVariation;
  public float grainAttack;
  public float grainSustain;
  public float grainRelease;
  public float grainFrequency;
  public int grainAttackSamples;
  public int grainSustainSamples;
  public int grainReleaseSamples;
  public int grainFrequencySamples;
  public int samplesUntilNextGrain;
  public List<GranularAudioClip.Grain> grains;
  private Random random;
  private bool inited;

  private void Update()
  {
    if (!this.inited && this.sourceClip.get_loadState() == 2)
    {
      this.sampleRate = this.sourceClip.get_frequency();
      this.sourceAudioData = new float[this.sourceClip.get_samples() * this.sourceClip.get_channels()];
      this.sourceClip.GetData(this.sourceAudioData, 0);
      this.InitAudioClip();
      M0 component = ((Component) this).GetComponent<AudioSource>();
      ((AudioSource) component).set_clip(this.granularClip);
      ((AudioSource) component).set_loop(true);
      ((AudioSource) component).Play();
      this.inited = true;
    }
    this.RefreshCachedData();
  }

  private void RefreshCachedData()
  {
    this.grainAttackSamples = Mathf.FloorToInt(this.grainAttack * (float) this.sampleRate * (float) this.sourceChannels);
    this.grainSustainSamples = Mathf.FloorToInt(this.grainSustain * (float) this.sampleRate * (float) this.sourceChannels);
    this.grainReleaseSamples = Mathf.FloorToInt(this.grainRelease * (float) this.sampleRate * (float) this.sourceChannels);
    this.grainFrequencySamples = Mathf.FloorToInt(this.grainFrequency * (float) this.sampleRate * (float) this.sourceChannels);
  }

  private void InitAudioClip()
  {
    int num1 = 1;
    int num2 = 1;
    AudioSettings.GetDSPBufferSize(ref num1, ref num2);
    // ISSUE: method pointer
    this.granularClip = AudioClip.Create(((Object) this.sourceClip).get_name() + " (granular)", num1, this.sourceClip.get_channels(), this.sampleRate, true, new AudioClip.PCMReaderCallback((object) this, __methodptr(OnAudioRead)));
    this.sourceChannels = this.sourceClip.get_channels();
  }

  private void OnAudioRead(float[] data)
  {
    for (int index1 = 0; index1 < data.Length; ++index1)
    {
      if (this.samplesUntilNextGrain <= 0)
        this.SpawnGrain();
      float num = 0.0f;
      for (int index2 = 0; index2 < this.grains.Count; ++index2)
        num += this.grains[index2].GetSample();
      data[index1] = num;
      --this.samplesUntilNextGrain;
    }
    this.CleanupFinishedGrains();
  }

  private void SpawnGrain()
  {
    if (this.grainFrequencySamples == 0)
      return;
    int start = Mathf.FloorToInt((this.sourceTime + ((float) (this.random.NextDouble() * (double) this.sourceTimeVariation * 2.0) - this.sourceTimeVariation)) * (float) this.sampleRate / (float) this.sourceChannels);
    GranularAudioClip.Grain grain = (GranularAudioClip.Grain) Pool.Get<GranularAudioClip.Grain>();
    grain.Init(this.sourceAudioData, start, this.grainAttackSamples, this.grainSustainSamples, this.grainReleaseSamples);
    this.grains.Add(grain);
    this.samplesUntilNextGrain = this.grainFrequencySamples;
  }

  private void CleanupFinishedGrains()
  {
    for (int index = this.grains.Count - 1; index >= 0; --index)
    {
      GranularAudioClip.Grain grain = this.grains[index];
      if (grain.finished)
      {
        // ISSUE: cast to a reference type
        Pool.Free<GranularAudioClip.Grain>((M0&) ref grain);
        this.grains.RemoveAt(index);
      }
    }
  }

  public GranularAudioClip()
  {
    base.\u002Ector();
  }

  public class Grain
  {
    private float[] sourceData;
    private int sourceDataLength;
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

    public void Init(float[] source, int start, int attack, int sustain, int release)
    {
      this.sourceData = source;
      this.sourceDataLength = this.sourceData.Length;
      this.startSample = start;
      this.currentSample = start;
      this.attackTimeSamples = attack;
      this.sustainTimeSamples = sustain;
      this.releaseTimeSamples = release;
      this.gainPerSampleAttack = 1f / (float) this.attackTimeSamples;
      this.gainPerSampleRelease = -1f / (float) this.releaseTimeSamples;
      this.attackEndSample = this.startSample + this.attackTimeSamples;
      this.releaseStartSample = this.attackEndSample + this.sustainTimeSamples;
      this.endSample = this.releaseStartSample + this.releaseTimeSamples;
      this.gain = 0.0f;
    }

    public float GetSample()
    {
      int index = this.currentSample % this.sourceDataLength;
      if (index < 0)
        index += this.sourceDataLength;
      double num = (double) this.sourceData[index];
      if (this.currentSample <= this.attackEndSample)
        this.gain += this.gainPerSampleAttack;
      else if (this.currentSample >= this.releaseStartSample)
        this.gain += this.gainPerSampleRelease;
      ++this.currentSample;
      double gain = (double) this.gain;
      return (float) (num * gain);
    }
  }
}
