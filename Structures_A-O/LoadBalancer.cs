// Decompiled with JetBrains decompiler
// Type: LoadBalancer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Rust;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LoadBalancer : SingletonComponent<LoadBalancer>
{
  public static bool Paused;
  private const float MinMilliseconds = 1f;
  private const float MaxMilliseconds = 100f;
  private const int MinBacklog = 1000;
  private const int MaxBacklog = 100000;
  private Queue<DeferredAction>[] queues;
  private Stopwatch watch;

  protected void LateUpdate()
  {
    if (Application.isReceiving != null || Application.isLoading != null || LoadBalancer.Paused)
      return;
    float num = Mathf.SmoothStep(1f, 100f, Mathf.InverseLerp(1000f, 100000f, (float) LoadBalancer.Count()));
    this.watch.Reset();
    this.watch.Start();
    for (int index = 0; index < this.queues.Length; ++index)
    {
      Queue<DeferredAction> queue = this.queues[index];
      while (queue.Count > 0)
      {
        queue.Dequeue().Action();
        if (this.watch.Elapsed.TotalMilliseconds > (double) num)
          return;
      }
    }
  }

  public static int Count()
  {
    if (!Object.op_Implicit((Object) SingletonComponent<LoadBalancer>.Instance))
      return 0;
    Queue<DeferredAction>[] queues = ((LoadBalancer) SingletonComponent<LoadBalancer>.Instance).queues;
    int num = 0;
    for (int index = 0; index < queues.Length; ++index)
      num += queues[index].Count;
    return num;
  }

  public static void ProcessAll()
  {
    if (!Object.op_Implicit((Object) SingletonComponent<LoadBalancer>.Instance))
      LoadBalancer.CreateInstance();
    foreach (Queue<DeferredAction> queue in ((LoadBalancer) SingletonComponent<LoadBalancer>.Instance).queues)
    {
      while (queue.Count > 0)
        queue.Dequeue().Action();
    }
  }

  public static void Enqueue(DeferredAction action)
  {
    if (!Object.op_Implicit((Object) SingletonComponent<LoadBalancer>.Instance))
      LoadBalancer.CreateInstance();
    ((LoadBalancer) SingletonComponent<LoadBalancer>.Instance).queues[action.Index].Enqueue(action);
  }

  private static void CreateInstance()
  {
    GameObject gameObject = new GameObject();
    ((Object) gameObject).set_name(nameof (LoadBalancer));
    gameObject.AddComponent<LoadBalancer>();
    Object.DontDestroyOnLoad((Object) gameObject);
  }

  public LoadBalancer()
  {
    base.\u002Ector();
  }
}
