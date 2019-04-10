// Decompiled with JetBrains decompiler
// Type: ExplosionDemoGUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class ExplosionDemoGUI : MonoBehaviour
{
  public GameObject[] Prefabs;
  public float reactivateTime;
  public Light Sun;
  private int currentNomber;
  private GameObject currentInstance;
  private GUIStyle guiStyleHeader;
  private float sunIntensity;
  private float dpiScale;

  private void Start()
  {
    if ((double) Screen.get_dpi() < 1.0)
      this.dpiScale = 1f;
    this.dpiScale = (double) Screen.get_dpi() >= 200.0 ? Screen.get_dpi() / 200f : 1f;
    this.guiStyleHeader.set_fontSize((int) (15.0 * (double) this.dpiScale));
    this.guiStyleHeader.get_normal().set_textColor(new Color(0.15f, 0.15f, 0.15f));
    this.currentInstance = (GameObject) Object.Instantiate<GameObject>((M0) this.Prefabs[this.currentNomber], ((Component) this).get_transform().get_position(), (Quaternion) null);
    ((ExplosionDemoReactivator) this.currentInstance.AddComponent<ExplosionDemoReactivator>()).TimeDelayToReactivate = this.reactivateTime;
    this.sunIntensity = this.Sun.get_intensity();
  }

  private void OnGUI()
  {
    if (GUI.Button(new Rect(10f * this.dpiScale, 15f * this.dpiScale, 135f * this.dpiScale, 37f * this.dpiScale), "PREVIOUS EFFECT"))
      this.ChangeCurrent(-1);
    if (GUI.Button(new Rect(160f * this.dpiScale, 15f * this.dpiScale, 135f * this.dpiScale, 37f * this.dpiScale), "NEXT EFFECT"))
      this.ChangeCurrent(1);
    this.sunIntensity = GUI.HorizontalSlider(new Rect(10f * this.dpiScale, 70f * this.dpiScale, 285f * this.dpiScale, 15f * this.dpiScale), this.sunIntensity, 0.0f, 0.6f);
    this.Sun.set_intensity(this.sunIntensity);
    GUI.Label(new Rect(300f * this.dpiScale, 70f * this.dpiScale, 30f * this.dpiScale, 30f * this.dpiScale), "SUN INTENSITY", this.guiStyleHeader);
    GUI.Label(new Rect(400f * this.dpiScale, 15f * this.dpiScale, 100f * this.dpiScale, 20f * this.dpiScale), "Prefab name is \"" + ((Object) this.Prefabs[this.currentNomber]).get_name() + "\"  \r\nHold any mouse button that would move the camera", this.guiStyleHeader);
  }

  private void ChangeCurrent(int delta)
  {
    this.currentNomber += delta;
    if (this.currentNomber > this.Prefabs.Length - 1)
      this.currentNomber = 0;
    else if (this.currentNomber < 0)
      this.currentNomber = this.Prefabs.Length - 1;
    if (Object.op_Inequality((Object) this.currentInstance, (Object) null))
      Object.Destroy((Object) this.currentInstance);
    this.currentInstance = (GameObject) Object.Instantiate<GameObject>((M0) this.Prefabs[this.currentNomber], ((Component) this).get_transform().get_position(), (Quaternion) null);
    ((ExplosionDemoReactivator) this.currentInstance.AddComponent<ExplosionDemoReactivator>()).TimeDelayToReactivate = this.reactivateTime;
  }

  public ExplosionDemoGUI()
  {
    base.\u002Ector();
  }
}
