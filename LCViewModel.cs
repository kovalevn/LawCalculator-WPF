using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using OfficeOpenXml;
using System.Globalization;
using System.Data.Entity;

namespace LawCalculator_WPF
{
    class LCViewModel
    {
        public ObservableCollection<Project> AllProjects { get; set; } = new ObservableCollection<Project>();
        public ObservableCollection<Lawyer> AllLawyers { get; set; } = new ObservableCollection<Lawyer>();
        public ObservableCollection<Partner> AllPartners { get; set; } = new ObservableCollection<Partner>();

        public LawyerContext db = new LawyerContext();

        public LCViewModel()
        {
            //Получаем курсы валют
            CurrencyConverter.Initialize();
            
            //Загружаем информацию из бд
            db.Projects.Include("Lawyers").Include("Payments").Include("PayedPayments").Load();
            AllProjects = db.Projects.Local;

            db.Lawyers.Include("LawyersProjects").Include("LawyersProjects.Payments").Load();
            AllLawyers = db.Lawyers.Local;

            db.Partners.Include("LawyersProjects").Include("LawyersProjects.Payments").Load();
            AllPartners = db.Partners.Local;

            //Выставляем начальную валюту для каждого проекта
            foreach (Project project in AllProjects) project.SetProjectCurrency();
        }

        public void GetInfoFromSpreadsheet(string sheetPath, int dollar, int euro, int rouble, bool dateFromActs)
        {
            var sheetAdress = new FileInfo(sheetPath);
            using (var p = new ExcelPackage(sheetAdress))
            {
                int[] sheets = new int[] { dollar, euro, rouble };
                foreach (int sheet in sheets)
                {
                    var currentSheet = p.Workbook.Worksheets[sheet];
                    for (int row = 2; row < 600; row++)
                    {
                        if (!string.IsNullOrEmpty((string)currentSheet.Cells[row, 4].Value))
                        {
                            if (currentSheet.Cells[row, 4].Value.ToString() != "Итого за день:" && currentSheet.Cells[row, 4].Value.ToString() != "Исходящий остаток"
                                && currentSheet.Cells[row, 4].Value.ToString() != "EUR" && currentSheet.Cells[row, 4].Value.ToString() != "USD"
                                && !currentSheet.Cells[row, 4].Value.ToString().ToLower().Contains("входящий остаток"))
                            {
                                string projName = currentSheet.Cells[row, 4].Value.ToString().Trim();
                                bool doNotAddPayment = false;
                                bool doNotAddProj = false;
                                DateTime date;

                                if (dateFromActs)
                                {

                                    try
                                    {
                                        date = Convert.ToDateTime(currentSheet.Cells[row, 6].Value, CultureInfo.InvariantCulture);
                                    }
                                    catch (FormatException)
                                    {
                                        string dateString = currentSheet.Cells[row, 6].Value.ToString();
                                        //Поправить логику: там, где нет актов - всегда выбираем из второй клетки
                                        if (dateString == "Возмещение расходов") date = DateTime.FromOADate((double)currentSheet.Cells[row, 2].Value);
                                        date = DateTime.FromOADate((double)currentSheet.Cells[row, 2].Value);
                                    }
                                }
                                else date = DateTime.FromOADate((double)currentSheet.Cells[row, 2].Value);


                                Payment pay = new Payment()
                                {
                                    Amount = (double)currentSheet.Cells[row, 5].Value,
                                    Currency = sheet == dollar ? CurrencyType.Dollar : sheet == euro ? CurrencyType.Euro : CurrencyType.Rouble,
                                    Date = date,
                                    ProjectName = projName 
                                };
                                //Добавить тут давность платежей, не учитываем те, которые пришли раньше, чем 3 месяца до сегодняшней даты, а также те, что старше Н месяцев
                                pay.ToPay = DateTime.Compare(pay.Date, DateTime.Today.AddMonths(-3)) <= 0 && !pay.Payed;
                                Project newProj = new Project(projName, false);
                                foreach (Project proj in AllProjects)
                                {
                                    if (proj.Name == projName)
                                    {
                                        doNotAddProj = true;
                                        foreach (Payment payment in proj.Payments) 
                                        {
                                            if (payment.Amount == pay.Amount && payment.Date == pay.Date && payment.Currency == pay.Currency) doNotAddPayment = true;
                                        }
                                        foreach (Payment payment in proj.PayedPayments)
                                        {
                                            if (payment.Amount == pay.Amount && payment.Date == pay.Date && payment.Currency == pay.Currency) doNotAddPayment = true;
                                        }
                                        if (!doNotAddPayment) 
                                        {
                                            proj.Payments.Add(pay);
                                            using (LawyerContext db = new LawyerContext())
                                            {
                                                Project pr = db.Projects.Include("Payments").Where(p => p.Name == proj.Name).FirstOrDefault();
                                                pr.Payments.Add(pay);
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                }

                                if (!doNotAddProj)
                                {
                                    newProj.Payments.Add(pay);
                                    AllProjects.Add(newProj);
                                    using (LawyerContext db = new LawyerContext())
                                    {
                                        db.Projects.Add(newProj);
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            foreach (Project project in AllProjects) project.SetProjectCurrency();
            MessageBox.Show("Платежи выгружены");
        }
    }
}
