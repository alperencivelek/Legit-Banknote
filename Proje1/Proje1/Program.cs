using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace Proje1
{
    class Program
    {
        //Matrisi ekrana basmak için;
        static int Show_me(double[,] m)
        {
            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    Console.Write(m[i, j] + " | ");
                }
                Console.WriteLine("\n");
            }
            return 0;
        }
        //Verilen değerlere göre rastgele noktalar üretmek için;
        static double[,] dot_producer(int dot_number,int width, int height)
        {
            Random random_double = new Random();
            double[,] dot_matrix = new double[dot_number, 2];
            for (int i=0; i<dot_number; i++)
            {
                dot_matrix[i, 0] = Convert.ToDouble((random_double.NextDouble()).ToString("0.0000"))*width;
                dot_matrix[i, 1] = Convert.ToDouble((random_double.NextDouble()).ToString("0.0000"))*height;
            }
            return dot_matrix;
        }
        //Euklidean Uzaklığı hesaplamak için;
        static double Distance(double x1, double y1, double x2, double y2)
        {
            return Convert.ToDouble(Math.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)).ToString("0.00"));
        }
        //Noktaların birbirine olan uzaklığına matris olarak döndürmek için;
        static double[,] distance_matrix(double[,] m)
        {
            double[,] distance_matrix= new double[m.GetLength(0),m.GetLength(0)];
            for (int i=0; i<m.GetLength(0); i++)
            {
                for(int j=0; j<m.GetLength(0); j++)
                {
                    if (i == j)
                    {
                        distance_matrix[i, j] = 0.00;
                    }
                    else
                    {
                        distance_matrix[i, j] = Distance(m[i, 0], m[i, 1], m[j, 0], m[j, 1]);
                    }
                }
            }
            return distance_matrix;
        }
        public class Banknote
        {
            public double variance;
            public double skew;
            public double flatness;
            public double entropy;
            public int validity;
            public Banknote(double variance, double skew, double flatness, double entropy, int validity)
            {
                this.variance = variance;
                this.skew = skew;
                this.flatness = flatness;
                this.entropy = entropy;
                this.validity = validity;
            }
            public Banknote(double variance, double skew, double flatness, double entropy)
            {
                this.variance = variance;
                this.skew = skew;
                this.flatness = flatness;
                this.entropy = entropy;
            }
            public string ToString()
            {
                return ("Variance: " + variance + " " + "skew: " + skew + " " + "flatness: " + flatness + " " + "Entropy: " + entropy + " " + "Validity: " + validity);
            }
        }
        public class Tupple
        {
            public Banknote banknote;
            public double distance;
            public Tupple(Banknote banknote, double distance)
            {
                this.banknote = banknote;
                this.distance = distance;
            }
        }

        static double Distance(Banknote pointOne, Banknote pointTwo)
        {
            double dVariance = pointOne.variance - pointTwo.variance;
            double dSkew = pointOne.skew - pointTwo.skew;
            double dFlatness = pointOne.flatness - pointTwo.flatness;
            double dEntropy = pointOne.entropy - pointTwo.entropy;

            return Math.Sqrt((dVariance * dVariance) + (dSkew * dSkew) + (dFlatness * dFlatness) + (dEntropy * dEntropy));
        }

        static Banknote[] Read(string text)
        {
            string[] texts = text.Split('\n');
            Banknote[] banknotes = new Banknote[texts.Length];
            int counter = 0;
            foreach (string line in texts)
            {
                string[] datas = line.Split(';');
                Banknote banknote = new Banknote(Convert.ToDouble(datas[0]), Convert.ToDouble(datas[1]), Convert.ToDouble(datas[2]), Convert.ToDouble(datas[3]), Convert.ToInt32(datas[4]));
                banknotes[counter] = banknote;
                counter++;
            }
            return banknotes;
        }

        static Banknote Input()
        {
            Console.Write("Variance: ");
            double variance = Convert.ToDouble(Console.ReadLine());
            Console.Write("Skew: ");
            double skew = Convert.ToDouble(Console.ReadLine());
            Console.Write("Flatness: ");
            double flatness = Convert.ToDouble(Console.ReadLine());
            Console.Write("Entropy: ");
            double entropy = Convert.ToDouble(Console.ReadLine());
            Console.Write("Closest Neighbor Count to Decide(K Number): ");
            int banknotes = Convert.ToInt32(Console.ReadLine());

            return new Banknote(variance, skew, flatness, entropy, banknotes);
        }

        static bool Classify(Banknote origin, Banknote[] banknotes, int count, bool print = true)
        {
            List<Tupple> ordered = new List<Tupple>();

            foreach (Banknote banknote in banknotes)
            {
                ordered.Add(new Tupple(banknote, Distance(origin, banknote)));
            }
            ordered = ordered.OrderBy(o => o.distance).ToList();

            if (print)
            {
                Console.WriteLine("Banknote's Closest Neighbors:");
                for (int i = 0; i < count; i++)
                {
                    Console.WriteLine(ordered[i].banknote.ToString());
                }
                Console.WriteLine();
            }

            int balance = 0;
            for (int i = 0; i < count; i++)
            {
                if (ordered[i].banknote.validity == 0) balance++;
                else balance--;
            }

            if (balance > 0)
            {
                if (print) Console.WriteLine("Banknote is Valid!");
                return true;
            }
            else if (balance == 0)
            {
                if (ordered[0].banknote.validity == 0)
                {
                    if (print) Console.WriteLine("Banknote is Valid! (For nearest banknote.)");
                    return true;
                }
                else
                {
                    if (print) Console.WriteLine("Banknotet is Invalid! (For nearest banknote.)");
                    return false;
                }
            }
            else
            {
                if (print) Console.WriteLine("Banknote is Invalid!");
                return false;
            }
        }

        static double SuccessRate(Banknote origin, Banknote[] banknotes)
        {
            List<Banknote> minority = new List<Banknote>();
            List<Banknote> majority = new List<Banknote>();
            Banknote[] majorityArray = new Banknote[1172];

            int count = 0;
            for (int i = 0; i < banknotes.Length; i++)
            {
                if (i < 662)
                {
                    majority.Add(banknotes[i]);
                    majorityArray[count] = banknotes[i];
                    count++;
                }
                else if (i < 762) minority.Add(banknotes[i]);
                else if (i < 1272)
                {
                    majority.Add(banknotes[i]);
                    majorityArray[count] = banknotes[i];
                    count++;
                }
                else minority.Add(banknotes[i]);
               
            }
            Console.WriteLine("Test Datas was:");
            for(int i=0; i<200; i++)
            {
                Console.WriteLine(minority[i].ToString());
            }

            double success = 0;
            int counter = 0;
            foreach (Banknote testItem in minority)
            {
                bool validity = false;
                if (testItem.validity == 0) validity = true;

                if (validity == Classify(testItem, majorityArray, origin.validity, false))
                {
                    success++;

                }
            }

            return success / Convert.ToDouble(minority.Count()) * 100.0;
        }

        static void Print(Banknote[] banknotes)
        {
            foreach (Banknote banknote in banknotes)
            {
                Console.WriteLine(banknote.ToString());
            }
            Console.WriteLine();
        }
        static void Main(string[] args)
        {
            double[,] coordinate_matrix = dot_producer(10, 100, 100);
            Show_me(coordinate_matrix);
            double[,] dm_matrix = distance_matrix(coordinate_matrix);
            Show_me(dm_matrix);
            Banknote[] banknotes = Read(System.IO.File.ReadAllText(@"data_banknote_authentication.txt"));

            Banknote origin = Input();

            Classify(origin, banknotes, origin.validity, true);

            Console.WriteLine("200 Test Datas' Success Rate is: " + SuccessRate(origin, banknotes));

            Console.ReadKey();
        }   
    }
}
