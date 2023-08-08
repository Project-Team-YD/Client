using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

public class BinaryConverter
{
    public static void Serialize<T>(T _obj, string _path) where T : new()
    {
        int size = Marshal.SizeOf(_obj);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(_obj, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);

        string path = Path.Combine(Application.dataPath, $"{_path}.min");
        Stream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
        BinaryFormatter serializer = new BinaryFormatter();
        serializer.Serialize(fs, _obj);
        fs.Close();

        //using (Stream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        //{
        //    using (BinaryWriter bw = new BinaryWriter(fs))
        //    {
        //        for (int i = 0; i < size; i++)
        //        {
        //            bw.Write(arr[i]);
        //        }
        //    }
        //}
    }

    public static async UniTask<T> Deserialize<T>(string _path) where T : new()
    {
        //.. TODO :: Apply Addressable
        string path = Path.Combine(Application.dataPath, $"{_path}.min");
        try
        {

            T obj;
            Stream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryFormatter deserializer = new BinaryFormatter();
            obj = (T)deserializer.Deserialize(fs);
            fs.Close();
            //using (Stream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                

                //using (BinaryReader br = new BinaryReader(fs))
                //{
                //    byte[] buffer = br.ReadBytes(Marshal.SizeOf(typeof(T)));
                //    //int size = Marshal.SizeOf(typeof(T));
                //    //if (size > buffer.Length)
                //    //{
                //    //    throw new Exception();
                //    //}


                //    Debug.Log($"SIZE :> {buffer.Length}");

                //    GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

                //    //IntPtr ptr = Marshal.AllocHGlobal(size);
                //    //Marshal.Copy(buffer, 0, ptr, size);
                //    T obj = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
                //    //Marshal.FreeHGlobal(ptr);
                //    handle.Free();

                //    return obj;
                //}
            }



            return obj;
        }
        catch(Exception _ex)
        {
            throw new Exception($"{_ex.Message}");
        }
    }
}
