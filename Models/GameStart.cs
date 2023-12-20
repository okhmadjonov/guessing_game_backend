namespace guessing_game_backend.Models
{
    public class GameStart
    {

        public int[] SecretNumber { get; set; }
        public int Attempts { get; set; }
        public List<string> Description { get; set;}


        public GameStart()
        {
            SecretNumber = GenerateSecretNumber();
            Attempts = 8;
            Description = new List<string>();
        }

        private int[] GenerateSecretNumber()
        {
            Random random = new Random();
            int[] digits = new int[4] { 7,0,4,6};

            //for (int i = 0; i < 4; i++)
            //{
            //    while (true)
            //    {
            //        int digit = random.Next(0, 10);

            //        if (!digits.Contains(digit))
            //        {
            //            digits[i] = digit;
            //            break;
            //        }
            //    }
            //}
            return digits;
        }

    }
}
