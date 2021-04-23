using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Linq;

namespace LawCalculator_WPF
{
    class Project : IComparable, INotifyPropertyChanged, IEquatable<Project>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CurrencyType ProjectCurrency { get; set; }

        private Partner originatingPartner;
        public Partner OriginatingPartner
        {
            get => originatingPartner;
            set 
            {
                originatingPartner = value;
                OnPropertyChanged(nameof(OriginatingPartner));
            }
        }
        public int OriginatingPartnerPercent { get; set; } = 20;

        private Partner managingPartner;
        public Partner ManagingPartner
        { 
            get => managingPartner;
            set 
            {
                managingPartner = value;
                OnPropertyChanged(nameof(ManagingPartner));
            } 
        }
        public int ManagingPartnerPercent { get; set; }
        public bool isSuccess;
        public ObservableCollection<Lawyer> Lawyers { get; set; } = new ObservableCollection<Lawyer>();
        public ObservableCollection<Payment> Payments { get; set; } = new ObservableCollection<Payment>();
        public ObservableCollection<Payment> PayedPayments { get; set; } = new ObservableCollection<Payment>();

        public event PropertyChangedEventHandler PropertyChanged;

        #region Свойства - триггеры видимости элементов

        public Visibility OriginatorVisibilityTrigger => OriginatingPartner == null ? Visibility.Collapsed : Visibility.Visible;
        public Visibility OriginatorInverseVisibilityTrigger => OriginatingPartner == null ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ManagerVisibilityTrigger => ManagingPartner == null ? Visibility.Collapsed : Visibility.Visible;
        public Visibility ManagerInverseVisibilityTrigger => ManagingPartner == null ? Visibility.Visible : Visibility.Collapsed;

        public Visibility LawyersVisibilityTrigger => Lawyers.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        public Visibility LawyersInverseVisibilityTrigger => Lawyers.Count > 0 ? Visibility.Collapsed : Visibility.Visible;

        #endregion

        //Как будем проверять, что надо выплатить в этом квартале: проверяем все платежи, если самый ранний платёж поступил за последние 
        //три месяца - переносим выплату на следующий, если от трёх до шести - выплачиваем все поступившие суммы, если платёж поступил более 
        //чем через 6 месяцв с даты первого платежа - записываем в текущий квартал и выплачиваем

        //По стрелке будет меню - партнёры, юристы, платежи. При нажатии на платежи можно будет увидеть весь список платежей с датой/суммой,
        //или осуществить поиск по дате

        #region Конструкторы

        public Project(string name, Partner originator, Partner manager, bool sucsess)
        {
            this.Name = name;
            OriginatingPartner = originator;
            ManagingPartner = manager;
            isSuccess = sucsess;
            ManagingPartnerPercent = isSuccess ? 40 : 25;
        }

        public Project() { }

        public Project(string name, bool sucsess) 
        {
            this.Name = name;
            isSuccess = sucsess;
            ManagingPartnerPercent = isSuccess ? 40 : 25;
        }

        #endregion

        public void SetProjectCurrency()
        {
            ProjectCurrency = Payments.Where(p => p.ToPay).FirstOrDefault().Currency;
        }

        public void PayMoney()
        {
            //Эта переменная считает, скольким людям не выплатили процент. Если не выплатили никому -
            //платёж не удаляется из коллекции платежей к выплате. Это временный костыль, потом нужно убрать
            int percentsNotPayed = 0;

            if (OriginatingPartner == null || ManagingPartner == null)
            {
                MessageBox.Show("В проект не добавлены партнёры");
                return;
            }

            List<Payment> paymentsToPay = new List<Payment>();
            foreach (Payment pay in Payments) if (pay.ToPay) paymentsToPay.Add(pay);
            if (paymentsToPay.Count == 0)
            {
                MessageBox.Show("Активных платежей по проекту нет");
                return;
            }

            AddMoneyToLawyersProjects(OriginatingPartner, OriginatingPartnerPercent, ref percentsNotPayed);
            AddMoneyToLawyersProjects(ManagingPartner, ManagingPartnerPercent, ref percentsNotPayed);

            foreach (Lawyer lawyer in Lawyers) AddMoneyToLawyersProjects(lawyer, ref percentsNotPayed);

            //Если у платежа статус "к уплате", то он был выплачен в данном методе, и мы убираем его в коллекцию выплаченных платежей
            if (percentsNotPayed >= 2 + Lawyers.Count) return;
            foreach (Payment pay in paymentsToPay)
            {
                PayedPayments.Add(pay);
                Payments.Remove(pay);
            }

        }

        public static double CountMoneyOfCurrency(ObservableCollection<Payment> payments, CurrencyType currencyType)
        {
            double moneyToAdd = 0;
            if (payments?.Count > 0) 
                foreach (Payment payment in payments) 
                    if (payment.Currency == currencyType && payment.ToPay) moneyToAdd += payment.Amount;
            return moneyToAdd;
        }

        private void AddMoneyToLawyersProjects(Partner lawyer, float percent, ref int percentsNotPayed)
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
                MessageBox.Show($"Партнёру {lawyer.Name} не указан процент.");
                percentsNotPayed += 1;
                return;
            }
            foreach (CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
            {
                double moneyToAdd = CountMoneyOfCurrency(Payments, currency);
                if (moneyToAdd > 0) thisProject.Payments.Add(new Payment()
                {
                    Amount = percent * moneyToAdd / 100, 
                    Date = DateTime.Today, 
                    Currency = currency, 
                    ProjectName = Name 
                });
            }
        }

        private void AddMoneyToLawyersProjects(Lawyer lawyer, ref int percentsNotPayed)
        {
            LawyersProject thisProject = lawyer.LawyersProjects.Where(x => x.Name == Name).FirstOrDefault();

            //Здесь добавляем один пэймент за каждую валюту, размера thisProject.Percent * Payment.Amount каждого вида валюты, дата - сегодняшний день
            //Для каждого вида валюты складываем все платежи c соответствующей валютой, умножаем на процент юриста и добавляем платёж
            if (thisProject.Percent <= 0)
            {
                MessageBox.Show($"Юристу {lawyer.Name} не указан процент.");
                percentsNotPayed += 1;
                return;
            }
            foreach (CurrencyType currency in Enum.GetValues(typeof(CurrencyType)))
            {
                double moneyToAdd = CountMoneyOfCurrency(Payments, currency);
                if (moneyToAdd > 0) thisProject.Payments.Add(new Payment() 
                { 
                    Amount = thisProject.Percent * moneyToAdd / 100, 
                    Date = DateTime.Today, 
                    Currency = currency, 
                    ProjectName = Name 
                });
            }
        }

        public void AddProjectToPartner(Partner partner)
        {
            if (OriginatingPartner == partner && ManagingPartner == partner) return;
            partner.LawyersProjects.Add(new LawyersProject(Name));
        }

        #region Реализация методов интерфейсов
        public int CompareTo(object obj)
        {
            return Name.CompareTo(((Project)obj).Name);
        }

        public void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
        #endregion
    }
}
