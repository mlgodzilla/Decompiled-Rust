// Decompiled with JetBrains decompiler
// Type: ConstructionSocket
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ConstructionSocket : Socket_Base
{
  public float angleAllowed = 150f;
  [Range(0.0f, 1f)]
  public float support = 1f;
  public ConstructionSocket.Type socketType;
  public int rotationDegrees;
  public int rotationOffset;
  public bool restrictPlacementAngle;
  public float faceAngle;

  private void OnDrawGizmos()
  {
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.set_color(Color.get_red());
    Gizmos.DrawLine(Vector3.get_zero(), Vector3.op_Multiply(Vector3.get_forward(), 0.6f));
    Gizmos.set_color(Color.get_blue());
    Gizmos.DrawLine(Vector3.get_zero(), Vector3.op_Multiply(Vector3.get_right(), 0.1f));
    Gizmos.set_color(Color.get_green());
    Gizmos.DrawLine(Vector3.get_zero(), Vector3.op_Multiply(Vector3.get_up(), 0.1f));
    Gizmos.DrawIcon(((Component) this).get_transform().get_position(), "light_circle_green.png", false);
  }

  private void OnDrawGizmosSelected()
  {
    if (!this.female)
      return;
    Gizmos.set_matrix(((Component) this).get_transform().get_localToWorldMatrix());
    Gizmos.DrawWireCube(this.selectCenter, this.selectSize);
  }

  public override bool TestTarget(Construction.Target target)
  {
    if (!base.TestTarget(target))
      return false;
    return this.IsCompatible(target.socket);
  }

  public override bool IsCompatible(Socket_Base socket)
  {
    if (!base.IsCompatible(socket))
      return false;
    ConstructionSocket constructionSocket = socket as ConstructionSocket;
    return !((PrefabAttribute) constructionSocket == (PrefabAttribute) null) && constructionSocket.socketType != ConstructionSocket.Type.None && (this.socketType != ConstructionSocket.Type.None && constructionSocket.socketType == this.socketType);
  }

  public override bool CanConnect(
    Vector3 position,
    Quaternion rotation,
    Socket_Base socket,
    Vector3 socketPosition,
    Quaternion socketRotation)
  {
    if (!base.CanConnect(position, rotation, socket, socketPosition, socketRotation))
      return false;
    Matrix4x4 matrix4x4_1 = Matrix4x4.TRS(position, rotation, Vector3.get_one());
    Matrix4x4 matrix4x4_2 = Matrix4x4.TRS(socketPosition, socketRotation, Vector3.get_one());
    if ((double) Vector3.Distance(((Matrix4x4) ref matrix4x4_1).MultiplyPoint3x4(this.worldPosition), ((Matrix4x4) ref matrix4x4_2).MultiplyPoint3x4(socket.worldPosition)) > 0.00999999977648258)
      return false;
    Vector3 vector3_1 = ((Matrix4x4) ref matrix4x4_1).MultiplyVector(Quaternion.op_Multiply(this.worldRotation, Vector3.get_forward()));
    Vector3 vector3_2 = ((Matrix4x4) ref matrix4x4_2).MultiplyVector(Quaternion.op_Multiply(socket.worldRotation, Vector3.get_forward()));
    float num = Vector3.Angle(vector3_1, vector3_2);
    if (this.male && this.female)
      num = Mathf.Min(num, Vector3.Angle(Vector3.op_UnaryNegation(vector3_1), vector3_2));
    if (socket.male && socket.female)
      num = Mathf.Min(num, Vector3.Angle(vector3_1, Vector3.op_UnaryNegation(vector3_2)));
    return (double) num <= 1.0;
  }

  public bool TestRestrictedAngles(
    Vector3 suggestedPos,
    Quaternion suggestedAng,
    Construction.Target target)
  {
    if (this.restrictPlacementAngle)
    {
      Quaternion quaternion = Quaternion.op_Multiply(Quaternion.Euler(0.0f, this.faceAngle, 0.0f), suggestedAng);
      float num = Vector3Ex.DotDegrees(Vector3Ex.XZ3D(((Ray) ref target.ray).get_direction()), Quaternion.op_Multiply(quaternion, Vector3.get_forward()));
      if ((double) num > (double) this.angleAllowed * 0.5 || (double) num < (double) this.angleAllowed * -0.5)
        return false;
    }
    return true;
  }

  public override Construction.Placement DoPlacement(Construction.Target target)
  {
    if (!Object.op_Implicit((Object) target.entity) || !Object.op_Implicit((Object) ((Component) target.entity).get_transform()))
      return (Construction.Placement) null;
    Vector3 worldPosition = target.GetWorldPosition();
    Quaternion worldRotation = target.GetWorldRotation(true);
    if (this.rotationDegrees > 0)
    {
      Construction.Placement placement = new Construction.Placement();
      float num1 = float.MaxValue;
      float num2 = 0.0f;
      for (int index = 0; index < 360; index += this.rotationDegrees)
      {
        Quaternion quaternion1 = Quaternion.Euler(0.0f, (float) (this.rotationOffset + index), 0.0f);
        Vector3 direction = ((Ray) ref target.ray).get_direction();
        Quaternion quaternion2 = worldRotation;
        Vector3 vector3 = Quaternion.op_Multiply(Quaternion.op_Multiply(quaternion1, quaternion2), Vector3.get_up());
        float num3 = Vector3.Angle(direction, vector3);
        if ((double) num3 < (double) num1)
        {
          num1 = num3;
          num2 = (float) index;
        }
      }
      for (int index = 0; index < 360; index += this.rotationDegrees)
      {
        Quaternion quaternion1 = Quaternion.op_Multiply(worldRotation, Quaternion.Inverse(this.rotation));
        Quaternion quaternion2 = Quaternion.op_Multiply(Quaternion.op_Multiply(Quaternion.Euler(target.rotation), Quaternion.Euler(0.0f, (float) (this.rotationOffset + index) + num2, 0.0f)), quaternion1);
        Vector3 vector3 = Quaternion.op_Multiply(quaternion2, this.position);
        placement.position = Vector3.op_Subtraction(worldPosition, vector3);
        placement.rotation = quaternion2;
        if (this.CheckSocketMods(placement))
          return placement;
      }
    }
    Construction.Placement placement1 = new Construction.Placement();
    Quaternion quaternion = Quaternion.op_Multiply(worldRotation, Quaternion.Inverse(this.rotation));
    Vector3 vector3_1 = Quaternion.op_Multiply(quaternion, this.position);
    placement1.position = Vector3.op_Subtraction(worldPosition, vector3_1);
    placement1.rotation = quaternion;
    if (!this.TestRestrictedAngles(worldPosition, worldRotation, target))
      return (Construction.Placement) null;
    return placement1;
  }

  public enum Type
  {
    None = 0,
    Foundation = 1,
    Floor = 2,
    Doorway = 4,
    Wall = 5,
    Block = 6,
    Window = 11, // 0x0000000B
    Shutters = 12, // 0x0000000C
    WallFrame = 13, // 0x0000000D
    FloorFrame = 14, // 0x0000000E
    WindowDressing = 15, // 0x0000000F
    DoorDressing = 16, // 0x00000010
  }
}
