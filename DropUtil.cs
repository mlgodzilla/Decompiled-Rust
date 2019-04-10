// Decompiled with JetBrains decompiler
// Type: DropUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using ConVar;
using Oxide.Core;
using UnityEngine;

public class DropUtil
{
  public static void DropItems(ItemContainer container, Vector3 position, float chance = 1f)
  {
    if (!Server.dropitems || container == null || (container.itemList == null || Interface.CallHook("OnContainerDropItems", (object) container) != null))
      return;
    float num1 = 0.25f;
    foreach (Item obj in container.itemList.ToArray())
    {
      if ((double) Random.Range(0.0f, 1f) <= (double) chance)
      {
        float num2 = Random.Range(0.0f, 2f);
        obj.RemoveFromContainer();
        BaseEntity worldObject = obj.CreateWorldObject(Vector3.op_Addition(position, new Vector3(Random.Range(-num1, num1), 1f, Random.Range(-num1, num1))), (Quaternion) null, (BaseEntity) null, 0U);
        if (Object.op_Equality((Object) worldObject, (Object) null))
          obj.Remove(0.0f);
        else if ((double) num2 > 0.0)
        {
          worldObject.SetVelocity(Vector3.op_Multiply(new Vector3(Random.Range(-1f, 1f), Random.Range(0.0f, 1f), Random.Range(-1f, 1f)), num2));
          worldObject.SetAngularVelocity(Vector3.op_Multiply(new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)), num2));
        }
      }
    }
  }
}
