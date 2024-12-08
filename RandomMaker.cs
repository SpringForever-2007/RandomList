using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomList
{
    public class RandomMaker
    {
        private Random random;

        public RandomMaker()
        {
            random = new Random();
        }

        public void Exchange(ref int max, ref int min)
        {
            if (max<min)
            {
                int tmp = max;
                max = min;
                min = tmp;
            }
        }

        public int[] MakeUnique(int length, int min, int max)
        {
            Exchange(ref max, ref min);
            if (length > (max - min))
            {
                return null;
            }

            HashSet<int> uniqueNumbers = new HashSet<int>();
            int[] randomNumbers = new int[length];

            while (uniqueNumbers.Count < length)
            {
                int randomNumber = random.Next(min, max);
                uniqueNumbers.Add(randomNumber);
            }

            uniqueNumbers.CopyTo(randomNumbers);
            return randomNumbers;
        }
    }
}
