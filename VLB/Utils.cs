// Decompiled with JetBrains decompiler
// Type: VLB.Utils
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F8974325-DA02-4276-BDFF-E336241E9635
// Assembly location: C:\Users\jacob\Downloads\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace VLB
{
  public static class Utils
  {
    private static Utils.FloatPackingPrecision ms_FloatPackingPrecision;
    private const int kFloatPackingHighMinShaderLevel = 35;

    public static string GetPath(Transform current)
    {
      if (Object.op_Equality((Object) current.get_parent(), (Object) null))
        return "/" + ((Object) current).get_name();
      return Utils.GetPath(current.get_parent()) + "/" + ((Object) current).get_name();
    }

    public static T NewWithComponent<T>(string name) where T : Component
    {
      return new GameObject(name, new Type[1]{ typeof (T) }).GetComponent<T>();
    }

    public static T GetOrAddComponent<T>(this GameObject self) where T : Component
    {
      T obj = self.GetComponent<T>();
      if (Object.op_Equality((Object) (object) obj, (Object) null))
        obj = self.AddComponent<T>();
      return obj;
    }

    public static T GetOrAddComponent<T>(this MonoBehaviour self) where T : Component
    {
      return ((Component) self).get_gameObject().GetOrAddComponent<T>();
    }

    public static bool HasFlag(this Enum mask, Enum flags)
    {
      return ((int) mask & (int) flags) == (int) flags;
    }

    public static Vector2 xy(this Vector3 aVector)
    {
      return new Vector2((float) aVector.x, (float) aVector.y);
    }

    public static Vector2 xz(this Vector3 aVector)
    {
      return new Vector2((float) aVector.x, (float) aVector.z);
    }

    public static Vector2 yz(this Vector3 aVector)
    {
      return new Vector2((float) aVector.y, (float) aVector.z);
    }

    public static Vector2 yx(this Vector3 aVector)
    {
      return new Vector2((float) aVector.y, (float) aVector.x);
    }

    public static Vector2 zx(this Vector3 aVector)
    {
      return new Vector2((float) aVector.z, (float) aVector.x);
    }

    public static Vector2 zy(this Vector3 aVector)
    {
      return new Vector2((float) aVector.z, (float) aVector.y);
    }

    public static float GetVolumeCubic(this Bounds self)
    {
      return (float) (((Bounds) ref self).get_size().x * ((Bounds) ref self).get_size().y * ((Bounds) ref self).get_size().z);
    }

    public static float GetMaxArea2D(this Bounds self)
    {
      return Mathf.Max(Mathf.Max((float) (((Bounds) ref self).get_size().x * ((Bounds) ref self).get_size().y), (float) (((Bounds) ref self).get_size().y * ((Bounds) ref self).get_size().z)), (float) (((Bounds) ref self).get_size().x * ((Bounds) ref self).get_size().z));
    }

    public static Color Opaque(this Color self)
    {
      return new Color((float) self.r, (float) self.g, (float) self.b, 1f);
    }

    public static void GizmosDrawPlane(Vector3 normal, Vector3 position, Color color, float size = 1f)
    {
      Vector3 vector3_1 = Vector3.Cross(normal, (double) Mathf.Abs(Vector3.Dot(normal, Vector3.get_forward())) < 0.999000012874603 ? Vector3.get_forward() : Vector3.get_up());
      Vector3 vector3_2 = Vector3.op_Multiply(((Vector3) ref vector3_1).get_normalized(), size);
      Vector3 vector3_3 = Vector3.op_Addition(position, vector3_2);
      Vector3 vector3_4 = Vector3.op_Subtraction(position, vector3_2);
      Vector3 vector3_5 = Quaternion.op_Multiply(Quaternion.AngleAxis(90f, normal), vector3_2);
      Vector3 vector3_6 = Vector3.op_Addition(position, vector3_5);
      Vector3 vector3_7 = Vector3.op_Subtraction(position, vector3_5);
      Gizmos.set_matrix(Matrix4x4.get_identity());
      Gizmos.set_color(color);
      Gizmos.DrawLine(vector3_3, vector3_4);
      Gizmos.DrawLine(vector3_6, vector3_7);
      Gizmos.DrawLine(vector3_3, vector3_6);
      Gizmos.DrawLine(vector3_6, vector3_4);
      Gizmos.DrawLine(vector3_4, vector3_7);
      Gizmos.DrawLine(vector3_7, vector3_3);
    }

    public static Plane TranslateCustom(this Plane plane, Vector3 translation)
    {
      ref Plane local = ref plane;
      ((Plane) ref local).set_distance(((Plane) ref local).get_distance() + Vector3.Dot(((Vector3) ref translation).get_normalized(), ((Plane) ref plane).get_normal()) * ((Vector3) ref translation).get_magnitude());
      return plane;
    }

    public static bool IsValid(this Plane plane)
    {
      Vector3 normal = ((Plane) ref plane).get_normal();
      return (double) ((Vector3) ref normal).get_sqrMagnitude() > 0.5;
    }

    public static Matrix4x4 SampleInMatrix(this Gradient self, int floatPackingPrecision)
    {
      Matrix4x4 matrix4x4 = (Matrix4x4) null;
      for (int index = 0; index < 16; ++index)
      {
        Color color = self.Evaluate(Mathf.Clamp01((float) index / 15f));
        ((Matrix4x4) ref matrix4x4).set_Item(index, color.PackToFloat(floatPackingPrecision));
      }
      return matrix4x4;
    }

    public static Color[] SampleInArray(this Gradient self, int samplesCount)
    {
      Color[] colorArray = new Color[samplesCount];
      for (int index = 0; index < samplesCount; ++index)
        colorArray[index] = self.Evaluate(Mathf.Clamp01((float) index / (float) (samplesCount - 1)));
      return colorArray;
    }

    private static Vector4 Vector4_Floor(Vector4 vec)
    {
      return new Vector4(Mathf.Floor((float) vec.x), Mathf.Floor((float) vec.y), Mathf.Floor((float) vec.z), Mathf.Floor((float) vec.w));
    }

    public static float PackToFloat(this Color color, int floatPackingPrecision)
    {
      Vector4 vector4 = Utils.Vector4_Floor(Color.op_Implicit(Color.op_Multiply(color, (float) (floatPackingPrecision - 1))));
      return (float) (0.0 + vector4.x * (double) floatPackingPrecision * (double) floatPackingPrecision * (double) floatPackingPrecision + vector4.y * (double) floatPackingPrecision * (double) floatPackingPrecision + vector4.z * (double) floatPackingPrecision + vector4.w);
    }

    public static Utils.FloatPackingPrecision GetFloatPackingPrecision()
    {
      if (Utils.ms_FloatPackingPrecision == Utils.FloatPackingPrecision.Undef)
        Utils.ms_FloatPackingPrecision = SystemInfo.get_graphicsShaderLevel() >= 35 ? Utils.FloatPackingPrecision.High : Utils.FloatPackingPrecision.Low;
      return Utils.ms_FloatPackingPrecision;
    }

    public static void MarkCurrentSceneDirty()
    {
    }

    public enum FloatPackingPrecision
    {
      Undef = 0,
      Low = 8,
      High = 64, // 0x00000040
    }
  }
}
