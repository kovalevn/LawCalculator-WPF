using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace LawCalculator_WPF
{
    class Partner
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public int Salary { get; set; } = 0;
        public ObservableCollection<LawyersProject> LawyersProjects { get; set; } = new ObservableCollection<LawyersProject>();
        public List<Lawyer> Lawyers = new List<Lawyer>();

        public Partner(string name)
        {
            this.Name = name;
        }

        public Partner()
        {
        }

        public double CountBalance(DateTime date)
        {
            List<Payment> thisMonthPayments = new List<Payment>();
            foreach (LawyersProject lp in LawyersProjects)
            {
                var payments = lp.Payments.Where(p => p.Date.Month == date.Month && p.Date.Year == date.Year).ToList();
                thisMonthPayments.AddRange(payments);
            }
            double sum = 0;
            foreach (Payment payment in thisMonthPayments)
            {
                switch (payment.Currency)
                {
                    case CurrencyType.Dollar:
                    case CurrencyType.DollarCashless:
                        sum += CurrencyConverter.ConvertToRouble(payment.Amount, CurrencyType.Dollar);
                        break;
                    case CurrencyType.Euro:
                    case CurrencyType.EuroCashless:
                        sum += CurrencyConverter.ConvertToRouble(payment.Amount, CurrencyType.Euro);
                        break;
                    default:
                        sum += payment.Amount;
                        break;
                }
            }
            return sum;
        }

        public double CountBalance(DateTime date, CurrencyType currency)
        {
            List<Payment> thisMonthPayments = new List<Payment>();
            foreach (LawyersProject lp in LawyersProjects)
            {
                var payments = lp.Payments.Where(p => p.Date.Month == date.Month && p.Date.Year == date.Year && p.Currency == currency).ToList();
                thisMonthPayments.AddRange(payments);
            }
            double sum = 0;
            foreach (Payment payment in thisMonthPayments) sum += payment.Amount;
            return sum;
        }

        public double CountYearBalance(DateTime date)
        {
            List<Payment> thisYearPayments = new List<Payment>();
            foreach (LawyersProject lp in LawyersProjects)
            {
                var payments = lp.Payments.Where(p => p.Date.Month <= date.Month && p.Date.Year == date.Year).ToList();
                thisYearPayments.AddRange(payments);
            }
            double sum = 0;
            foreach (Payment payment in thisYearPayments)
            {
                switch (payment.Currency)
                {
                    case CurrencyType.Dollar:
                    case CurrencyType.DollarCashless:
                        sum += CurrencyConverter.ConvertToRouble(payment.Amount, CurrencyType.Dollar);
                        break;
                    case CurrencyType.Euro:
                    case CurrencyType.EuroCashless:
                        sum += CurrencyConverter.ConvertToRouble(payment.Amount, CurrencyType.Euro);
                        break;
                    default:
                        sum += payment.Amount;
                        break;
                }
            }
            return sum;
        }
    }
}
