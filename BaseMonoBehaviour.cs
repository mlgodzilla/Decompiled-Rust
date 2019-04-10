// Decompiled with JetBrains decompiler
// Type: BaseMonoBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using UnityEngine;

public abstract class BaseMonoBehaviour : FacepunchBehaviour
{
  public virtual bool IsDebugging()
  {
    return false;
  }

  public virtual string GetLogColor()
  {
    return "yellow";
  }

  public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str, object arg1)
  {
    if (!this.IsDebugging() && Global.developer < level)
      return;
    string str1 = string.Format(str, arg1);
    Debug.Log((object) string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", (object) log.ToString().PadRight(10), (object) ((object) this).ToString(), (object) str1, (object) this.GetLogColor()), (Object) ((Component) this).get_gameObject());
  }

  public void LogEntry(
    BaseMonoBehaviour.LogEntryType log,
    int level,
    string str,
    object arg1,
    object arg2)
  {
    if (!this.IsDebugging() && Global.developer < level)
      return;
    string str1 = string.Format(str, arg1, arg2);
    Debug.Log((object) string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", (object) log.ToString().PadRight(10), (object) ((object) this).ToString(), (object) str1, (object) this.GetLogColor()), (Object) ((Component) this).get_gameObject());
  }

  public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str)
  {
    if (!this.IsDebugging() && Global.developer < level)
      return;
    string str1 = str;
    Debug.Log((object) string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", (object) log.ToString().PadRight(10), (object) ((object) this).ToString(), (object) str1, (object) this.GetLogColor()), (Object) ((Component) this).get_gameObject());
  }

  protected BaseMonoBehaviour()
  {
    base.\u002Ector();
  }

  public enum LogEntryType
  {
    General,
    Network,
    Hierarchy,
    Serialization,
  }
}
