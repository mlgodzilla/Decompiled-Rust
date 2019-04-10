// Decompiled with JetBrains decompiler
// Type: RagdollEditor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using UnityEngine;

public class RagdollEditor : SingletonComponent<RagdollEditor>
{
  private Vector3 view;
  private Rigidbody grabbedRigid;
  private Vector3 grabPos;
  private Vector3 grabOffset;

  private void OnGUI()
  {
    GUI.Box(new Rect((float) ((double) Screen.get_width() * 0.5 - 2.0), (float) ((double) Screen.get_height() * 0.5 - 2.0), 4f, 4f), "");
  }

  protected virtual void Awake()
  {
    ((SingletonComponent) this).Awake();
  }

  private void Update()
  {
    Camera.get_main().set_fieldOfView(75f);
    if (Input.GetKey((KeyCode) 324))
    {
      ref __Null local1 = ref this.view.y;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local1 = ^(float&) ref local1 + Input.GetAxisRaw("Mouse X") * 3f;
      ref __Null local2 = ref this.view.x;
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      // ISSUE: cast to a reference type
      // ISSUE: explicit reference operation
      ^(float&) ref local2 = ^(float&) ref local2 - Input.GetAxisRaw("Mouse Y") * 3f;
      Cursor.set_lockState((CursorLockMode) 1);
      Cursor.set_visible(false);
    }
    else
    {
      Cursor.set_lockState((CursorLockMode) 0);
      Cursor.set_visible(true);
    }
    ((Component) Camera.get_main()).get_transform().set_rotation(Quaternion.Euler(this.view));
    Vector3 vector3 = Vector3.get_zero();
    if (Input.GetKey((KeyCode) 119))
      vector3 = Vector3.op_Addition(vector3, Vector3.get_forward());
    if (Input.GetKey((KeyCode) 115))
      vector3 = Vector3.op_Addition(vector3, Vector3.get_back());
    if (Input.GetKey((KeyCode) 97))
      vector3 = Vector3.op_Addition(vector3, Vector3.get_left());
    if (Input.GetKey((KeyCode) 100))
      vector3 = Vector3.op_Addition(vector3, Vector3.get_right());
    Transform transform = ((Component) Camera.get_main()).get_transform();
    transform.set_position(Vector3.op_Addition(transform.get_position(), Vector3.op_Multiply(Quaternion.op_Multiply(((Component) this).get_transform().get_rotation(), vector3), 0.05f)));
    if (Input.GetKeyDown((KeyCode) 323))
      this.StartGrab();
    if (!Input.GetKeyUp((KeyCode) 323))
      return;
    this.StopGrab();
  }

  private void FixedUpdate()
  {
    if (!Input.GetKey((KeyCode) 323))
      return;
    this.UpdateGrab();
  }

  private void StartGrab()
  {
    RaycastHit raycastHit;
    if (!Physics.Raycast(((Component) this).get_transform().get_position(), ((Component) this).get_transform().get_forward(), ref raycastHit, 100f))
      return;
    this.grabbedRigid = (Rigidbody) ((Component) ((RaycastHit) ref raycastHit).get_collider()).GetComponent<Rigidbody>();
    if (Object.op_Equality((Object) this.grabbedRigid, (Object) null))
      return;
    Matrix4x4 worldToLocalMatrix1 = ((Component) this.grabbedRigid).get_transform().get_worldToLocalMatrix();
    this.grabPos = ((Matrix4x4) ref worldToLocalMatrix1).MultiplyPoint(((RaycastHit) ref raycastHit).get_point());
    Matrix4x4 worldToLocalMatrix2 = ((Component) this).get_transform().get_worldToLocalMatrix();
    this.grabOffset = ((Matrix4x4) ref worldToLocalMatrix2).MultiplyPoint(((RaycastHit) ref raycastHit).get_point());
  }

  private void UpdateGrab()
  {
    if (Object.op_Equality((Object) this.grabbedRigid, (Object) null))
      return;
    Vector3 vector3_1 = ((Component) this).get_transform().TransformPoint(this.grabOffset);
    Vector3 vector3_2 = ((Component) this.grabbedRigid).get_transform().TransformPoint(this.grabPos);
    Vector3 vector3_3 = vector3_2;
    this.grabbedRigid.AddForceAtPosition(Vector3.op_Multiply(Vector3.op_Subtraction(vector3_1, vector3_3), 100f), vector3_2, (ForceMode) 5);
  }

  private void StopGrab()
  {
    this.grabbedRigid = (Rigidbody) null;
  }

  public RagdollEditor()
  {
    base.\u002Ector();
  }
}
