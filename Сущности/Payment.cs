using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LawCalculator_WPF
{
    public enum CurrencyType { Dollar, Rouble, Euro, DollarCashless, RoubleCashless, EuroCashless }

    class Payment : IEquatable<Payment>, IHaveId
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }

        public string DateString
        { 
            get 
            {
                return Date.ToShortDateString();
            }
        }
        public CurrencyType Currency { get; set; }
        public string ProjectName { get; set; }
        public bool ToPay 
        {
            get
            {
                return DateTime.Compare(Date, DateTime.Today.AddMonths(-3)) <= 0 && !Payed;
            }
        }
        public bool Payed { get; set; } = false;

        public bool Equals([AllowNull] Payment other)
        {
            return null != other && Amount == other.Amount && Date == other.Date && Currency == other.Currency;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Payment);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
