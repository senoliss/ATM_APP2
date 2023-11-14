namespace ATM_APP2
{
	internal class Program
	{
		static string cardsFolderPath = "Cards";
		static void Main(string[] args)
		{
			using var dbContext = new ATM_Context();

			while (true)
			{
				Directory.CreateDirectory(cardsFolderPath);

				Console.WriteLine("Welcome to the ATM");
				Console.WriteLine("Insert your card (GUID):");
				string cardGuid = Console.ReadLine();

				if (Guid.TryParse(cardGuid, out Guid parsedGuid))
				{
					string cardFilePath = Path.Combine(cardsFolderPath, $"{parsedGuid}.txt");
					var data = LoadData(parsedGuid);

					// Checking if the card exists in database
					var cardExists = dbContext.Accounts.Select(c => c.CardNumber.Equals(cardGuid));
					if (cardExists == null)
					{
						// Inserting account data after creation, need to consider if card exists or not
						var card = new Account(data.CardNumber, data.Pin, data.Balance);
						dbContext.Accounts.Add(card);
						dbContext.SaveChanges();
					}
					else
					{
                        Console.WriteLine("The card exists in database, pls write ur pin.");
                    }
                    Login(ref parsedGuid, data);
					DisplayMenu(parsedGuid);
				}
				else
				{
					Console.WriteLine("Invalid card.");
					Thread.Sleep(2000);
					Console.Clear();
				}
			}
		}

		private static Account LoadData(Guid card)
		{
			// dummy cards guids
			//a5fcf4be-0c2e-4235-aef1-1c58ede75c6e
			//28f97545-5750-4369-b466-79d2ff056548
			//98a7cb65-f8e1-472a-a684-e286bd9b40f3
			//35b04b6e-cafc-4a24-b58e-94a911df2568
			//c4ba04fe-3609-47ad-b093-7548f53124d0

			var fileName = GetFileName(card);

			if (File.Exists(fileName))
			{
				var lines = File.ReadAllLines(fileName);
				try
				{

					return new Account
					{
						CardNumber = Guid.Parse(lines[0]),//card,
						Balance = decimal.Parse(lines[1]),
						Pin = Int32.Parse(lines[2]),        // want to somehow implement security on pin
						History = lines.Skip(3).Select(line => new History { Amount = decimal.Parse(line.Split(',')[0]), Timestamp = DateTime.Parse(line.Split(',')[1]) }).ToList()
					};
				}
				catch (Exception e)
				{
					Console.WriteLine("Invalid card file cointaining data. Or file is not created yet.");
					Console.WriteLine(e.Message);
					return null;
				}

			}
			else
			{
				//Console.WriteLine("Card is first time inserted, please set up the pin: ");
				//int pin = Int32.Parse(Console.ReadLine());
				//return new Account { Balance = 1000, History = new List<History>(), Pin = pin, CardNumber = card };
				Console.WriteLine("Card does not exist. Do you want to create a new card? (yes/no)");
				string response = Console.ReadLine();
				if (response.ToLower() == "yes")
				{
					Console.WriteLine("Please set up your PIN:");
					int newPin = Convert.ToInt32(Console.ReadLine());
					decimal initialBalance = 550.0M;

					File.WriteAllText(fileName, $"{card}\n{initialBalance}\n{newPin}\n");
					Console.WriteLine("Card created successfully...");
					return new Account { Balance = initialBalance, History = new List<History>(), Pin = newPin, CardNumber = card };
				}
				else return null;
			}
		}

		private static void Login(ref Guid card, Account data)
		{
			var attempts = 0;
			int password = 0;
			while (attempts < 3)
			{
				Console.WriteLine("Please enter your password.");
				password = Int32.Parse(Console.ReadLine());
				if (data.Pin == password)
				{
					Console.WriteLine("Login succesful1");
					Thread.Sleep(2000);
					Console.Clear();
					break;
				}
				else
				{
					attempts++;
					Console.WriteLine($"Invalid password. Please try again. Attempts: {attempts}/3");
				}
			}
			if (attempts == 3)
			{
				Console.WriteLine("Too many incorrect attempts. Closing program.");
				Environment.Exit(0);
			}
		}


		private static void DisplayMenu(Guid card)
		{
			var data = LoadData(card);

			while (true)
			{
				Console.WriteLine("Please select an option:");
				Console.WriteLine("1. Check available money");
				Console.WriteLine("2. View 5 past transactions");
				Console.WriteLine("3. Deposit money");
				Console.WriteLine("4. Withdraw money");
				Console.WriteLine("5. Exit");
				var option = Console.ReadLine();
				switch (option)
				{
					case "1":
						Console.Clear();
						Console.WriteLine($"Available money: {data.Balance}");
						Console.Write("Press any key to go back to menu: ");
						Console.ReadKey();
						Console.Clear();
						break;
					case "2":
						Console.Clear();
						Console.WriteLine("Past transactions:");
						Console.WriteLine("=========================");
						Range transactionRange = new Range();
						if (data.History.Count > 5)
						{
							transactionRange = new Range(data.History.Count - 5, data.History.Count);
						}
						else { transactionRange = new Range(0, 5); }

						foreach (var transaction in data.History.Take(transactionRange))
						{
							string baba = transaction.Amount > 0 ? "Deposited:" : "Withdrew:";
							Console.WriteLine($"{baba} {transaction.Amount} - {transaction.Timestamp}");
						}
						Console.WriteLine("=========================");
						Console.Write("Press any key to go back to menu: ");
						Console.ReadKey();
						Console.Clear();
						break;
					case "3":
						Console.Clear();
						DepositMoney(card, ref data);
						Console.WriteLine("=========================");
						Console.Write("Press any key to go back to menu: ");
						Console.ReadKey();
						Console.Clear();
						break;
					case "4":
						Console.Clear();
						WithdrawMoney(card, ref data);
						Console.WriteLine("=========================");
						Console.Write("Press any key to go back to menu: ");
						Console.ReadKey();
						Console.Clear();
						break;
					case "5":
						Console.WriteLine("Exiting...");
						Thread.Sleep(2000);
						Console.Clear();
						return;
					default:
						Console.WriteLine("Invalid option. Please try again.");
						break;
				}
			}
		}

		static void DepositMoney(Guid card, ref Account data)
		{
			Console.WriteLine("Enter the amount to deposit:");
			decimal amount = decimal.Parse(Console.ReadLine());

			if (amount <= 0 || amount > 1000000)
			{
				Console.WriteLine("Invalid deposit amount.");
			}
			else
			{
				data.Balance += amount;
				data.History.Add(new History { Amount = amount, Timestamp = DateTime.Now });
				Console.WriteLine($"Deposited {amount} euros. New balance: {data.Balance} euros");
			}

			SaveData(card, data);
		}

		static void WithdrawMoney(Guid card, ref Account data)
		{
			Console.WriteLine($"Enter the amount to withdraw (max {data.Balance} euros):");
			decimal amount = decimal.Parse(Console.ReadLine());

			if (amount <= 0 || amount > data.Balance)
			{
				Console.WriteLine("Invalid withdrawal amount.");
			}
			else if (data.Balance >= amount)
			{
				data.Balance -= amount;
				data.History.Add(new History { Amount = -amount, Timestamp = DateTime.Now }); // $"Withdrawal: -{amount} euros");
				SaveData(card, data);
				Console.WriteLine($"Withdrawn {amount} euros. New balance: {data.Balance} euros");
			}
			else
			{
				Console.WriteLine("Insufficient funds.");
			}
		}

		private static string GetFileName(Guid card)
		{
			return Path.Combine(cardsFolderPath, $"{card}.txt");
		}

		private static void SaveData(Guid card, Account data)
		{
			var fileName = GetFileName(card);
			//if (File.Exists(fileName))
			var lines = new List<string> { data.CardNumber.ToString(), data.Balance.ToString(), data.Pin.ToString() };

			lines.AddRange(data.History.Select(transaction => $"{transaction.Amount},{transaction.Timestamp}"));

			File.WriteAllLines(fileName, lines);
		}

	}
}