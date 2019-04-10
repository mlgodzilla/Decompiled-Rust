// Decompiled with JetBrains decompiler
// Type: GizmosUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

public static class GizmosUtil
{
  public static void DrawWireCircleX(Vector3 pos, float radius)
  {
    Matrix4x4 matrix = Gizmos.get_matrix();
    Gizmos.set_matrix(Matrix4x4.op_Multiply(Gizmos.get_matrix(), Matrix4x4.TRS(pos, Quaternion.get_identity(), new Vector3(0.0f, 1f, 1f))));
    Gizmos.DrawWireSphere(Vector3.get_zero(), radius);
    Gizmos.set_matrix(matrix);
  }

  public static void DrawWireCircleY(Vector3 pos, float radius)
  {
    Matrix4x4 matrix = Gizmos.get_matrix();
    Gizmos.set_matrix(Matrix4x4.op_Multiply(Gizmos.get_matrix(), Matrix4x4.TRS(pos, Quaternion.get_identity(), new Vector3(1f, 0.0f, 1f))));
    Gizmos.DrawWireSphere(Vector3.get_zero(), radius);
    Gizmos.set_matrix(matrix);
  }

  public static void DrawWireCircleZ(Vector3 pos, float radius)
  {
    Matrix4x4 matrix = Gizmos.get_matrix();
    Gizmos.set_matrix(Matrix4x4.op_Multiply(Gizmos.get_matrix(), Matrix4x4.TRS(pos, Quaternion.get_identity(), new Vector3(1f, 1f, 0.0f))));
    Gizmos.DrawWireSphere(Vector3.get_zero(), radius);
    Gizmos.set_matrix(matrix);
  }

  public static void DrawCircleX(Vector3 pos, float radius)
  {
    Matrix4x4 matrix = Gizmos.get_matrix();
    Gizmos.set_matrix(Matrix4x4.op_Multiply(Gizmos.get_matrix(), Matrix4x4.TRS(pos, Quaternion.get_identity(), new Vector3(0.0f, 1f, 1f))));
    Gizmos.DrawSphere(Vector3.get_zero(), radius);
    Gizmos.set_matrix(matrix);
  }

  public static void DrawCircleY(Vector3 pos, float radius)
  {
    Matrix4x4 matrix = Gizmos.get_matrix();
    Gizmos.set_matrix(Matrix4x4.op_Multiply(Gizmos.get_matrix(), Matrix4x4.TRS(pos, Quaternion.get_identity(), new Vector3(1f, 0.0f, 1f))));
    Gizmos.DrawSphere(Vector3.get_zero(), radius);
    Gizmos.set_matrix(matrix);
  }

  public static void DrawCircleZ(Vector3 pos, float radius)
  {
    Matrix4x4 matrix = Gizmos.get_matrix();
    Gizmos.set_matrix(Matrix4x4.op_Multiply(Gizmos.get_matrix(), Matrix4x4.TRS(pos, Quaternion.get_identity(), new Vector3(1f, 1f, 0.0f))));
    Gizmos.DrawSphere(Vector3.get_zero(), radius);
    Gizmos.set_matrix(matrix);
  }

  public static void DrawWireCylinderX(Vector3 pos, float radius, float height)
  {
    GizmosUtil.DrawWireCircleX(Vector3.op_Subtraction(pos, new Vector3(0.5f * height, 0.0f, 0.0f)), radius);
    GizmosUtil.DrawWireCircleX(Vector3.op_Addition(pos, new Vector3(0.5f * height, 0.0f, 0.0f)), radius);
  }

  public static void DrawWireCylinderY(Vector3 pos, float radius, float height)
  {
    GizmosUtil.DrawWireCircleY(Vector3.op_Subtraction(pos, new Vector3(0.0f, 0.5f * height, 0.0f)), radius);
    GizmosUtil.DrawWireCircleY(Vector3.op_Addition(pos, new Vector3(0.0f, 0.5f * height, 0.0f)), radius);
  }

  public static void DrawWireCylinderZ(Vector3 pos, float radius, float height)
  {
    GizmosUtil.DrawWireCircleZ(Vector3.op_Subtraction(pos, new Vector3(0.0f, 0.0f, 0.5f * height)), radius);
    GizmosUtil.DrawWireCircleZ(Vector3.op_Addition(pos, new Vector3(0.0f, 0.0f, 0.5f * height)), radius);
  }

  public static void DrawCylinderX(Vector3 pos, float radius, float height)
  {
    GizmosUtil.DrawCircleX(Vector3.op_Subtraction(pos, new Vector3(0.5f * height, 0.0f, 0.0f)), radius);
    GizmosUtil.DrawCircleX(Vector3.op_Addition(pos, new Vector3(0.5f * height, 0.0f, 0.0f)), radius);
  }

  public static void DrawCylinderY(Vector3 pos, float radius, float height)
  {
    GizmosUtil.DrawCircleY(Vector3.op_Subtraction(pos, new Vector3(0.0f, 0.5f * height, 0.0f)), radius);
    GizmosUtil.DrawCircleY(Vector3.op_Addition(pos, new Vector3(0.0f, 0.5f * height, 0.0f)), radius);
  }

  public static void DrawCylinderZ(Vector3 pos, float radius, float height)
  {
    GizmosUtil.DrawCircleZ(Vector3.op_Subtraction(pos, new Vector3(0.0f, 0.0f, 0.5f * height)), radius);
    GizmosUtil.DrawCircleZ(Vector3.op_Addition(pos, new Vector3(0.0f, 0.0f, 0.5f * height)), radius);
  }

  public static void DrawWireCapsuleX(Vector3 pos, float radius, float height)
  {
    Vector3 vector3_1 = Vector3.op_Subtraction(pos, new Vector3(0.5f * height, 0.0f, 0.0f));
    Vector3 vector3_2 = Vector3.op_Addition(pos, new Vector3(0.5f * height, 0.0f, 0.0f));
    Gizmos.DrawWireSphere(vector3_1, radius);
    Gizmos.DrawWireSphere(vector3_2, radius);
    Gizmos.DrawLine(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(Vector3.get_forward(), radius)), Vector3.op_Addition(vector3_2, Vector3.op_Multiply(Vector3.get_forward(), radius)));
    Gizmos.DrawLine(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(Vector3.get_up(), radius)), Vector3.op_Addition(vector3_2, Vector3.op_Multiply(Vector3.get_up(), radius)));
    Gizmos.DrawLine(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(Vector3.get_back(), radius)), Vector3.op_Addition(vector3_2, Vector3.op_Multiply(Vector3.get_back(), radius)));
    Gizmos.DrawLine(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(Vector3.get_down(), radius)), Vector3.op_Addition(vector3_2, Vector3.op_Multiply(Vector3.get_down(), radius)));
  }

  public static void DrawWireCapsuleY(Vector3 pos, float radius, float height)
  {
    Vector3 vector3_1 = Vector3.op_Subtraction(pos, new Vector3(0.0f, 0.5f * height, 0.0f));
    Vector3 vector3_2 = Vector3.op_Addition(pos, new Vector3(0.0f, 0.5f * height, 0.0f));
    Gizmos.DrawWireSphere(vector3_1, radius);
    Gizmos.DrawWireSphere(vector3_2, radius);
    Gizmos.DrawLine(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(Vector3.get_forward(), radius)), Vector3.op_Addition(vector3_2, Vector3.op_Multiply(Vector3.get_forward(), radius)));
    Gizmos.DrawLine(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(Vector3.get_right(), radius)), Vector3.op_Addition(vector3_2, Vector3.op_Multiply(Vector3.get_right(), radius)));
    Gizmos.DrawLine(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(Vector3.get_back(), radius)), Vector3.op_Addition(vector3_2, Vector3.op_Multiply(Vector3.get_back(), radius)));
    Gizmos.DrawLine(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(Vector3.get_left(), radius)), Vector3.op_Addition(vector3_2, Vector3.op_Multiply(Vector3.get_left(), radius)));
  }

  public static void DrawWireCapsuleZ(Vector3 pos, float radius, float height)
  {
    Vector3 vector3_1 = Vector3.op_Subtraction(pos, new Vector3(0.0f, 0.0f, 0.5f * height));
    Vector3 vector3_2 = Vector3.op_Addition(pos, new Vector3(0.0f, 0.0f, 0.5f * height));
    Gizmos.DrawWireSphere(vector3_1, radius);
    Gizmos.DrawWireSphere(vector3_2, radius);
    Gizmos.DrawLine(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(Vector3.get_up(), radius)), Vector3.op_Addition(vector3_2, Vector3.op_Multiply(Vector3.get_up(), radius)));
    Gizmos.DrawLine(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(Vector3.get_right(), radius)), Vector3.op_Addition(vector3_2, Vector3.op_Multiply(Vector3.get_right(), radius)));
    Gizmos.DrawLine(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(Vector3.get_down(), radius)), Vector3.op_Addition(vector3_2, Vector3.op_Multiply(Vector3.get_down(), radius)));
    Gizmos.DrawLine(Vector3.op_Addition(vector3_1, Vector3.op_Multiply(Vector3.get_left(), radius)), Vector3.op_Addition(vector3_2, Vector3.op_Multiply(Vector3.get_left(), radius)));
  }

  public static void DrawCapsuleX(Vector3 pos, float radius, float height)
  {
    Vector3 vector3_1 = Vector3.op_Subtraction(pos, new Vector3(0.5f * height, 0.0f, 0.0f));
    Vector3 vector3_2 = Vector3.op_Addition(pos, new Vector3(0.5f * height, 0.0f, 0.0f));
    Gizmos.DrawSphere(vector3_1, radius);
    double num = (double) radius;
    Gizmos.DrawSphere(vector3_2, (float) num);
  }

  public static void DrawCapsuleY(Vector3 pos, float radius, float height)
  {
    Vector3 vector3_1 = Vector3.op_Subtraction(pos, new Vector3(0.0f, 0.5f * height, 0.0f));
    Vector3 vector3_2 = Vector3.op_Addition(pos, new Vector3(0.0f, 0.5f * height, 0.0f));
    Gizmos.DrawSphere(vector3_1, radius);
    double num = (double) radius;
    Gizmos.DrawSphere(vector3_2, (float) num);
  }

  public static void DrawCapsuleZ(Vector3 pos, float radius, float height)
  {
    Vector3 vector3_1 = Vector3.op_Subtraction(pos, new Vector3(0.0f, 0.0f, 0.5f * height));
    Vector3 vector3_2 = Vector3.op_Addition(pos, new Vector3(0.0f, 0.0f, 0.5f * height));
    Gizmos.DrawSphere(vector3_1, radius);
    double num = (double) radius;
    Gizmos.DrawSphere(vector3_2, (float) num);
  }

  public static void DrawWireCube(Vector3 pos, Vector3 size, Quaternion rot)
  {
    Matrix4x4 matrix = Gizmos.get_matrix();
    Gizmos.set_matrix(Matrix4x4.TRS(pos, rot, size));
    Gizmos.DrawWireCube(Vector3.get_zero(), Vector3.get_one());
    Gizmos.set_matrix(matrix);
  }

  public static void DrawCube(Vector3 pos, Vector3 size, Quaternion rot)
  {
    Matrix4x4 matrix = Gizmos.get_matrix();
    Gizmos.set_matrix(Matrix4x4.TRS(pos, rot, size));
    Gizmos.DrawCube(Vector3.get_zero(), Vector3.get_one());
    Gizmos.set_matrix(matrix);
  }

  public static void DrawWirePath(Vector3 a, Vector3 b, float thickness)
  {
    GizmosUtil.DrawWireCircleY(a, thickness);
    GizmosUtil.DrawWireCircleY(b, thickness);
    Vector3 vector3_1 = Vector3.op_Subtraction(b, a);
    Vector3 vector3_2 = Quaternion.op_Multiply(Quaternion.Euler(0.0f, 90f, 0.0f), ((Vector3) ref vector3_1).get_normalized());
    Gizmos.DrawLine(Vector3.op_Addition(b, Vector3.op_Multiply(vector3_2, thickness)), Vector3.op_Addition(a, Vector3.op_Multiply(vector3_2, thickness)));
    Gizmos.DrawLine(Vector3.op_Subtraction(b, Vector3.op_Multiply(vector3_2, thickness)), Vector3.op_Subtraction(a, Vector3.op_Multiply(vector3_2, thickness)));
  }

  public static void DrawSemiCircle(float radius)
  {
    float num1 = (float) ((double) radius * (Math.PI / 180.0) * 0.5);
    Vector3 vector3_1 = Vector3.op_Addition(Vector3.op_Multiply(Mathf.Cos(num1), Vector3.get_forward()), Vector3.op_Multiply(Mathf.Sin(num1), Vector3.get_right()));
    Gizmos.DrawLine(Vector3.get_zero(), vector3_1);
    Vector3 vector3_2 = Vector3.op_Addition(Vector3.op_Multiply(Mathf.Cos(-num1), Vector3.get_forward()), Vector3.op_Multiply(Mathf.Sin(-num1), Vector3.get_right()));
    Gizmos.DrawLine(Vector3.get_zero(), vector3_2);
    float num2 = Mathf.Clamp(radius / 16f, 4f, 64f);
    float num3 = num1 / num2;
    for (float num4 = num1; (double) num4 > 0.0; num4 -= num3)
    {
      Vector3 vector3_3 = Vector3.op_Addition(Vector3.op_Multiply(Mathf.Cos(num4), Vector3.get_forward()), Vector3.op_Multiply(Mathf.Sin(num4), Vector3.get_right()));
      Gizmos.DrawLine(Vector3.get_zero(), vector3_3);
      if (Vector3.op_Inequality(vector3_1, Vector3.get_zero()))
        Gizmos.DrawLine(vector3_3, vector3_1);
      vector3_1 = vector3_3;
      Vector3 vector3_4 = Vector3.op_Addition(Vector3.op_Multiply(Mathf.Cos(-num4), Vector3.get_forward()), Vector3.op_Multiply(Mathf.Sin(-num4), Vector3.get_right()));
      Gizmos.DrawLine(Vector3.get_zero(), vector3_4);
      if (Vector3.op_Inequality(vector3_2, Vector3.get_zero()))
        Gizmos.DrawLine(vector3_4, vector3_2);
      vector3_2 = vector3_4;
    }
    Gizmos.DrawLine(vector3_1, vector3_2);
  }

  public static void DrawMeshes(Transform transform)
  {
    foreach (MeshRenderer componentsInChild in (MeshRenderer[]) ((Component) transform).GetComponentsInChildren<MeshRenderer>())
    {
      if (((Renderer) componentsInChild).get_enabled())
      {
        MeshFilter component = (MeshFilter) ((Component) componentsInChild).GetComponent<MeshFilter>();
        if (Object.op_Implicit((Object) component))
        {
          Transform transform1 = ((Component) componentsInChild).get_transform();
          Gizmos.DrawMesh(component.get_sharedMesh(), transform1.get_position(), transform1.get_rotation(), transform1.get_lossyScale());
        }
      }
    }
  }

  public static void DrawBounds(Transform transform)
  {
    Bounds bounds = transform.GetBounds(true, false, true);
    Vector3 lossyScale = transform.get_lossyScale();
    Quaternion rotation = transform.get_rotation();
    Vector3 pos = Vector3.op_Addition(transform.get_position(), Quaternion.op_Multiply(rotation, Vector3.Scale(lossyScale, ((Bounds) ref bounds).get_center())));
    Vector3 size = Vector3.Scale(lossyScale, ((Bounds) ref bounds).get_size());
    GizmosUtil.DrawCube(pos, size, rotation);
    GizmosUtil.DrawWireCube(pos, size, rotation);
  }
}
