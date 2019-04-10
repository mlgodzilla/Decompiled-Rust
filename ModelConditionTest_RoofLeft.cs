// Decompiled with JetBrains decompiler
// Type: ModelConditionTest_RoofLeft
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ModelConditionTest_RoofLeft : ModelConditionTest
{
  private const string socket = "roof/sockets/neighbour/2";
  private const string socket_female = "roof/sockets/neighbour/1";

  protected void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(Color.get_gray());
    Gizmos.DrawWireCube(new Vector3(3f, 1.5f, 0.0f), new Vector3(3f, 3f, 3f));
  }

  public override bool DoTest(BaseEntity ent)
  {
    EntityLink link = ent.FindLink("roof/sockets/neighbour/2");
    if (link == null)
      return false;
    for (int index = 0; index < link.connections.Count; ++index)
    {
      if (link.connections[index].name == "roof/sockets/neighbour/1")
        return false;
    }
    return true;
  }
}
