using System;
using System.IO;
using System.Linq;
using System.Text;

namespace PakReader
{
    public static class BinaryExtensions
    {
        public static FStringWithUCS ReadFString(this BinaryReader reader)
        {
            // > 0 for ANSICHAR, < 0 for UCS2CHAR serialization
            var SaveNum = reader.ReadInt32();
            bool LoadUCS2Char = SaveNum < 0;
            if (LoadUCS2Char)
            {
                // If SaveNum cannot be negated due to integer overflow, Ar is corrupted.
                if (SaveNum == int.MinValue)
                {
                    throw new FileLoadException("Archive is corrupted");
                }

                SaveNum = -SaveNum;
            }

            if (SaveNum == 0) return new FStringWithUCS()
            {
                FString = null,
                LoadUCS2Char = false
            };

            // 1 byte is removed because of null terminator (\0)
            if (LoadUCS2Char)
            {
                ushort[] data = new ushort[SaveNum];
                for (int i = 0; i < SaveNum; i++)
                {
                    data[i] = reader.ReadUInt16();
                }
                unsafe
                {
                    fixed (ushort* dataPtr = &data[0])
                        return new FStringWithUCS()
                        {
                            FString = new string((char*) dataPtr, 0, data.Length - 1),
                            LoadUCS2Char = true
                        };
                }
            }
            else
            {
                return new FStringWithUCS()
                {
                    FString = Encoding.UTF8.GetString(reader.ReadBytes(SaveNum).AsSpan(..^1)),
                    LoadUCS2Char = false
            };
            }
        }

        public static void WriteFString(this BinaryWriter writer, FStringWithUCS stringtowrite)
        {
            var SaveNum = stringtowrite.FString.Length;

            var bytes = Encoding.UTF8.GetBytes(stringtowrite.FString);

            if (stringtowrite.LoadUCS2Char) // To produce 1-to-1 copies of .locres
            {
                SaveNum = -SaveNum;
            }

            if (stringtowrite.LoadUCS2Char)
            {
                writer.Write(SaveNum - 1);
                foreach (char chr in stringtowrite.FString)
                {
                    writer.Write((UInt16)chr);
                }
                writer.Write((byte)0);
                writer.Write((byte)0);
            }
            else
            {
                writer.Write(SaveNum + 1);
                writer.Write(bytes);
                writer.Write((byte) 0);
            }
        }

        public static T[] ReadTArray<T>(this BinaryReader reader, Func<T> Getter)
        {
            int SerializeNum = reader.ReadInt32();
            T[] A = new T[SerializeNum];
            for(int i = 0; i < SerializeNum; i++)
            {
                A[i] = Getter(); // read FString, read RefNumber
            }
            return A;
        }

        public struct FStringWithUCS
        {
            public string FString;
            public bool LoadUCS2Char;
            public int RefNumber;
        }
    }
}
