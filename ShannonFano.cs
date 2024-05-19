using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PavImgConverter
{
    public class ShannonFanoNode
    {
        public byte Symbol { get; set; }
        public double Probability { get; set; }
        public List<bool> Code { get; set; } = new List<bool>();
    }
    public class ShannonFano
    {
        public Dictionary<byte, List<bool>> CreateDictionary(byte[] data, int alphabetSize)
        {
            var frequencies = data.GroupBy(b => b)
                                  .Select(g => new ShannonFanoNode
                                  {
                                      Symbol = g.Key,
                                      Probability = (double)g.Count() / data.Length
                                  })
                                  .OrderByDescending(n => n.Probability)
                                  .Take(alphabetSize)
                                  .ToList();

           
            double totalProbability = frequencies.Sum(n => n.Probability);
            foreach (var node in frequencies)
            {
                node.Probability /= totalProbability;
            }

            GenerateCodes(frequencies, new List<bool>(), alphabetSize);
            return frequencies.ToDictionary(node => node.Symbol, node => node.Code);
        }

        private void GenerateCodes(List<ShannonFanoNode> nodes, List<bool> prefix, int alphabetSize)
        {
            if (nodes.Count == 1)
            {
                nodes[0].Code.AddRange(prefix);
                return;
            }

            double totalProbability = nodes.Sum(n => n.Probability);
            double halfProbability = totalProbability / 2;

            double currentSum = 0;
            int splitIndex = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                currentSum += nodes[i].Probability;
                if (currentSum >= halfProbability)
                {
                    splitIndex = i;
                    break;
                }
            }

            GenerateCodes(nodes.Take(splitIndex + 1).ToList(), new List<bool>(prefix) { false }, alphabetSize);
            GenerateCodes(nodes.Skip(splitIndex + 1).ToList(), new List<bool>(prefix) { true }, alphabetSize);
        }

        public unsafe byte[] Compress(byte[] data, Dictionary<byte, List<bool>> dictionary)
        {
            List<bool> compressedBits = new List<bool>();

            foreach (byte symbol in data)
            {
                compressedBits.AddRange(dictionary[symbol]);
            }

            int bitCount = compressedBits.Count;
            int byteCount = (bitCount + 7) / 8;
            byte[] compressedData = new byte[byteCount];

            fixed (byte* pCompressedData = compressedData)
            {
                byte* ptr = pCompressedData;
                byte currentByte = 0;
                int bitIndex = 0;

                for (int i = 0; i < bitCount; i++)
                {
                    if (compressedBits[i])
                    {
                        currentByte |= (byte)(1 << (7 - (bitIndex % 8)));
                    }

                    if (++bitIndex % 8 == 0)
                    {
                        *ptr++ = currentByte;
                        currentByte = 0;
                    }
                }

                if (bitIndex % 8 != 0)
                {
                    *ptr = currentByte;
                }

            }
            

            return compressedData;
        }

        public unsafe byte[] Decompress(byte[] compressedData, Dictionary<byte, List<bool>> dictionary)
        {
            var reverseDictionary = dictionary.ToDictionary(kvp => string.Join("", kvp.Value.Select(b => b ? "1" : "0")), kvp => kvp.Key);

            List<byte> decompressedData = new List<byte>();
            List<bool> buffer = new List<bool>();

            fixed (byte* pCompressedData = compressedData)
            {
                byte* ptr = pCompressedData;

                for (int i = 0; i < compressedData.Length * 8; i++)
                {
                    bool bit = ((*ptr) & (1 << (7 - (i % 8)))) != 0;
                    buffer.Add(bit);

                    if ((i + 1) % 8 == 0)
                    {
                        ptr++;
                    }

                    string bufferKey = string.Join("", buffer.Select(b => b ? "1" : "0"));

                    if (reverseDictionary.TryGetValue(bufferKey, out byte symbol))
                    {
                        decompressedData.Add(symbol);
                        buffer.Clear();
                    }
                }
            }

            return decompressedData.ToArray();
        }
    }
}

