// Decompiled with JetBrains decompiler
// Type: RFManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class RFManager
{
  public static Dictionary<int, List<IRFObject>> _listeners = new Dictionary<int, List<IRFObject>>();
  public static Dictionary<int, List<IRFObject>> _broadcasters = new Dictionary<int, List<IRFObject>>();
  public static int minFreq = 1;
  public static int maxFreq = 9999;
  private static int reserveRangeMin = 4760;
  private static int reserveRangeMax = 4790;
  public static string reserveString = "Channels " + (object) RFManager.reserveRangeMin + " to " + (object) RFManager.reserveRangeMax + " are restricted.";

  public static int ClampFrequency(int freq)
  {
    return Mathf.Clamp(freq, RFManager.minFreq, RFManager.maxFreq);
  }

  public static List<IRFObject> GetListenList(int frequency)
  {
    frequency = RFManager.ClampFrequency(frequency);
    List<IRFObject> rfObjectList = (List<IRFObject>) null;
    if (!RFManager._listeners.TryGetValue(frequency, out rfObjectList))
    {
      rfObjectList = new List<IRFObject>();
      RFManager._listeners.Add(frequency, rfObjectList);
    }
    return rfObjectList;
  }

  public static List<IRFObject> GetBroadcasterList(int frequency)
  {
    frequency = RFManager.ClampFrequency(frequency);
    List<IRFObject> rfObjectList = (List<IRFObject>) null;
    if (!RFManager._broadcasters.TryGetValue(frequency, out rfObjectList))
    {
      rfObjectList = new List<IRFObject>();
      RFManager._broadcasters.Add(frequency, rfObjectList);
    }
    return rfObjectList;
  }

  public static void AddListener(int frequency, IRFObject obj)
  {
    frequency = RFManager.ClampFrequency(frequency);
    List<IRFObject> listenList = RFManager.GetListenList(frequency);
    if (listenList.Contains(obj))
    {
      Debug.Log((object) "adding same listener twice");
    }
    else
    {
      listenList.Add(obj);
      RFManager.MarkFrequencyDirty(frequency);
    }
  }

  public static void RemoveListener(int frequency, IRFObject obj)
  {
    frequency = RFManager.ClampFrequency(frequency);
    List<IRFObject> listenList = RFManager.GetListenList(frequency);
    if (listenList.Contains(obj))
      listenList.Remove(obj);
    obj.RFSignalUpdate(false);
  }

  public static void AddBroadcaster(int frequency, IRFObject obj)
  {
    frequency = RFManager.ClampFrequency(frequency);
    List<IRFObject> broadcasterList = RFManager.GetBroadcasterList(frequency);
    if (broadcasterList.Contains(obj))
      return;
    broadcasterList.Add(obj);
    RFManager.MarkFrequencyDirty(frequency);
  }

  public static void RemoveBroadcaster(int frequency, IRFObject obj)
  {
    frequency = RFManager.ClampFrequency(frequency);
    List<IRFObject> broadcasterList = RFManager.GetBroadcasterList(frequency);
    if (broadcasterList.Contains(obj))
      broadcasterList.Remove(obj);
    RFManager.MarkFrequencyDirty(frequency);
  }

  public static bool IsReserved(int frequency)
  {
    return frequency >= RFManager.reserveRangeMin && frequency <= RFManager.reserveRangeMax;
  }

  public static void ReserveErrorPrint(BasePlayer player)
  {
    player.ChatMessage(RFManager.reserveString);
  }

  public static void ChangeFrequency(
    int oldFrequency,
    int newFrequency,
    IRFObject obj,
    bool isListener,
    bool isOn = true)
  {
    newFrequency = RFManager.ClampFrequency(newFrequency);
    if (isListener)
    {
      RFManager.RemoveListener(oldFrequency, obj);
      if (!isOn)
        return;
      RFManager.AddListener(newFrequency, obj);
    }
    else
    {
      RFManager.RemoveBroadcaster(oldFrequency, obj);
      if (!isOn)
        return;
      RFManager.AddBroadcaster(newFrequency, obj);
    }
  }

  public static void MarkFrequencyDirty(int frequency)
  {
    frequency = RFManager.ClampFrequency(frequency);
    List<IRFObject> broadcasterList = RFManager.GetBroadcasterList(frequency);
    List<IRFObject> listenList = RFManager.GetListenList(frequency);
    bool on = broadcasterList.Count > 0;
    bool flag1 = false;
    bool flag2 = false;
    foreach (IRFObject rfObject1 in listenList)
    {
      if (!rfObject1.IsValidEntityReference<IRFObject>())
      {
        flag1 = true;
      }
      else
      {
        if (on)
        {
          on = false;
          foreach (IRFObject rfObject2 in broadcasterList)
          {
            if (!rfObject2.IsValidEntityReference<IRFObject>())
              flag2 = true;
            else if ((double) Vector3.Distance(rfObject2.GetPosition(), rfObject1.GetPosition()) <= (double) rfObject2.GetMaxRange())
            {
              on = true;
              break;
            }
          }
        }
        rfObject1.RFSignalUpdate(on);
      }
    }
    if (flag1)
    {
      Debug.LogWarning((object) ("Found null entries in the RF listener list for frequency " + (object) frequency + "... cleaning up."));
      for (int index = listenList.Count - 1; index >= 0; --index)
      {
        if (listenList[index] == null)
          listenList.RemoveAt(index);
      }
    }
    if (!flag2)
      return;
    Debug.LogWarning((object) ("Found null entries in the RF broadcaster list for frequency " + (object) frequency + "... cleaning up."));
    for (int index = broadcasterList.Count - 1; index >= 0; --index)
    {
      if (broadcasterList[index] == null)
        broadcasterList.RemoveAt(index);
    }
  }
}
