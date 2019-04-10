// Decompiled with JetBrains decompiler
// Type: Socket_Base
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public class Socket_Base : PrefabAttribute
{
  public bool male = true;
  public Vector3 selectSize = new Vector3(2f, 0.1f, 2f);
  public Vector3 selectCenter = new Vector3(0.0f, 0.0f, 1f);
  public bool maleDummy;
  public bool female;
  public bool femaleDummy;
  public bool monogamous;
  [NonSerialized]
  public Vector3 position;
  [NonSerialized]
  public Quaternion rotation;
  [HideInInspector]
  public string socketName;
  [NonSerialized]
  public SocketMod[] socketMods;

  public Vector3 GetSelectPivot(Vector3 position, Quaternion rotation)
  {
    return Vector3.op_Addition(position, Quaternion.op_Multiply(rotation, this.worldPosition));
  }

  public OBB GetSelectBounds(Vector3 position, Quaternion rotation)
  {
    return new OBB(Vector3.op_Addition(position, Quaternion.op_Multiply(rotation, this.worldPosition)), Vector3.get_one(), Quaternion.op_Multiply(rotation, this.worldRotation), new Bounds(this.selectCenter, this.selectSize));
  }

  protected override System.Type GetIndexedType()
  {
    return typeof (Socket_Base);
  }

  protected override void AttributeSetup(
    GameObject rootObj,
    string name,
    bool serverside,
    bool clientside,
    bool bundling)
  {
    base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
    this.position = ((Component) this).get_transform().get_position();
    this.rotation = ((Component) this).get_transform().get_rotation();
    this.socketMods = (SocketMod[]) ((Component) this).GetComponentsInChildren<SocketMod>(true);
    foreach (SocketMod socketMod in this.socketMods)
      socketMod.baseSocket = this;
  }

  public virtual bool TestTarget(Construction.Target target)
  {
    return (PrefabAttribute) target.socket != (PrefabAttribute) null;
  }

  public virtual bool IsCompatible(Socket_Base socket)
  {
    if ((PrefabAttribute) socket == (PrefabAttribute) null || !socket.male && !this.male || !socket.female && !this.female)
      return false;
    return ((object) socket).GetType() == ((object) this).GetType();
  }

  public virtual bool CanConnect(
    Vector3 position,
    Quaternion rotation,
    Socket_Base socket,
    Vector3 socketPosition,
    Quaternion socketRotation)
  {
    return this.IsCompatible(socket);
  }

  public virtual Construction.Placement DoPlacement(Construction.Target target)
  {
    Quaternion quaternion = Quaternion.op_Multiply(Quaternion.LookRotation(target.normal, Vector3.get_up()), Quaternion.Euler(target.rotation));
    Vector3 vector3 = Vector3.op_Subtraction(target.position, Quaternion.op_Multiply(quaternion, this.position));
    return new Construction.Placement()
    {
      rotation = quaternion,
      position = vector3
    };
  }

  public virtual bool CheckSocketMods(Construction.Placement placement)
  {
    foreach (SocketMod socketMod in this.socketMods)
      socketMod.ModifyPlacement(placement);
    foreach (SocketMod socketMod in this.socketMods)
    {
      if (!socketMod.DoCheck(placement))
      {
        if (socketMod.FailedPhrase.IsValid())
          Construction.lastPlacementError = "Failed Check: (" + socketMod.FailedPhrase.translated + ")";
        return false;
      }
    }
    return true;
  }
}
