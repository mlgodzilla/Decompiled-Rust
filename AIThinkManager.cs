// Decompiled with JetBrains decompiler
// Type: AIThinkManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class AIThinkManager : BaseMonoBehaviour
{
  public static ListHashSet<IThinker> _processQueue = new ListHashSet<IThinker>(8);
  public static ListHashSet<IThinker> _removalQueue = new ListHashSet<IThinker>(8);
  [Help("How many miliseconds to budget for processing AI entities per server frame")]
  [ServerVar]
  public static float framebudgetms = 2.5f;
  private static int lastIndex = 0;

  public static void ProcessQueue()
  {
    float realtimeSinceStartup = Time.get_realtimeSinceStartup();
    float num = AIThinkManager.framebudgetms / 1000f;
    if (AIThinkManager._removalQueue.get_Count() > 0)
    {
      using (IEnumerator<IThinker> enumerator = AIThinkManager._removalQueue.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          IThinker current = enumerator.Current;
          AIThinkManager._processQueue.Remove(current);
        }
      }
      AIThinkManager._removalQueue.Clear();
    }
    for (; AIThinkManager.lastIndex < AIThinkManager._processQueue.get_Count() && (double) Time.get_realtimeSinceStartup() < (double) realtimeSinceStartup + (double) num; ++AIThinkManager.lastIndex)
      AIThinkManager._processQueue.get_Item(AIThinkManager.lastIndex)?.TryThink();
    if (AIThinkManager.lastIndex != AIThinkManager._processQueue.get_Count())
      return;
    AIThinkManager.lastIndex = 0;
  }

  public static void Add(IThinker toAdd)
  {
    AIThinkManager._processQueue.Add(toAdd);
  }

  public static void Remove(IThinker toRemove)
  {
    AIThinkManager._removalQueue.Add(toRemove);
  }
}
