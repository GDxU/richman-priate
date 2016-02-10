using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.IO;

namespace LitJson
{
    public class JsonExtend
    {
        public static void AddExtentds()
        {
            // Rect exporter
            ExporterFunc<UnityEngine.Rect> rectExporter = new ExporterFunc<UnityEngine.Rect>(JsonExtend.rectexp);
            JsonMapper.RegisterExporter<UnityEngine.Rect>(rectExporter);

            // UnityEngine.Color exporter
            ExporterFunc<UnityEngine.Color> colorExporter = new ExporterFunc<UnityEngine.Color>(JsonExtend.colorexp);
            JsonMapper.RegisterExporter<UnityEngine.Color>(colorExporter);

			// UnityEngine.GameObject exporter
            ExporterFunc<UnityEngine.GameObject> gameObjectExporter = new ExporterFunc<UnityEngine.GameObject>(JsonExtend.gameObjexp);
            JsonMapper.RegisterExporter<UnityEngine.GameObject>(gameObjectExporter);
			
			// UnityEngine.Quaternion exporter
            ExporterFunc<UnityEngine.Quaternion> quaternionExporter = new ExporterFunc<UnityEngine.Quaternion>(JsonExtend.quaternionexp);
            JsonMapper.RegisterExporter<UnityEngine.Quaternion>(quaternionExporter);

            // UnityEngine.Object exporter
            ExporterFunc<UnityEngine.Object> objectExporter = new ExporterFunc<UnityEngine.Object>(JsonExtend.objectexp);
            JsonMapper.RegisterExporter<UnityEngine.Object>(objectExporter);

            // Vector4 exporter
            ExporterFunc<Vector4> vector4Exporter = new ExporterFunc<Vector4>(JsonExtend.vector4exp);
            JsonMapper.RegisterExporter<Vector4>(vector4Exporter);

            // Vector3 exporter
            ExporterFunc<Vector3> vector3Exporter = new ExporterFunc<Vector3>(JsonExtend.vector3exp);
            JsonMapper.RegisterExporter<Vector3>(vector3Exporter);

            // Vector2 exporter
            ExporterFunc<Vector2> vector2Exporter = new ExporterFunc<Vector2>(JsonExtend.vector2exp);
            JsonMapper.RegisterExporter<Vector2>(vector2Exporter);

            // float to double
            ExporterFunc<float> float2double = new ExporterFunc<float>(JsonExtend.float2double);
            JsonMapper.RegisterExporter<float>(float2double);

            // double to float
            ImporterFunc<double, Single> double2float = new ImporterFunc<double, Single>(JsonExtend.double2float);
            JsonMapper.RegisterImporter<double, Single>(double2float);
        }

        public static void rectexp(UnityEngine.Rect value, JsonWriter writer)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("x");
            writer.Write(value.x);
            writer.WritePropertyName("y");
            writer.Write(value.y);
            writer.WritePropertyName("width");
            writer.Write(value.width);
            writer.WritePropertyName("height");
            writer.Write(value.height);
            writer.WriteObjectEnd();
        }

        public static void colorexp(UnityEngine.Color value, JsonWriter writer)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("r");
            writer.Write(value.r);
            writer.WritePropertyName("g");
            writer.Write(value.g);
            writer.WritePropertyName("b");
            writer.Write(value.b);
            writer.WritePropertyName("a");
            writer.Write(value.a);
            writer.WriteObjectEnd();
        }
		
		public static void gameObjexp(UnityEngine.GameObject value, JsonWriter writer)
        {
			writer.Write(null);
        }
		
		public static void quaternionexp(UnityEngine.Quaternion value, JsonWriter writer)
        {
			writer.WriteObjectStart();
			writer.WritePropertyName("x");
            writer.Write(value.x);
            writer.WritePropertyName("y");
            writer.Write(value.y);
            writer.WritePropertyName("z");
            writer.Write(value.z);
            writer.WritePropertyName("w");
            writer.Write(value.w);
            writer.WriteObjectEnd();
        }

        public static void objectexp(UnityEngine.Object value, JsonWriter writer)
        {
			writer.Write(null);
        }

        public static void vector4exp(Vector4 value, JsonWriter writer)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("x");
            writer.Write(value.x);
            writer.WritePropertyName("y");
            writer.Write(value.y);
            writer.WritePropertyName("z");
            writer.Write(value.z);
            writer.WritePropertyName("w");
            writer.Write(value.w);
            writer.WriteObjectEnd();
        }

        public static void vector3exp(Vector3 value, JsonWriter writer)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("x");
            writer.Write(value.x);
            writer.WritePropertyName("y");
            writer.Write(value.y);
            writer.WritePropertyName("z");
            writer.Write(value.z);
            writer.WriteObjectEnd();
        }

        public static void vector2exp(Vector2 value, JsonWriter writer)
        {
            writer.WriteObjectStart();
            writer.WritePropertyName("x");
            writer.Write(value.x);
            writer.WritePropertyName("y");
            writer.Write(value.y);
            writer.WriteObjectEnd();
        }

        public static void float2double(float value, JsonWriter writer)
        {
            writer.Write((double)value);
        }

        public static System.Single double2float(double value)
        {
            return (System.Single)value;
        }

        /// <summary>
        /// Load file, parse and return object;
        /// </summary>
        public static T Load<T>(string path)
        {
            try
            {
                if (File.Exists(path) == false)
	            {
                    DebugExt.LogError("file " + path + " doesn't exist!");
                    return default(T);
	            }
            	
	            string str = string.Empty;
                using (StreamReader sr = new StreamReader(path))
	            {
		            str = sr.ReadToEnd();
		            sr.Dispose();
	            }

                //Debug.LogWarning("   JsonExtend.Load()  = " + str);

	            return JsonMapper.ToObject<T>(str);
            }

            catch (Exception Ex)
            {
                DebugExt.LogError(Ex.ToString());
                return default(T);
            }
        }
        
        public static void Save<T>(T toJson, string path)
        {
            try
            {
				StreamWriter sw = null;
				sw = new StreamWriter(path);    //fix a bug in running time;
				sw.Write(JsonMapper.ToJson(toJson));
		        sw.Close();
            }

            catch (Exception Ex)
            {
                DebugExt.LogError(Ex.ToString());
            }
        }
    }
}
