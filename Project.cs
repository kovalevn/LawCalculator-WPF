using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Windows;

namespace LawCalculator_WPF
{
    class Project : IComparable, INotifyPropertyChanged, IEquatable<Project>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CurrencyType ProjectCurrency { get; set; }

        public Partner OriginatingPartner { get; set; }
        public int OriginatingPartnerPercent { get; set; } = 20;
        public Partner ManagingPartner { get; set; }
        public int ManagingPartnerPercent { get; set; }
        public bool isSuccess;
        public ObservableCollection<Lawyer> Lawyers { get; set; } = new ObservableCollection<Lawyer>();
        public ObservableCollection<Payment> Payments { get; set; } = new ObservableCollection<Payment>();
        public ObservableCollection<Payment> PayedPayments { get; set; } = new ObservableCollection<Payment>();
        //private Payment firstPayment;

        private Visibility originatorVisibilityTrigger;
        public Visibility OriginatorVisibilityTrigger
        {
            get
            {
                return originatorVisibilityTrigger;
            }
            set
            {
                originatorVisibilityTrigger = value;
                OnPropertyChanged(nameof(OriginatorVisibilityTrigger));
            }
        }

        private Visibility managerVisibilityTrigger;
        public Visibility ManagerVisibilityTrigger
        {
            get
            {
                return managerVisibilityTrigger;
            }
            set
            {
                managerVisibilityTrigger = value;
                OnPropertyChanged(nameof(ManagerVisibilityTrigger));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //Как будем проверять, что надо выплатить в этом квартале: проверяем все платежи, если самый ранний платёж поступил за последние 
        //три месяца - переносим выплату на следующий, если от трёх до шести - выплачиваем все поступившие суммы, если платёж поступил более 
        //чем через 6 месяцв с даты первого платежа - записываем в текущий квартал и выплачиваем

        //По стрелке будет меню - партнёры, юристы, платежи. При нажатии на платежи можно будет увидеть весь список платежей с датой/суммой,
        //или осуществить поиск по дате

        public Project(string name, Partner originator, Partner manager, bool sucsess)
        {
            this.Name = name;
            OriginatingPartner = originator;
            ManagingPartner = manager;
            isSuccess = sucsess;
            ManagingPartnerPercent = isSuccess ? 40 : 25;

            OriginatorVisibilityTrigger = Visibility.Collapsed;
            ManagerVisibilityTrigger = Visibility.Collapsed;
        }

        public Project() { }

        public Project(string name, bool sucsess) 
        {
            this.Name = name;
            isSuccess = sucsess;
            ManagingPartnerPercent = isSuccess ? 40 : 25;

            OriginatorVisibilityTrigger = Visibility.Collapsed;
            ManagerVisibilityTrigger = Visibility.Collapsed;
        }

        #region Старые методы подсчёта денег

        public void CountMoneyOld()
        {
        //    MoneyDollar = 0;
        //    MoneyRouble = 0;
        //    MoneyEuro = 0;
        //    MoneyDollarCashless = 0;
        //    MoneyRoubleCashless = 0;
        //    MoneyEuroCashless = 0;
            bool setCurrency = true;
            foreach (Payment pay in Payments)
            {
                if (pay.ToPay)
                {
                    if (setCurrency)
                    {
                        ProjectCurrency = pay.Currency;
                        setCurrency = false;
                    }
                    //pay.ToPay = true;
                    //            if (pay.Currency == CurrencyType.Dollar) MoneyDollar += pay.Amount;
                    //            if (pay.Currency == CurrencyType.Rouble) MoneyRouble += pay.Amount;
                    //            if (pay.Currency == CurrencyType.Euro) MoneyEuro += pay.Amount;
                    //            if (pay.Currency == CurrencyType.DollarCashless) MoneyDollarCashless += pay.Amount;
                    //            if (pay.Currency == CurrencyType.RoubleCashless) MoneyRoubleCashless += pay.Amount;
                    //            if (pay.Currency == CurrencyType.EuroCashless) MoneyEuroCashless += pay.Amount;
                }
                //else pay.ToPay = false;
            }

            //    Money = new List<double> { MoneyDollar, MoneyRouble, MoneyEuro, MoneyDollarCashless, MoneyRoubleCashless, MoneyEuroCashless };
            //    OriginatorMoney = CountOriginatorMoney();
            //    ManagerMoney = CountManagerMoney();
            }

            //private ObservableCollection<string> CountOriginatorMoney()
            //{
            //    return new ObservableCollection<string>()
            //        {
            //            (MoneyDollar * OriginatingPartnerPercent / 100).ToString("N0", CultureInfo.InvariantCulture) + " $",
            //            (MoneyRouble * OriginatingPartnerPercent / 100).ToString("N0", CultureInfo.InvariantCulture) + " ₽",
            //            (MoneyEuro * OriginatingPartnerPercent / 100).ToString("N0", CultureInfo.InvariantCulture) + " €",
            //            (MoneyDollarCashless * OriginatingPartnerPercent / 100).ToString("N0", CultureInfo.InvariantCulture) + " $ б/н",
            //            (MoneyRoubleCashless * OriginatingPartnerPercent / 100).ToString("N0", CultureInfo.InvariantCulture) + " ₽ б/н",
            //            (MoneyEuroCashless * OriginatingPartnerPercent / 100).ToString("N0", CultureInfo.InvariantCulture) + " € б/н",
            //        };
            //}

            //private ObservableCollection<string> CountManagerMoney()
            //{
            //    return new ObservableCollection<string>()
            //        {
            //            (MoneyDollar * ManagingPartnerPercent / 100).ToString("N0", CultureInfo.InvariantCulture) + " $",
            //            (MoneyRouble * ManagingPartnerPercent / 100).ToString("N0", CultureInfo.InvariantCulture) + " ₽",
            //            (MoneyEuro * OriginatingPartnerPercent / 100).ToString("N0", CultureInfo.InvariantCulture) + " €",
            //            (MoneyDollarCashless * ManagingPartnerPercent / 100).ToString("N0", CultureInfo.InvariantCulture) + " $ б/н",
            //            (MoneyRoubleCashless * ManagingPartnerPercent / 100).ToString("N0", CultureInfo.InvariantCulture) + " ₽ б/н",
            //            (MoneyEuroCashless * ManagingPartnerPercent / 100).ToString("N0", CultureInfo.InvariantCulture) + " € б/н",
            //        };
            //}
            #endregion

            public void PayMoney()
        {
            //Пока не добавил выплаты для партнёров, потом выплаты будут происходить даже при отсутствии юристов, если партнёры указаны
            if (Lawyers.Count == 0)
            {
                MessageBox.Show("В проекте нет активных юристов");
                return;
            }

            List<Payment> paymentsToPay = new List<Payment>();
            foreach (Payment pay in Payments) if (pay.ToPay) paymentsToPay.Add(pay);
            if (paymentsToPay.Count == 0)
            {
                MessageBox.Show("Активных платежей по проекту нет");
                return;
            }

            AddMoneyToLawyersProjects(OriginatingPartner, OriginatingPartnerPercent);
            AddMoneyToLawyersProjects(ManagingPartner, ManagingPartnerPercent);

            foreach (Lawyer lawyer in Lawyers) AddMoneyToLawyersProjects(lawyer);

            //Если у платежа статус "к уплате", то он был выплачен в данном методе, и мы убираем его в коллекцию выплаченных платежей
            foreach (Payment pay in paymentsToPay)
            {
                PayedPayments.Add(pay);
                Payments.Remove(pay);
            }
            LawyerContext.UpdateProject(this);
        }

        public static double CountMoneyOfCurrency(ObservableCollection<Payment> payments, CurrencyType currencyType)
        {
            double moneyToAdd = 0;
            if (payments?.Count > 0) foreach (Payment payment in payments) if (payment.Currency == currencyType && payment.ToPay) moneyToAdd += payment.Amount;
            return moneyToAdd;
        }

        private void AddMoneyToLawyersProjects(Partner lawyer, float percent)
        {
            LawyersProject thisProject = new LawyersProject();
            foreach (LawyersProject project in lawyer.LawyersProjects)
            {
                if (project.Name == Name)
                {
                    thisProject = project;
                }
            }
            //Здесь добавляем один пэймент за каждую валюту, размера thisProject.Percent * Payment.Amount каждого вида валюты, дата - сегодняшний день
            //Для каждого вида валюты складываем все платежи c соответствующей валютой, умножаем на процент юриста и добавляем платёж
            if (percent <= 0)
            {
                MessageBox.Show($"Партнёру {lawyer.Name} не указан процент. Для проведения выплаты укажите проценты всем юристам");
                return;
            }
            foreach (CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
            {
                double moneyToAdd = CountMoneyOfCurrency(Payments, currency);
                if (moneyToAdd > 0) thisProject.Payments.Add(new Payment() { Amount = percent * moneyToAdd / 100, Date = DateTime.Today, Currency = currency, ProjectName = Name });
            }
            //foreach (Payment payment in thisProject.Payments) MessageBox.Show($"{payment.Amount.ToString()} {payment.Currency} получил {lawyer.Name} по проекту {payment.ProjectName}");
        }

        private void AddMoneyToLawyersProjects(Lawyer lawyer)
        {
            LawyersProject thisProject = new LawyersProject();
            foreach (LawyersProject project in lawyer.LawyersProjects)
            {
                if (project.Name == Name)
                {
                    thisProject = project;
                }
            }
            //Здесь добавляем один пэймент за каждую валюту, размера thisProject.Percent * Payment.Amount каждого вида валюты, дата - сегодняшний день
            //Для каждого вида валюты складываем все платежи c соответствующей валютой, умножаем на процент юриста и добавляем платёж
            if (thisProject.Percent <= 0)
            {
                MessageBox.Show($"Юристу {lawyer.Name} не указан процент. Для проведения выплаты укажите проценты всем юристам");
                return;
            }
            foreach (CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
            {
                double moneyToAdd = CountMoneyOfCurrency(Payments, currency);
                if (moneyToAdd > 0) thisProject.Payments.Add(new Payment() { Amount = thisProject.Percent * moneyToAdd / 100, Date = DateTime.Today, Currency = currency, ProjectName = Name });
            }
            LawyerContext.UpdateLawyer(lawyer);
            //foreach (Payment payment in thisProject.Payments) MessageBox.Show($"{payment.Amount.ToString()} {payment.Currency} получил {lawyer.Name} по проекту {payment.ProjectName}");
        }

        public void AddMoney(int amount, CurrencyType currency)
        {
            Payment pay = new Payment() { Amount = amount, Date = DateTime.Today, Currency = currency };
            //if (Payments.Count == 0) firstPayment = pay;
            Payments.Add(pay);
        }

        public void AddMoney(int amount, CurrencyType currency, DateTime date)
        {
            Payment pay = new Payment() { Amount = amount, Date = date, Currency = currency };
            Payments.Add(pay);
        }

        public void AddLawyer(Lawyer lawyer, float success)
        {
            //lawyer.Projects.Add(this.Name, new SuccessAndProject(success, this)); //(new Project(name, OriginatingPartner, ManagingPartner, isSuccess) { lawyersPercentInProject = success });
            lawyer.LawyersProjects.Add(new LawyersProject(Name) { Percent = success });
            Lawyers.Add(lawyer);
            LawyerContext.UpdateLawyer(lawyer);
            LawyerContext.UpdateProject(this);
        }

        public void RemoveLawyer(Lawyer lawyer)
        {
            //lawyer.Projects.Remove(this.Name);
            foreach (LawyersProject project in lawyer.LawyersProjects)
            {
                if (project.Name == Name)
                {
                    lawyer.LawyersProjects.Remove(project);
                    return;
                }
            }
            Lawyers.Remove(lawyer);
        }

        public void AddPartner(Partner partner)
        {
            if (OriginatingPartner == partner && ManagingPartner == partner) return;
            partner.LawyersProjects.Add(new LawyersProject(Name));
        }

        public void RemovePartner(Partner partner)
        {
            foreach (LawyersProject project in partner.LawyersProjects)
            {
                if (project.Name == Name)
                {
                    partner.LawyersProjects.Remove(project);
                    return;
                }
            }
        }

        public void ShowPaymentsAtDate(string date)
        {
            bool anyPayments = false;
            foreach (Payment payment in Payments)
            {
                if (payment.Date.ToShortDateString() == date)
                {
                    Console.WriteLine(payment.Amount);
                    anyPayments = true;
                }
            }
            if (!anyPayments) Console.WriteLine("Платежей в эту дату не найдено");
        }

        public int CompareTo(object obj)
        {
            return Name.CompareTo(((Project)obj).Name);
        }

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public bool Equals(Project other)
        {
            return null != other && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Project);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
