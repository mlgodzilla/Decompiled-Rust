// Decompiled with JetBrains decompiler
// Type: ModelConditionTest_RoofBottom
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ModelConditionTest_RoofBottom : ModelConditionTest
{
  private const string socket = "roof/sockets/wall-male";
  private const string socket_female = "roof/sockets/wall-female";

  protected void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(Color.get_gray());
    Gizmos.DrawWireCube(new Vector3(0.0f, -1.5f, 3f), new Vector3(3f, 3f, 3f));
  }

  public override bool DoTest(BaseEntity ent)
  {
    EntityLink link = ent.FindLink("roof/sockets/wall-male");
    if (link == null)
      return false;
    for (int index = 0; index < link.connections.Count; ++index)
    {
      if (link.connections[index].name == "roof/sockets/wall-female")
        return false;
    }
    return true;
  }
}
