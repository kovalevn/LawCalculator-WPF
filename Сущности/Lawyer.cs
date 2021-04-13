using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LawCalculator_WPF
{
    class Lawyer : IHaveId
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Salary { get; set; }
        public bool IsCounselor;
        public ICollection<Project> Project { get; set; }

        //Добавить коллекцию <Project, процент> и <Project, Payments> - так не нужен будет отдельный класс LawyerProject

        public ObservableCollection<LawyersProject> LawyersProjects { get; set; } = new ObservableCollection<LawyersProject>();

        public Lawyer(string name, int salary)
        {
            this.Name = name;
            this.Salary = salary;
        }

        public Lawyer() { }

        //Считаем баланс юриста: складываем соответствующие по датам платежи из его проектов, и вычетаем зарплату
        public double CountBalance(DateTime date)
        {
            List<Payment> thisMonthPayments = new List<Payment>();
            foreach (LawyersProject lp in LawyersProjects)
            {
                var payments = lp.Payments.Where(p => p.Date.Month == date.Month && p.Date.Year == date.Year).ToList();
                thisMonthPayments.AddRange(payments);
            }
            double sum = -Salary;
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

        public double CountYearBalance(DateTime date)
        {
            List<Payment> thisYearPayments = new List<Payment>();
            foreach (LawyersProject lp in LawyersProjects)
            {
                var payments = lp.Payments.Where(p => p.Date.Month <= date.Month && p.Date.Year == date.Year).ToList();
                thisYearPayments.AddRange(payments);
            }
            double sum = (-Salary * date.Month);
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
