using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATM_APP2
{
	public class History
	{
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
		public DateTime Timestamp { get; set; }
        
        [ForeignKey("Account")] // navigation property to be able to pin pages to books by book id
        public Guid CardId { get; set; }
        public Account Account { get; set; }
    }
}
