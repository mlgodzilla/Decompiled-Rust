// Decompiled with JetBrains decompiler
// Type: Performance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class Performance : SingletonComponent<Performance>
{
  private static long cycles = 0;
  private static int[] frameRateHistory = new int[60];
  private static float[] frameTimeHistory = new float[60];
  public static Performance.Tick current;
  public static Performance.Tick report;
  private int frames;
  private float time;

  private void Update()
  {
    using (TimeWarning.New("FPSTimer", 0.1f))
      this.FPSTimer();
  }

  private void FPSTimer()
  {
    ++this.frames;
    this.time += Time.get_unscaledDeltaTime();
    if ((double) this.time < 1.0)
      return;
    Performance.current.frameRate = this.frames;
    Performance.current.frameTime = (float) ((double) this.time / (double) this.frames * 1000.0);
    Performance.frameRateHistory[Performance.cycles % (long) Performance.frameRateHistory.Length] = Performance.current.frameRate;
    Performance.frameTimeHistory[Performance.cycles % (long) Performance.frameTimeHistory.Length] = Performance.current.frameTime;
    Performance.current.frameRateAverage = this.AverageFrameRate();
    Performance.current.frameTimeAverage = this.AverageFrameTime();
    Performance.current.memoryUsageSystem = (long) SystemInfoEx.systemMemoryUsed;
    Performance.current.memoryAllocations = GC.GetTotalMemory(false) / 1048576L;
    Performance.current.memoryCollections = (long) GC.CollectionCount(0);
    Performance.current.loadBalancerTasks = (long) LoadBalancer.Count();
    Performance.current.invokeHandlerTasks = (long) InvokeHandler.Count();
    this.frames = 0;
    this.time = 0.0f;
    ++Performance.cycles;
    Performance.report = Performance.current;
  }

  private float AverageFrameRate()
  {
    float num = 0.0f;
    for (int index = 0; index < Performance.frameRateHistory.Length; ++index)
      num += (float) Performance.frameRateHistory[index];
    return num / (float) Performance.frameRateHistory.Length;
  }

  private float AverageFrameTime()
  {
    float num = 0.0f;
    for (int index = 0; index < Performance.frameTimeHistory.Length; ++index)
      num += Performance.frameTimeHistory[index];
    return num / (float) Performance.frameTimeHistory.Length;
  }

  public Performance()
  {
    base.\u002Ector();
  }

  public struct Tick
  {
    public int frameRate;
    public float frameTime;
    public float frameRateAverage;
    public float frameTimeAverage;
    public long memoryUsageSystem;
    public long memoryAllocations;
    public long memoryCollections;
    public long loadBalancerTasks;
    public long invokeHandlerTasks;
    public int ping;
  }
}
