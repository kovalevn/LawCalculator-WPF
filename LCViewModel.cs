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

        public LCViewModel()
        {
            Partner Kovalev = new Partner("Сергей Ковалев");
            Partner Kislov = new Partner("Сергей Кислов");
            Partner Tugushi = new Partner("Дмитрий Тугуши");
            Lawyer Ivan = new Lawyer("Иван", 60000);
            Lawyer Roman = new Lawyer("Роман", 80000);

            //Получаем курсы валют
            CurrencyConverter.Initialize();

            //Заполняем коллекции юристов, проектов и партнёров из базы данных
            using (LawyerContext db = new LawyerContext())
            {
                var lawyers = db.Lawyers;
                var projects = db.Projects;
                var partners = db.Partners;

                //Раcкомментить для создания юристов и партнёров после удаления датабазы
                //lawyers.Add(Ivan);
                //lawyers.Add(Roman);
                //partners.Add(Kovalev);
                //partners.Add(Kislov);
                //partners.Add(Tugushi);
                //db.SaveChanges();

                foreach (Project project in projects)
                {
                    Project proj = db.Projects.Include("Lawyers").Include("Payments").Include("PayedPayments").Where(p => p.Name == project.Name).FirstOrDefault();
                    AllProjects.Add(proj);
                }

                foreach (Lawyer lawyer in lawyers)
                {
                    Lawyer lwyr = db.Lawyers.Include("LawyersProjects").Include("LawyersProjects.Payments").Where(l => l.Name == lawyer.Name).FirstOrDefault();
                    AllLawyers.Add(lwyr);
                }

                //db.Lawyers.Include("LawyersProjects").Include("LawyersProjects.Payments").Load();
                //AllLawyers = db.Lawyers.Local;

                foreach (Partner partner in partners)
                {
                    Partner prtnr = db.Partners.Include("LawyersProjects").Include("LawyersProjects.Payments").Where(p => p.Name == partner.Name).FirstOrDefault();
                    AllPartners.Add(prtnr);
                }
            }

            //Выставляем начальную валюту для каждого проекта
            foreach (Project project in AllProjects) project.SetProjectCurrency();

            //Сортируем проекты
            AllProjects = new ObservableCollection<Project>(AllProjects.OrderBy(i => i));
        }

        public void GetInfoFromSpreadsheet(string sheetPath, int dollar, int euro, int rouble, bool dateFromActs)
        {
            //var sheetAdress = new FileInfo(@"C:\Users\Никита\Desktop\Проекты\LawCalculator WPF\Движение по счетам_2020_.xlsx");
            //var sheetAdress = new FileInfo(@"C:\Users\Никита\Desktop\Проекты\LawCalculator WPF\Движение по счетам_2020_с актами.xlsx");
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
