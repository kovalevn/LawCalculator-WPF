using System;
using System.Collections.Generic;
using System.Text;

namespace LawCalculator_WPF
{
    public enum CurrencyType { Dollar, Rouble, Euro, DollarCashless, RoubleCashless, EuroCashless }
    class Payment
    {
        public double Amount { get; set; }
        public DateTime Date { get => date; set { DateString = value.ToShortDateString(); date = value; } }
        private DateTime date;
        public string DateString { get; private set; }
        public CurrencyType Currency { get; set; }
        public string ProjectName { get; set; }
        public bool ToPay { get; set; } = false;
        public bool Payed { get; set; } = false;
        //public int quartal;
    }
}
