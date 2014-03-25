using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO.Compression;
using System.Runtime.Serialization;

[StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]

public  class MyDataFormatChange :MonoBehaviour{
    /**/
    /// <summary>
    /// 结构体转byte数组
    /// </summary>
    /// <param name="structObj">要转换的结构体</param>
    /// <returns>转换后的byte数组</returns>
    public static byte[] StructToBytes(object structObj)
    {
        int size = Marshal.SizeOf(structObj);
        IntPtr buffer = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(structObj, buffer, false);
            byte[] bytes = new byte[size];
            Marshal.Copy(buffer, bytes, 0, size);
            return bytes;
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /**/
    /// <summary>
    /// byte数组转结构体
    /// </summary>
    /// <param name="bytes">byte数组</param>
    /// <param name="type">结构体类型</param>
    /// <returns>转换后的结构体</returns>
    public static object BytesToStruct(byte[] bytes, Type strcutType)
    {
	      int size = Marshal.SizeOf(strcutType);
		  IntPtr buffer = Marshal.AllocHGlobal(size);
		  try
		  {
		  Marshal.Copy(bytes, 0, buffer, size);
		  return Marshal.PtrToStructure(buffer, strcutType);
		  }
		  finally
		  {
		  Marshal.FreeHGlobal(buffer);
		  }
    }
	
//	  public unsafe GameStruct.MSG_GET_ONE_MJ xamBytesToStruct( byte[] arr ) {
//   if( arr.Length < sizeof(GameStruct.MSG_GET_ONE_MJ) )
//    throw new ArgumentException();
//
//   GameStruct.MSG_GET_ONE_MJ s;
//   fixed( byte* parr = arr ) {
//   s = *((GameStruct.MSG_GET_ONE_MJ*)parr); }
//   return s;
//  }
//  
	
//	 public static unsafe byte[] SaveMyStruct(MyStruct* st)
//        {
//            int len = sizeof(MyStruct);
//            byte[] buf = new byte[len];
//            byte* p = (byte*)st;
//            for (int i = 0; i < len; i++)
//            {
//                buf[i] = *p++;
//            }
//            return buf;
//        }
//
//   static unsafe MyStruct ReadMyStruct(byte[] buf)
//        {
//            MyStruct st = new MyStruct();
//            byte* p = (byte*)&st;
//            int len = sizeof(MyStruct);
//            for (int i = 0; i < len; i++)
//            {
//                *p++ = buf[i];
//            }
//            return st;
//        }   
	
//	public static unsafe Object ReadMyStruct(ref T fac,byte[] bytes)
//    {
//       Object ts = Activator.CreateInstance(fac.GetType());
//		return ts;
//	}
	
//	public static byte[] StructToBytes(object obj)
//	{
//		using (MemoryStream ms = new MemoryStream())
//		{
//		IFormatter formatter = new BinaryFormatter();
//		formatter.Serialize(ms, obj);
//		return ms.GetBuffer();
//		}
//	}
//	
//	public static object BytesToStruct(byte[] Bytes, Type strcutType)
//	{
//		using (MemoryStream ms = new MemoryStream(Bytes))
//		{
//		IFormatter formatter = new BinaryFormatter();
//		return formatter.Deserialize(ms);
//		}
//	}
	
	 /// <summary>
         /// 结构体转字节数组（按大端模式）
         /// </summary>
         /// <param name="obj">struct type</param>
    /// <returns></returns>
    public static byte[] StructToBytesEX(object obj)
    {
        object thisBoxed = obj;   //copy ，将 struct 装箱
        Type test = thisBoxed.GetType();

        int offset = 0;
        byte[] data = new byte[Marshal.SizeOf(thisBoxed)];

        object fieldValue;
        TypeCode typeCode;
        byte[] temp;
        // 列举结构体的每个成员，并Reverse
        foreach (var field in test.GetFields())
        {
            fieldValue = field.GetValue(thisBoxed); // Get value

            typeCode = Type.GetTypeCode(fieldValue.GetType());  // get type
            switch (typeCode)
            {
                case TypeCode.Single: // float
                    {
                        temp = BitConverter.GetBytes((Single)fieldValue);
                        //Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(Single));
                        break;
                    }
                case TypeCode.Int32:
                    {
                        temp = BitConverter.GetBytes((Int32)fieldValue);
                        //  Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(Int32));
                        break;
                    }
                case TypeCode.UInt32:
                    {
                        temp = BitConverter.GetBytes((UInt32)fieldValue);
                        //Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(UInt32));
                        break;
                    }
                case TypeCode.Int16:
                    {
                        temp = BitConverter.GetBytes((Int16)fieldValue);
                        // Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(Int16));
                        break;
                    }
                case TypeCode.UInt16:
                    {
                        temp = BitConverter.GetBytes((UInt16)fieldValue);
                        //Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(UInt16));
                        break;
                    }
                case TypeCode.Int64:
                    {
                        temp = BitConverter.GetBytes((Int64)fieldValue);
                        //  Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(Int64));
                        break;
                    }
                case TypeCode.UInt64:
                    {
                        temp = BitConverter.GetBytes((UInt64)fieldValue);
                        // Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(UInt64));
                        break;
                    }
                case TypeCode.Double:
                    {
                        temp = BitConverter.GetBytes((Double)fieldValue);
                        // Array.Reverse(temp);
                        Array.Copy(temp, 0, data, offset, sizeof(Double));
                        break;
                    }
                case TypeCode.Byte:
                    {
                        data[offset] = (Byte)fieldValue;
                        break;
                    }
                default:
                    {
                        //System.Diagnostics.Debug.Fail("No conversion provided for this type : " + typeCode.ToString());
                        break;
                    }
            }; // switch
            if (typeCode == TypeCode.Object)
            {
                //                     int length = ((byte[])fieldValue).Length;
                //                     Array.Copy(((byte[])fieldValue), 0, data, offset, length);
                if (fieldValue is Array)
                {
                    //print((fieldValue as Array).Length);
                    temp = ArrayToByteArray(fieldValue);
                    Array.Copy(temp, 0, data, offset, temp.Length);
                    offset += temp.Length;
                }
                else
                {
                    temp = StructToBytesEX(fieldValue);
                    Array.Copy(temp, 0, data, offset, temp.Length);
                    offset += temp.Length;
                }

            }
            else
            {
                offset += Marshal.SizeOf(fieldValue);
            }
        } // foreach
        return data;
    } // Swap
	
	
	public static byte[] ArrayToByteArray(object obj)
	{
		byte[] datatmp=new byte[2048];
		byte[] data;
		byte[] temp;
		int offset=0;
		object thisBoxed = obj;   //copy ，将 struct 装箱
        Type test = thisBoxed.GetType();
 		switch(test.ToString())
		{
          case "System.Single[]": // float
             {
                foreach(Single element in (Single[])thisBoxed)
				{
					temp=BitConverter.GetBytes(element);
					Array.Copy(temp, 0, datatmp, offset, temp.Length);
				    offset+=temp.Length;
				}
                 break;
             }
         case "System.Int32[]":
             {
				foreach(Int32 element in (Int32[])thisBoxed)
				{
					temp=BitConverter.GetBytes(element);
					Array.Copy(temp, 0, datatmp, offset, temp.Length);
				    offset+=temp.Length;
				}
                 break;
             }
         case "System.UInt32[]":
             {
                 foreach (UInt32 element in (UInt32[])thisBoxed)
                 {
                     temp = BitConverter.GetBytes(element);
                     Array.Copy(temp, 0, datatmp, offset, temp.Length);
                     offset += temp.Length;
                 }
                 break;
             }
         case "System.Int16[]":
             {
                 foreach (Int16 element in (Int16[])thisBoxed)
                 {
                     temp = BitConverter.GetBytes(element);
                     Array.Copy(temp, 0, datatmp, offset, temp.Length);
                     offset += temp.Length;
                 }
                 break;
             }
         case "System.UInt16[]":
             {
                 foreach (UInt16 element in (UInt16[])thisBoxed)
                 {
                     temp = BitConverter.GetBytes(element);
                     Array.Copy(temp, 0, datatmp, offset, temp.Length);
                     offset += temp.Length;
                 }
                 break;
             }
         case "System.Int64[]":
             {
                 foreach (Int64 element in (Int64[])thisBoxed)
                 {
                     temp = BitConverter.GetBytes(element);
                     Array.Copy(temp, 0, datatmp, offset, temp.Length);
                     offset += temp.Length;
                 }
                 break;
             }
         case "System.UInt64[]":
             {
                 foreach (UInt64 element in (UInt64[])thisBoxed)
                 {
                     temp = BitConverter.GetBytes(element);
                     Array.Copy(temp, 0, datatmp, offset, temp.Length);
                     offset += temp.Length;
                 }
                 break;
             }
         case "System.Double[]":
             {
                 foreach (Double element in (Double[])thisBoxed)
                 {
                     temp = BitConverter.GetBytes(element);
                     Array.Copy(temp, 0, datatmp, offset, temp.Length);
                     offset += temp.Length;
                 }
                 break;
             }
         case "System.Byte[]":
             {
                 foreach (byte element in (byte[])thisBoxed)
                 {
                     temp = new byte[1];
                     temp[0] = element;
                     Array.Copy(temp, 0, datatmp, offset, 1);
                     offset += temp.Length;
                 }
                 break;
             }
         default:
             {
                 System.Array array = thisBoxed as Array;
                 int elementLength = Marshal.SizeOf(array.GetValue(0).GetType());
                 temp = new byte[elementLength];
                 for (int j = 0; j < array.Length; j++)
                 {
                     temp = StructToBytesEX(array.GetValue(j));
                     Array.Copy(temp, 0, datatmp, offset, temp.Length);
                     offset += temp.Length;
                 }

                 break;
             }
		}

       
		data=new byte[offset];
		Array.Copy(datatmp, 0, data, 0,offset);
		return data;
		
	}

    public static object BytesToStructEX(object fac, byte[] bytes)
    {
        object thisBoxed = fac;//Activator.CreateInstance(fac.GetType());
        LoopSetValue(ref thisBoxed, bytes);

        return thisBoxed;

    }

    private static void LoopSetValue(ref object thisBoxed,byte[] bytes)
    {
        Type test = thisBoxed.GetType();
        int offset = 0;
        // byte[] data = new byte[Marshal.SizeOf(thisBoxed)];

        object fieldValue;
        TypeCode typeCode;
        byte[] temp;
        // 列举结构体的每个成员，并Reverse
        //foreach (var field in test.GetFields())
        foreach (System.Reflection.FieldInfo field in test.GetFields())
        {
            fieldValue = field.GetValue(thisBoxed); // Get value
            typeCode = Type.GetTypeCode(fieldValue.GetType());  // get type
            switch (typeCode)
            {

                case TypeCode.Single: // float
                    {
                        Single s = BitConverter.ToSingle(bytes, offset);
                        field.SetValue(thisBoxed, s);
                        offset += 4;
                        break;
                    }
                case TypeCode.Int32:
                    {
                        Int32 s = BitConverter.ToInt32(bytes, offset);
                        field.SetValue(thisBoxed, s);
                        offset += 4;
                        break;
                    }
                case TypeCode.UInt32:
                    {
                        UInt32 s = BitConverter.ToUInt32(bytes, offset);
                        field.SetValue(thisBoxed, s);
                        offset += 4;
                        break;
                    }
                case TypeCode.Int16:
                    {
                        Int16 s = BitConverter.ToInt16(bytes, offset);
                        field.SetValue(thisBoxed, s);
                        offset += 2;
                        break;
                    }
                case TypeCode.UInt16:
                    {
                        UInt16 s = BitConverter.ToUInt16(bytes, offset);
                        field.SetValue(thisBoxed, s);
                        offset += 2;
                        break;
                    }
                case TypeCode.Int64:
                    {
                        Int64 s = BitConverter.ToInt64(bytes, offset);
                        field.SetValue(thisBoxed, s);
                        offset += 8;
                        break;
                    }
                case TypeCode.UInt64:
                    {
                        UInt64 s = BitConverter.ToUInt64(bytes, offset);
                        field.SetValue(thisBoxed, s);
                        offset += 8;
                        break;
                    }
                case TypeCode.Double:
                    {
                        Double s = BitConverter.ToDouble(bytes, offset);
                        field.SetValue(thisBoxed, s);
                        offset += 8;
                        break;
                    }
                case TypeCode.Byte:
                    {
                        Byte s = bytes[offset];
                        field.SetValue(thisBoxed, s);
                        offset += 1;
                        break;
                    }
                case TypeCode.Object:
                    {

                        if (fieldValue is Array)
                        {
                            System.Array array = fieldValue as Array;

                            int num = array.Length;

                            int aaaa = 10;
                            int elementLength = Marshal.SizeOf(array.GetValue(0).GetType());
                            byte[] bs = new byte[elementLength];
                            for (int j = 0; j < array.Length; j++)
                            {
                                Array.Copy(bytes, offset, bs, 0, elementLength);
                                object obj = array.GetValue(j);
                                switch (Type.GetTypeCode(array.GetValue(j).GetType()))
                                {

                                    case TypeCode.Single: // float
                                        {
                                            Single s = BitConverter.ToSingle(bs, 0);
                                            array.SetValue(s, j);
                                            break;
                                        }
                                    case TypeCode.Int32:
                                        {
                                            Int32 s = BitConverter.ToInt32(bs, 0);
                                            array.SetValue(s, j);
                                            break;
                                        }
                                    case TypeCode.UInt32:
                                        {
                                            UInt32 s = BitConverter.ToUInt32(bs, 0);
                                            array.SetValue(s, j);
                                            break;
                                        }
                                    case TypeCode.Int16:
                                        {
                                            Int16 s = BitConverter.ToInt16(bs, 0);
                                            array.SetValue(s, j);
                                            break;
                                        }
                                    case TypeCode.UInt16:
                                        {
                                            UInt16 s = BitConverter.ToUInt16(bs, 0);
                                            array.SetValue(s, j);
                                            break;
                                        }
                                    case TypeCode.Int64:
                                        {
                                            Int64 s = BitConverter.ToInt64(bs, 0);
                                            array.SetValue(s, j);
                                            break;
                                        }
                                    case TypeCode.UInt64:
                                        {
                                            UInt64 s = BitConverter.ToUInt64(bs, 0);
                                            array.SetValue(s, j);
                                            break;
                                        }
                                    case TypeCode.Double:
                                        {
                                            Double s = BitConverter.ToDouble(bs, 0);
                                            array.SetValue(s, j);
                                            break;
                                        }
                                    case TypeCode.Byte:
                                        {
                                            array.SetValue(bs[0],j);
                                            break;
                                        }
                                    case TypeCode.Object:
                                        {
                                            LoopSetValue(ref obj, bs);
                                            break;
                                        }
                                }

                                offset += elementLength;
                            }

                        }
                        else
                        {
                           // BytesToStructEX(object fac, byte[] bytes)
                           // print("bbbbb" + Marshal.SizeOf(fieldValue) + fieldValue.ToString());

                            int elementLength = Marshal.SizeOf(fieldValue);
                            byte[] bs = new byte[elementLength];
                            Array.Copy(bytes, offset, bs, 0, elementLength);
                            offset += elementLength;
                            LoopSetValue(ref fieldValue,bs);
                            //field = BytesToStructEX(field, bs);
                            //field.SetValue(thisBoxed, s);
                            
      
                        }
                    }
                    break;
                default:
                    {
                        //System.Diagnostics.Debug.Fail("No conversion provided for this type : " + typeCode.ToString());
                        break;
                    }

            }
        }
    }

    public static int GetHeaderLength(object o)
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        formatter.Serialize(stream, o);
        return (int)stream.Length;
    }
	
	/// <summary>
        /// 字节数组转结构体(按大端模式)
        /// </summary>
        /// <param name="bytearray">字节数组</param>
        /// <param name="obj">目标结构体</param>
        /// <param name="startoffset">bytearray内的起始位置</param>
        public static void ByteArrayToStructureEndian(byte[] bytearray, ref object obj, int startoffset)
        {
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
            byte[] temparray = (byte[])bytearray.Clone();
            // 从结构体指针构造结构体
            obj = Marshal.PtrToStructure(i, obj.GetType());
            // 做大端转换
            object thisBoxed = obj;
            Type test = thisBoxed.GetType();
            int reversestartoffset = startoffset;
            // 列举结构体的每个成员，并Reverse
            foreach (var field in test.GetFields())
            {
                object fieldValue = field.GetValue(thisBoxed); // Get value
                
                TypeCode typeCode = Type.GetTypeCode(fieldValue.GetType());  //Get Type
                if (typeCode != TypeCode.Object)  //如果为值类型
                {
                    Array.Reverse(temparray, reversestartoffset, Marshal.SizeOf(fieldValue));
                    reversestartoffset += Marshal.SizeOf(fieldValue);
                }
                else  //如果为引用类型
                {
                    reversestartoffset += ((byte[])fieldValue).Length;
                }
            }
            try
            {
                //将字节数组复制到结构体指针
                Marshal.Copy(temparray, startoffset, i, len);
            }
            catch (Exception ex)
			{ 
				Console.WriteLine("ByteArrayToStructure FAIL: error " + ex.ToString());
			}
            obj = Marshal.PtrToStructure(i, obj.GetType());
            Marshal.FreeHGlobal(i);  //释放内存
        }


    /// <summary>
    /// 返回data1和data2合拼的byte[]
    /// </summary>
    /// <param name="data1"></param>
    /// <param name="data2"></param>
    /// <returns></returns>
    public static byte[] ConnectBytes(byte[] data1, byte[] data2)
    {
        byte[] nCon = null;
        if (data2 == null)
        {
            nCon = new byte[data1.Length];
            data1.CopyTo(nCon, 0);
        }
        else
        {
            nCon = new byte[data1.Length + data2.Length];
            data1.CopyTo(nCon, 0);
            data2.CopyTo(nCon, data1.Length);
        }

       
        return nCon;

    }

    public static void WriteToFile(string fileName,byte[] data)
    {
        try
        {
            FileStream fs = new FileStream(fileName,FileMode.Create);
            fs.Write(data,0,data.Length);
            fs.Flush();
            fs.Close();
        }
        catch
        {
            Debug.Log("写文件失败");
        }
    }

    public static void ReadFromFile(string fileName,ref byte[] data)
    {
        try
        {
            FileInfo fi = new FileInfo(fileName);
            if (fi.Exists)
            {
                FileStream fs = new FileStream(fileName, FileMode.Open);
                fs.Read(data, 0, (int)(fi.Length));
                fs.Close();
            } 
            else
            {
                Debug.Log("文件不存在");
            }
    
        }
        catch
        {
            Debug.Log("读文件失败");
        }
    }

    public static void WriteToFile(string fileName, string data)
    {

        MyDataFormatChange.WriteToFile(fileName,MyDataFormatChange.stringToByte(data));
    }

    ///// <summary>
    ///// 压缩string为Gzip格式
    ///// </summary>
    ///// <param name="inBytes"></param>
    ///// <returns></returns>
    //public static byte[] CompressByte(string str)
    //{

    //    return MyDataFormatChange.CompressByte(MyDataFormatChange.stringToByte(str));
    //}

    ///// <summary>
    ///// 压缩byte数组为Gzip格式
    ///// </summary>
    ///// <param name="inBytes"></param>
    ///// <returns></returns>
    //public static byte[] CompressByte(byte[] inBytes)
    //{
    //    MemoryStream outStream = new MemoryStream();
    //    Stream zipStream = new GZipOutputStream(outStream);
    //    zipStream.Write(inBytes, 0, inBytes.Length);
    //    zipStream.Close();
    //    byte[] outData = outStream.ToArray();
    //    outStream.Close();
    //    return outData;
    //}


    ///// <summary>
    ///// 解压缩byte数组
    ///// </summary>
    ///// <param name="inBytes">需要解压缩的byte数组</param>
    ///// <returns></returns>
    //public static byte[] DecompressByte(byte[] inBytes)
    //{
    //    byte[] writeData = new byte[2048];
    //    MemoryStream inStream = new MemoryStream(inBytes);
    //    Stream zipStream = new GZipInputStream(inStream) as Stream;
    //    MemoryStream outStream = new MemoryStream();
    //    while (true)
    //    {
    //        int size = zipStream.Read(writeData, 0, writeData.Length);
    //        if (size > 0)
    //        {
    //            outStream.Write(writeData, 0, size);
    //        }
    //        else
    //        {
    //            break;
    //        }
    //    }
    //    zipStream.Close();
    //    byte[] outData = outStream.ToArray();
    //    outStream.Close();
    //    return outData;
    //}


    ////压缩字符串
    //public static string ZipString(string unCompressedString)
    //{

    //    byte[] bytData = System.Text.Encoding.UTF8.GetBytes(unCompressedString);
    //    MemoryStream ms = new MemoryStream();
    //    Stream s = new GZipStream(ms, CompressionMode.Compress);
    //    s.Write(bytData, 0, bytData.Length);
    //    s.Close();
    //    byte[] compressedData = (byte[])ms.ToArray();
    //    return System.Convert.ToBase64String(compressedData, 0, compressedData.Length);
    //}

    ////解压缩字符串

    //public static string UnzipString(string unCompressedString)
    //{
    //    System.Text.StringBuilder uncompressedString = new System.Text.StringBuilder();
    //    byte[] writeData = new byte[4096];
    //    byte[] bytData = System.Convert.FromBase64String(unCompressedString);
    //    int totalLength = 0;
    //    int size = 0;
    //    Stream s = new GZipStream(new MemoryStream(bytData), CompressionMode.Decompress);
    //    while (true)
    //    {
    //        size = s.Read(writeData, 0, writeData.Length);
    //        if (size > 0)
    //        {
    //            totalLength += size;
    //            uncompressedString.Append(System.Text.Encoding.UTF8.GetString(writeData, 0, size));
    //        }
    //        else
    //        {
    //            break;
    //        }
    //    }
    //    s.Close();
    //    return uncompressedString.ToString();
    //}


 

     /// <summary>
    /// 字符串转字节数组
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static byte[] stringToByte(string str)
    {
        return System.Text.Encoding.Default.GetBytes(str);
    }

    /// <summary>
    /// 字节数组转字符串
    /// </summary>
    /// <param name="byteArray"></param>
    /// <returns></returns>
    public static string ByteToString(byte[] byteArray)
    {
        return System.Text.Encoding.Default.GetString(byteArray);
    }

    /// <summary>
    /// int 转byte数组+
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static byte[] intToByte(int i)
    {
        byte[] bt = System.BitConverter.GetBytes(i);
        return bt;
    }



    /// <summary>
    /// byte数组转int
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static int byteToInt(byte[] b)
    {
        int n = System.BitConverter.ToInt32(b, 0);
        return n;
    }

    /// <summary>
    /// float 转byte数组+
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static byte[] floatToByte(float i)
    {
        byte[] bt = System.BitConverter.GetBytes(i);
        return bt;
    }

    /// <summary>
    /// byte数组转float
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float byteTofloat(byte[] b)
    {
        float n = System.BitConverter.ToSingle(b, 0);
        return n;
    } 
}
