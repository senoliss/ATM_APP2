using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM_APP2
{
	public class Account
	{
        public Guid Id { get; set; }
        public Guid CardNumber { get; set; }
		public int Pin { get; set; }
		public decimal Balance { get; set; }
		public List<History> History { get; set; }

		public Account(Guid cardNumber, int pin, decimal balance) 
		{
			Id = Guid.NewGuid();
			CardNumber = cardNumber;
			Pin = pin;
			Balance = balance;
		}
		public Account() { }
	}
}
