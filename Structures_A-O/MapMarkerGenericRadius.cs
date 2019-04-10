// Decompiled with JetBrains decompiler
// Type: MapMarkerGenericRadius
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using Network;
using UnityEngine;

public class MapMarkerGenericRadius : MapMarker
{
  public float radius;
  public Color color1;
  public Color color2;
  public float alpha;

  public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
  {
    using (TimeWarning.New("MapMarkerGenericRadius.OnRpcMessage", 0.1f))
      ;
    return base.OnRpcMessage(player, rpc, msg);
  }

  public void SendUpdate(bool fullUpdate = true)
  {
    float a = (float) this.color1.a;
    Vector3 vector3_1;
    ((Vector3) ref vector3_1).\u002Ector((float) this.color1.r, (float) this.color1.g, (float) this.color1.b);
    Vector3 vector3_2;
    ((Vector3) ref vector3_2).\u002Ector((float) this.color2.r, (float) this.color2.g, (float) this.color2.b);
    this.ClientRPC<Vector3, float, Vector3, float, float>((Connection) null, "MarkerUpdate", vector3_1, a, vector3_2, this.alpha, this.radius);
  }
}
