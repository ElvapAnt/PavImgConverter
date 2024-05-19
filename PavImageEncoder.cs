using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PavImgConverter
{
    public class YUV
    {
        public double Y { get; set; }
        public double U { get; set; }
        public double V { get; set; }
    }

    public class ImageProcessor
    {
        private readonly int[,] _kernel = new int[,]
        {
            { 0, 0, 0, 8, 4 },
            { 2, 4, 8, 4, 2 },
            { 1, 2, 4, 2, 1 }
        };

        private readonly int _kernelSum = 42;

        public unsafe YUV[,] ConvertRGBtoYUV(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            YUV[,] yuvImage = new YUV[width, height];

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            byte* p = (byte*)bitmapData.Scan0;

            for (int y = 0; y < height; y++)
            {
                byte* row = p + (y * bitmapData.Stride);
                for (int x = 0; x < width; x++)
                {
                    byte b = row[x * 3];
                    byte g = row[x * 3 + 1];
                    byte r = row[x * 3 + 2];

                    double yValue = 0.299 * r + 0.587 * g + 0.114 * b;
                    double uValue = -0.14713 * r - 0.28886 * g + 0.436 * b;
                    double vValue = 0.615 * r - 0.51499 * g - 0.10001 * b;

                    yuvImage[x, y] = new YUV { Y = yValue, U = uValue, V = vValue };
                }
            }

            bitmap.UnlockBits(bitmapData);
            return yuvImage;
        }

        public (double[,], double[,], double[,]) Downsample(YUV[,] yuvImage)
        {
            int width = yuvImage.GetLength(0);
            int height = yuvImage.GetLength(1);

            double[,] yChannel = new double[width, height];
            double[,] uChannel = new double[(width + 1) / 2, (height + 1) / 2];
            double[,] vChannel = new double[(width + 1) / 2, (height + 1) / 2];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    yChannel[x, y] = yuvImage[x, y].Y;
                    if (x % 2 == 0 && y % 2 == 0)
                    {
                        int uIndexX = x / 2;
                        int uIndexY = y / 2;
                        if (uIndexX < uChannel.GetLength(0) && uIndexY < uChannel.GetLength(1))
                        {
                            uChannel[uIndexX, uIndexY] = yuvImage[x, y].U;
                            vChannel[uIndexX, uIndexY] = yuvImage[x, y].V;
                        }
                    }
                }
            }

            return (yChannel, uChannel, vChannel);
        }

        public unsafe byte[] Compress(double[,] yChannel, double[,] uChannel, double[,] vChannel, int alphabetSize)
        {
            List<byte> data = new List<byte>();

            data.AddRange(ConvertChannelToByteArray(yChannel));
            data.AddRange(ConvertChannelToByteArray(uChannel));
            data.AddRange(ConvertChannelToByteArray(vChannel));

            ShannonFano shannonFano = new ShannonFano();
            var dictionary = shannonFano.CreateDictionary(data.ToArray(), alphabetSize);
            byte[] compressedData = shannonFano.Compress(data.ToArray(), dictionary);

            List<byte> finalData = new List<byte>();
            finalData.AddRange(BitConverter.GetBytes(dictionary.Count));
            foreach (var kvp in dictionary)
            {
                finalData.Add(kvp.Key);
                finalData.AddRange(BitConverter.GetBytes(kvp.Value.Count));
                foreach (var bit in kvp.Value)
                {
                    finalData.Add((byte)(bit ? 1 : 0));
                }
            }
            finalData.AddRange(compressedData);

            Console.WriteLine("Dictionary Count: " + dictionary.Count);
            Console.WriteLine("Compressed Data Length: " + compressedData.Length);
            Console.WriteLine("Final Data Length: " + finalData.Count);

            return finalData.ToArray();
        }
      
        public void SaveToPavFile(string filePath, byte[] compressedData, int width, int height)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            using (var writer = new BinaryWriter(fileStream))
            {
                writer.Write(width);
                writer.Write(height);
                writer.Write(compressedData.Length);
                writer.Write(compressedData);
            }
        }

        public Bitmap LoadFromPavFile(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
            using (var reader = new BinaryReader(fileStream))
            {
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();
                int dataLength = reader.ReadInt32();
                byte[] compressedData = reader.ReadBytes(dataLength);

                // Decompress data
                byte[] decompressedData = Decompress(compressedData);
                return ConvertByteArrayToBitmap(decompressedData, width, height);
            }
        }

        public unsafe byte[] Decompress(byte[] compressedData)
        {
            int dictionarySize = BitConverter.ToInt32(compressedData, 0);
            int offset = 4;

            Dictionary<byte, List<bool>> dictionary = new Dictionary<byte, List<bool>>(dictionarySize);

            bool[] codeBuffer = new bool[256];

            for (int i = 0; i < dictionarySize; i++)
            {
                byte symbol = compressedData[offset++];
                int codeLength = BitConverter.ToInt32(compressedData, offset);
                offset += 4;

                for (int j = 0; j < codeLength; j++)
                {
                    codeBuffer[j] = compressedData[offset++] == 1;
                }

                dictionary[symbol] = codeBuffer.Take(codeLength).ToList();
            }

            int compressedBytesLength = compressedData.Length - offset;
            byte[] compressedBytes = new byte[compressedBytesLength];
            fixed (byte* pCompressedData = compressedData)
            {
                byte* srcPtr = pCompressedData + offset;
                fixed (byte* pCompressedBytes = compressedBytes)
                {
                    byte* destPtr = pCompressedBytes;
                    for (int i = 0; i < compressedBytesLength; i++)
                    {
                        *destPtr++ = *srcPtr++;
                    }
                }
            }

            ShannonFano shannonFano = new ShannonFano();
            byte[] decompressedData = shannonFano.Decompress(compressedBytes, dictionary);

            Console.WriteLine("Dictionary Size: " + dictionarySize);
            Console.WriteLine("Compressed Bytes Length: " + compressedBytesLength);
            Console.WriteLine("Decompressed Data Length: " + decompressedData.Length);

            return decompressedData;
        }

        //dithering + filters
        public unsafe Bitmap ApplyHomogeneityEdgeDetection(Bitmap original)
        {
            int width = original.Width;
            int height = original.Height;
            Bitmap edgeDetected = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData originalData = original.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData edgeData = edgeDetected.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            byte* originalPtr = (byte*)originalData.Scan0;
            byte* edgePtr = (byte*)edgeData.Scan0;

            int stride = originalData.Stride;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 3;
                    byte b = originalPtr[index];
                    byte g = originalPtr[index + 1];
                    byte r = originalPtr[index + 2];

                    double intensity = 0.299 * r + 0.587 * g + 0.114 * b;
                    double maxDifference = 0;

                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            if (dx == 0 && dy == 0) continue;

                            int neighborX = x + dx;
                            int neighborY = y + dy;

                            if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                            {
                                int neighborIndex = neighborY * stride + neighborX * 3;
                                byte nb = originalPtr[neighborIndex];
                                byte ng = originalPtr[neighborIndex + 1];
                                byte nr = originalPtr[neighborIndex + 2];

                                double neighborIntensity = 0.299 * nr + 0.587 * ng + 0.114 * nb;
                                double difference = Math.Abs(intensity - neighborIntensity);
                                if (difference > maxDifference)
                                {
                                    maxDifference = difference;
                                }
                            }
                        }
                    }

                    byte edgeValue = (byte)Math.Clamp(maxDifference, 0, 255);
                    edgePtr[index] = edgeValue;
                    edgePtr[index + 1] = edgeValue;
                    edgePtr[index + 2] = edgeValue;
                }
            }

            original.UnlockBits(originalData);
            edgeDetected.UnlockBits(edgeData);

            return edgeDetected;
        }
        public unsafe Bitmap ApplyDithering(Bitmap original)
        {
            int width = original.Width;
            int height = original.Height;
            Bitmap dithered = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData originalData = original.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData ditheredData = dithered.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            byte* originalPtr = (byte*)originalData.Scan0;
            byte* ditheredPtr = (byte*)ditheredData.Scan0;

            int stride = originalData.Stride;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + x * 3;
                    byte b = originalPtr[index];
                    byte g = originalPtr[index + 1];
                    byte r = originalPtr[index + 2];

                    int oldPixel = (int)(0.299 * r + 0.587 * g + 0.114 * b);
                    int newPixel = oldPixel > 127 ? 255 : 0;
                    byte newPixelByte = (byte)newPixel;

                    ditheredPtr[index] = newPixelByte;
                    ditheredPtr[index + 1] = newPixelByte;
                    ditheredPtr[index + 2] = newPixelByte;

                    int quantError = oldPixel - newPixel;

                    for (int ky = 0; ky < _kernel.GetLength(0); ky++)
                    {
                        for (int kx = 0; kx < _kernel.GetLength(1); kx++)
                        {
                            int nx = x + kx - 2;
                            int ny = y + ky;

                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                int nIndex = ny * stride + nx * 3;
                                byte nb = originalPtr[nIndex];
                                byte ng = originalPtr[nIndex + 1];
                                byte nr = originalPtr[nIndex + 2];

                                int neighborPixel = (int)(0.299 * nr + 0.587 * ng + 0.114 * nb);
                                int newNeighborPixel = neighborPixel + (quantError * _kernel[ky, kx]) / _kernelSum;
                                newNeighborPixel = Math.Clamp(newNeighborPixel, 0, 255);

                                originalPtr[nIndex] = (byte)newNeighborPixel;
                                originalPtr[nIndex + 1] = (byte)newNeighborPixel;
                                originalPtr[nIndex + 2] = (byte)newNeighborPixel;
                            }
                        }
                    }
                }
            }

            original.UnlockBits(originalData);
            dithered.UnlockBits(ditheredData);

            return dithered;
        }
        public static bool ApplyGamma(Bitmap b, double red, double green, double blue)
        {
            if (red < .2 || red > 5) return false;
            if (green < .2 || green > 5) return false;
            if (blue < .2 || blue > 5) return false;

            byte[] redGamma = new byte[256];
            byte[] greenGamma = new byte[256];
            byte[] blueGamma = new byte[256];

            for (int i = 0; i < 256; ++i)
            {
                redGamma[i] = (byte)Math.Min(255, (int)((255.0 * Math.Pow(i / 255.0, 1.0 / red)) + 0.5));
                greenGamma[i] = (byte)Math.Min(255, (int)((255.0 * Math.Pow(i / 255.0, 1.0 / green)) + 0.5));
                blueGamma[i] = (byte)Math.Min(255, (int)((255.0 * Math.Pow(i / 255.0, 1.0 / blue)) + 0.5));
            }

            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width * 3;

                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        p[2] = redGamma[p[2]];
                        p[1] = greenGamma[p[1]];
                        p[0] = blueGamma[p[0]];

                        p += 3;
                    }
                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);

            return true;
        }


        //helper methods
        private byte[] ConvertChannelToByteArray(double[,] channel)
        {
            int width = channel.GetLength(0);
            int height = channel.GetLength(1);
            byte[] byteArray = new byte[width * height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byteArray[y * width + x] = (byte)Math.Clamp(channel[x, y], 0, 255);
                }
            }

            return byteArray;
        }
        private unsafe Bitmap ConvertByteArrayToBitmap(byte[] data, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            byte* p = (byte*)bitmapData.Scan0;

            int yLength = width * height;
            int uvWidth = width / 2;
            int uvHeight = height / 2;
            int uvLength = uvWidth * uvHeight;


            for (int y = 0; y < height; y++)
            {
                byte* row = p + (y * bitmapData.Stride);
                for (int x = 0; x < width; x++)
                {
                    int yIndex = y * width + x;
                    int uvIndex = (y / 2) * uvWidth + (x / 2);

                    byte yValue = data[yIndex];
                    byte uValue = data[yLength + uvIndex];
                    byte vValue = data[yLength + uvLength + uvIndex];

                    int r = (int)(yValue + 1.13983 * vValue);
                    int g = (int)(yValue - 0.39465 * uValue - 0.58060 * vValue);
                    int b = (int)(yValue + 2.03211 * uValue);

                    r = Math.Clamp(r, 0, 255);
                    g = Math.Clamp(g, 0, 255);
                    b = Math.Clamp(b, 0, 255);

                    row[x * 3] = (byte)b;
                    row[x * 3 + 1] = (byte)g;
                    row[x * 3 + 2] = (byte)r;
                }
            }

            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }
    }
}
