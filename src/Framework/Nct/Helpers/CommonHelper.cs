using System;

namespace Nct.Helpers
{
    public static class CommonHelper
    {

      
        public static string GetRandom(int len, bool number = false)
        {
            string randomChar = "1234567890ABCDEFGHJKLMNPQRSTUVWXYZ";
            if (number)
            {
                randomChar = "1234567890";

            }

            char[] allCharArray = randomChar.ToCharArray();
            int n = allCharArray.Length;
            string randomCode = "";
            System.Random random = new Random(~unchecked((int)DateTime.Now.Ticks));
            for (int i = 0; i < len; i++)
            {
                int rnd = random.Next(0, n);
                randomCode += allCharArray[rnd];
            }
            return randomCode.ToLower();
        }


        public static string BytesToBase64Img(byte[] bytes)
        {

            string base64 = Convert.ToBase64String(bytes);
            return $"data:image/png;base64,{base64}";

        }






    }
}
