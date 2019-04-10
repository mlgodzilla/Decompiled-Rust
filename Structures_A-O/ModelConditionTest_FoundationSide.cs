// Decompiled with JetBrains decompiler
// Type: ModelConditionTest_FoundationSide
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ModelConditionTest_FoundationSide : ModelConditionTest
{
  private string socket = string.Empty;
  private const string square_south = "foundation/sockets/foundation-top/1";
  private const string square_north = "foundation/sockets/foundation-top/3";
  private const string square_west = "foundation/sockets/foundation-top/2";
  private const string square_east = "foundation/sockets/foundation-top/4";
  private const string triangle_south = "foundation.triangle/sockets/foundation-top/1";
  private const string triangle_northwest = "foundation.triangle/sockets/foundation-top/2";
  private const string triangle_northeast = "foundation.triangle/sockets/foundation-top/3";

  protected void OnDrawGizmosSelected()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(Color.get_gray());
    Gizmos.DrawWireCube(new Vector3(1.5f, 1.5f, 0.0f), new Vector3(3f, 3f, 3f));
  }

  protected override void AttributeSetup(
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    Vector3 vector3 = Quaternion.op_Multiply(this.worldRotation, Vector3.get_right());
    if (name.Contains("foundation.triangle"))
    {
      if (vector3.z < -0.899999976158142)
        this.socket = "foundation.triangle/sockets/foundation-top/1";
      if (vector3.x < -0.100000001490116)
        this.socket = "foundation.triangle/sockets/foundation-top/2";
      if (vector3.x <= 0.100000001490116)
        return;
      this.socket = "foundation.triangle/sockets/foundation-top/3";
    }
    else
    {
      if (vector3.z < -0.899999976158142)
        this.socket = "foundation/sockets/foundation-top/1";
      if (vector3.z > 0.899999976158142)
        this.socket = "foundation/sockets/foundation-top/3";
      if (vector3.x < -0.899999976158142)
        this.socket = "foundation/sockets/foundation-top/2";
      if (vector3.x <= 0.899999976158142)
        return;
      this.socket = "foundation/sockets/foundation-top/4";
    }
  }

  public override bool DoTest(BaseEntity ent)
  {
    EntityLink link = ent.FindLink(this.socket);
    if (link == null)
      return false;
    for (int index = 0; index < link.connections.Count; ++index)
    {
      BuildingBlock owner = link.connections[index].owner as BuildingBlock;
      if (!Object.op_Equality((Object) owner, (Object) null) && !(owner.blockDefinition.info.name.token == "foundation_steps") && (owner.grade == BuildingGrade.Enum.TopTier || owner.grade == BuildingGrade.Enum.Metal || owner.grade == BuildingGrade.Enum.Stone))
        return false;
    }
    return true;
  }
}
