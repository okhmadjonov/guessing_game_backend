// GameLogic.cs
namespace guessing_game_backend.Helpers
{
    public class GameLogic
    {
        public static bool TryParseInput(string input, out int[] digits)
        {
            if (input.Length == 4 && input.All(char.IsDigit))
            {
                digits = input.Select(ch => ch - '0').ToArray();
                return true;
            }
            else
            {
                digits = null;
                return false;
            }
        }

        public static int CalculateM(int[] secretNumber, int[] guess)
        {
            int count = 0;
            for (int i = 0; i < secretNumber.Length; i++)
            {
                for (int j = 0; j < guess.Length; j++)
                {
                    if (secretNumber[i] == guess[j])
                    {
                        count++;
                        break; 
                    }
                }
            }
            return count;
        }

        public static int CalculateP(int[] secretNumber, int[] guess)
        {
            return secretNumber.Where((digit, index) => digit == guess[index]).Count();
        }
    }
}
