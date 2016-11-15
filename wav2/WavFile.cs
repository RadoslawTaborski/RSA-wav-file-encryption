using System;

namespace wav2
{
    public class WavFile
    {
        public string ChrunkID { get; private set; }
        public int ChrunkSize { get; private set; }
        public string Format { get; private set; }
        public string Subchunk1ID { get; private set; }
        public int Subchunk1Size { get; private set; }
        public int AudioFormat { get; private set; }
        public int NumChanels { get; private set; }
        public int SampleRate { get; private set; }
        public int ByteRate { get; private set; }
        public int BlockAlign { get; private set; }
        public int BitsPerSample { get; private set; }
        public string Subchunk2ID { get; private set; }
        public int Subchunk2Size { get; private set; }
        public float[,] Data { get; private set; }

        public WavFile(byte[] bytes)
        {
            ChrunkID = readString(bytes, 0, 4);
            ChrunkSize = readValue(bytes, 4, 8);
            Format = readString(bytes, 8, 12);
            Subchunk1ID = readString(bytes, 12, 16);
            Subchunk1Size = readValue(bytes, 16, 20);
            AudioFormat = readValue(bytes, 20, 22);
            NumChanels = readValue(bytes, 22, 24);
            SampleRate = readValue(bytes, 24, 28);
            ByteRate = readValue(bytes, 28, 32);
            BlockAlign = readValue(bytes, 32, 34);
            BitsPerSample = readValue(bytes, 34, 36);
            Subchunk2ID = readString(bytes, 36, 40);
            Subchunk2Size = readValue(bytes, 40, 44);
            Data = new float[NumChanels, Subchunk2Size / 2 / NumChanels];
            int index = 0;
            int nr = 44;
            while (nr < Subchunk2Size + 42)
            {
                for (var i = 0; i < NumChanels; ++i)
                {
                    Data[i, index] = readFloat2(bytes, nr, nr + 2);
                    nr += 2;
                    if (nr >= Subchunk2Size + 42)
                        break;
                    // Console.WriteLine(Data[i, index]);
                }
                index++;
            }
        }

        private static Int16 getSample(byte[] buffer, int position)
        {
            return (Int16)(((buffer[position + 1] & 0xff) << 8) | (buffer[position] & 0xff));
        }

        private float readFloat(byte[] bytes, int start)
        {
            return (float)getSample(bytes, start) / (float)Int16.MaxValue;
        }

        private float readFloat2(byte[] bytes, int start, int end)
        {
            Int16 result = 0;
            int index = 0;
            var hexArray = new string[end - start];
            for (int i = start; i < end; i++)
            {
                hexArray[index] = bytes[i].ToString("X2");
                index++;
            }

            string sizeStr = "";
            foreach (var str in hexArray)
            {
                sizeStr = str + sizeStr;
            }
            result = Convert.ToInt16(sizeStr, 16);

            float f = ((float)result) / (float)Int16.MaxValue;
            if (f > 1) f = 1;
            if (f < -1) f = -1;

            return f;
        }

        private string readString(byte[] bytes, int start, int end)
        {
            string result = "";
            for (int i = start; i < end; i++)
            {
                result = result + Convert.ToChar(bytes[i]);
            }
            return result;
        }

        private int readValue(byte[] bytes, int start, int end)
        {
            int result = 0;
            int index = 0;
            var hexArray = new string[end - start];
            for (int i = start; i < end; i++)
            {
                hexArray[index] = bytes[i].ToString("X2");
                index++;
            }

            string sizeStr = "";
            foreach (var str in hexArray)
            {
                sizeStr = str + sizeStr;
            }
            result = Convert.ToInt32(sizeStr, 16);

            return result;
        }

        public void Header()
        {
            Console.WriteLine("ChrunkID: " + ChrunkID);
            Console.WriteLine("ChrunkSize: " + ChrunkSize.ToString());
            Console.WriteLine("Format: " + Format);
            Console.WriteLine("Subchunk1ID: " + Subchunk1ID);
            Console.WriteLine("Subchunk1Size: " + Subchunk1Size.ToString());
            Console.WriteLine("AudioFormat: " + AudioFormat.ToString());
            Console.WriteLine("NumChanels: " + NumChanels.ToString());
            Console.WriteLine("SampleRate: " + SampleRate.ToString());
            Console.WriteLine("ByteRate: " + ByteRate.ToString());
            Console.WriteLine("BlockAlign: " + BlockAlign.ToString());
            Console.WriteLine("BitsPerSample: " + BitsPerSample.ToString());
            Console.WriteLine("Subchunk2ID: " + Subchunk2ID);
            Console.WriteLine("Subchunk2Size: " + Subchunk2Size.ToString());

            /*for (var j = 0; j < Subchunk2Size / 2 / NumChanels; ++j)
                for (var i = 0; i < NumChanels; ++i)
                    Console.WriteLine(Data[i, j].ToString() + " ");*/

            /* for (var j = 0; j < Subchunk2Size / 2 / NumChanels; ++j)
                 Console.WriteLine(Data[0, j].ToString() + " ");
             Console.WriteLine("end");*/
        }
    }
}
