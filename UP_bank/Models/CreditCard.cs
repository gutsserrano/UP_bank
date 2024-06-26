using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Models
{
    public class CreditCard
    {
        public long Number { get; set; }
        public DateTime ExpirationDate { get; set; }
        public double Limit { get; set; }
        public string CVV { get; set; }
        public string Name { get; set; }
        public string Flag { get; set; }
        public bool Active { get; set; } 

        public CreditCard()
        {
            
        }
        public CreditCard(string name, string profile)
        {
            do
            {
                this.Number = CreateRandomNumber();
                this.Flag = ApllyFlag(this.Number);

            } while (this.Flag == null);
            this.CVV = CreateRandomCVV();
            this.Name = FormatName(name);
            this.ExpirationDate = CreateRandomExpirationDate();
            this.Limit = ApllyLimit(profile);
            this.Active = false;
        }
        

        private long CreateRandomNumber()
        {
            Random random = new Random();
            long number = random.NextInt64(1000000000000000, 9999999999999999);
            while (!VerifyRandomNumber(number))
            {
                number = random.NextInt64(1000000000000000, 9999999999999999);
            }
            return number;
        }
        private bool VerifyRandomNumber(long number)
        {
            string numberString = number.ToString();
            char[] chars = numberString.ToCharArray();
            Array.Reverse(chars);
            string reversed = new string(chars);

            List<int> sums = new List<int>();
            foreach(var item in reversed)
            {
                if (reversed.IndexOf(item) % 2 != 0)
                {
                    var sum = int.Parse(item.ToString()) * 2;
                    if (sum > 9)
                    {
                        sum = sum - 9;
                    }
                    sums.Add(sum);
                }
                else
                {
                    sums.Add(int.Parse(item.ToString()));
                }
            }
            int total = sums.Sum();
            if (total % 10 != 0)
            {
                return false;
            }
            else
            {
                return true;
            }            
        }

        private string CreateRandomCVV()
        {
            Random random = new Random();
            string cvv = random.Next(1, 999).ToString();
            if(cvv.Length < 3)
            {
                var fill = 3 - cvv.Length;

                cvv = cvv.PadLeft(fill, '0');
            }
            return cvv;
        }

        private string ApllyFlag(long number)
        {
            string numberString = number.ToString();

            string[] flags = new string[] { "Visa", "MasterCard", "American Express", "Elo", "Hipercard"};
            
            Regex visa = new Regex(@"^4\d{15}$");
            Regex masterCard = new Regex(@"^5[1-5]\d{14}$");
            Regex american = new Regex(@"^3[47]\d{13}$");
            Regex elo = new Regex(@"^636[2-3]\d{13}$|^5067\d{12}$|^4576\d{12,15}$");
            Regex hipercard = new Regex(@"^(3841[0-9]|60\d{2})\d{12}(?:\d{3})?$");

            if (visa.IsMatch(numberString))
            {
                return "Visa";
            }
            else if (masterCard.IsMatch(numberString))
            {
                return "MasterCard";
            }
            else if (american.IsMatch(numberString))
            {
                return "American Express";
            }
            else if (elo.IsMatch(numberString))
            {
                return "Elo";
            }
            else if (hipercard.IsMatch(numberString))
            {
                return "Hipercard";
            }
            else
            {
                return null;
            }
        }

        private string FormatName(string name)
        {
            string[] parts = name.Split(' ');

            if (parts.Length <= 2)
            {
                return name;
            }
            StringBuilder nameFormated = new StringBuilder();

            nameFormated.Append(parts[0]);
            for (int i = 1; i < (parts.Length - 1); i++)
            {
                nameFormated.Append(" " + parts[i][0] + ".");
            }

            nameFormated.Append(" " + parts[parts.Length - 1]);
            return nameFormated.ToString().ToUpper();            
        }

        private DateTime CreateRandomExpirationDate()
        {           
            int month = DateTime.Now.Month;
            int year = DateTime.Now.AddYears(5).Year;
            return new DateTime(year, month,1);
        }

        private double ApllyLimit(string profile)
        {
            switch (profile)
            {
                case "University":
                    return 2400;
                case "Normal":
                    return 5000.5;
                case "Vip":
                    return 20000;
                default:
                    return 0;
            }
        }
    }
}
