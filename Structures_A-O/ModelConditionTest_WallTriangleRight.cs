// Decompiled with JetBrains decompiler
// Type: ModelConditionTest_WallTriangleRight
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ModelConditionTest_WallTriangleRight : ModelConditionTest
{
  private const string socket_1 = "wall/sockets/wall-female";
  private const string socket_2 = "wall/sockets/floor-female/1";
  private const string socket_3 = "wall/sockets/floor-female/2";
  private const string socket_4 = "wall/sockets/stability/2";
  private const string socket = "wall/sockets/neighbour/1";

  public static bool CheckCondition(BaseEntity ent)
  {
    if (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/wall-female") || ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/1") || (ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/floor-female/2") || ModelConditionTest_WallTriangleRight.CheckSocketOccupied(ent, "wall/sockets/stability/2")))
      return false;
    EntityLink link = ent.FindLink("wall/sockets/neighbour/1");
    if (link == null)
      return false;
    for (int index = 0; index < link.connections.Count; ++index)
    {
      BuildingBlock owner = link.connections[index].owner as BuildingBlock;
      if (!Object.op_Equality((Object) owner, (Object) null) && !(owner.blockDefinition.info.name.token != "roof") && (double) Vector3.Angle(((Component) ent).get_transform().get_forward(), Vector3.op_UnaryNegation(((Component) owner).get_transform().get_forward())) <= 10.0)
        return true;
    }
    return false;
  }

  private static bool CheckSocketOccupied(BaseEntity ent, string socket)
  {
    EntityLink link = ent.FindLink(socket);
    if (link == null)
      return false;
    return !link.IsEmpty();
  }

  public override bool DoTest(BaseEntity ent)
  {
    return ModelConditionTest_WallTriangleRight.CheckCondition(ent);
  }
}
