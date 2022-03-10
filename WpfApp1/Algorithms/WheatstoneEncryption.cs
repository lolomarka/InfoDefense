using System;
using System.Collections.Generic;
using System.Windows;


namespace InfoDefence.Algorithms
{
    public class WheatstoneEncryption
    {
        public char[,] LeftTable
        {
            get; private set;
        }
        public char[,] RightTable
        {
            get; private set;
        }

        private Random Random = new Random();

        public WheatstoneEncryption()
        {
            LeftTable = new char[8, 9];
            RightTable = new char[8, 9];

            List<char> LeftList = new List<char>();
            List<char> RightList = new List<char>();
            for (int i = 0; i < 64; i++)
            {
                LeftList.Add((char)(1040 + i));
                RightList.Add((char)(1040 + i));
            }
            LeftList.Add('.');
            RightList.Add('.');
            LeftList.Add(',');
            RightList.Add(',');
            LeftList.Add('?');
            RightList.Add('?');
            LeftList.Add('!');
            RightList.Add('!');
            LeftList.Add(' ');
            RightList.Add(' ');

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (LeftList.Count == 0)
                        continue;
                    var lIndex = Random.Next(LeftList.Count - 1);
                    LeftTable[i, j] = LeftList[lIndex];
                    LeftList.RemoveAt(lIndex);

                    var rIndex = Random.Next(RightList.Count - 1);
                    RightTable[i, j] = RightList[rIndex];
                    RightList.RemoveAt(rIndex);

                }
            }
        }

        private (Point, Point) GetCharCoordinate(char c1, char c2)
        {
            Point fPoint;
            Point sPoint;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (LeftTable[i, j] == c1)
                    {
                        fPoint = new Point(i, j);
                    }
                }
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (RightTable[i, j] == c2)
                    {
                        sPoint = new Point(i, j);
                    }
                }
            }

            return (fPoint, sPoint);
        }

        private (Point, Point) GetCharCoordinateForDecrypt(char c1, char c2)
        {
            Point fPoint;
            Point sPoint;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (RightTable[i, j] == c1)
                    {
                        fPoint = new Point(i, j);
                    }
                }
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (LeftTable[i, j] == c2)
                    {
                        sPoint = new Point(i, j);
                    }
                }
            }

            return (fPoint, sPoint);
        }

        public string Encrypt(string text)
        {
            try
            {
                var outputRes = "";

                for (int i = 1; i < text.Length; i += 2)
                {
                    char fCh = text[i - 1];
                    char sCh = text[i];

                    var coords = GetCharCoordinate(fCh, sCh);

                    outputRes += RightTable[(int)coords.Item2.X, (int)coords.Item1.Y];
                    outputRes += LeftTable[(int)coords.Item1.X, (int)coords.Item2.Y];
                }

                return outputRes;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public string Decrypt(string text)
        {
            try
            {
                var outputRes = "";

                for (int i = 1; i < text.Length; i += 2)
                {
                    char fCh = text[i - 1];
                    char sCh = text[i];

                    var coords = GetCharCoordinateForDecrypt(fCh, sCh);

                    outputRes += LeftTable[(int)coords.Item2.X, (int)coords.Item1.Y];
                    outputRes += RightTable[(int)coords.Item1.X, (int)coords.Item2.Y];
                }

                return outputRes;
            }
            catch (Exception)
            {

                return null;
            }
        }
    }
}
