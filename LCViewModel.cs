using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OfficeOpenXml;

namespace LawCalculator_WPF
{
    class LCViewModel
    {
        public ObservableCollection<Project> AllProjects { get; set; } = new ObservableCollection<Project>();
        public ObservableCollection<Lawyer> AllLawyers { get; set; } = new ObservableCollection<Lawyer>();
        public ObservableCollection<Partner> AllPartners { get; set; } = new ObservableCollection<Partner>();

        public LCViewModel()
        {
            //Partner Kovalev = new Partner("Сергей Ковалев");
            //Partner Kislov = new Partner("Сергей Кислов");
            //Partner Tugushi = new Partner("Дмитрий Тугуши");
            //Project Clarke = new Project("Clarke Invest Group", Kislov, Kovalev, false);
            //Lawyer Ivan = new Lawyer("Иван", 60000);
            //Lawyer Roman = new Lawyer("Роман", 80000);

            //Clarke.AddLawyer(Ivan, 5);
            //Clarke.AddLawyer(Roman, 7);
            //Clarke.AddMoney(10000, CurrencyType.Rouble);
            //Clarke.AddMoney(10000, CurrencyType.Rouble, DateTime.Today.AddMonths(-4));

            //Получаем курсы валют
            CurrencyConverter.Initialize();

            //Заполняем коллекции юристов, проектов и партнёров из базы данных
            using (LawyerContext db = new LawyerContext())
            {
                var lawyers = db.Lawyers;
                var projects = db.Projects;
                var partners = db.Partners;
                //projects.Add(Clarke);
                //db.SaveChanges();

                foreach (Project project in projects)
                {
                    Project proj = db.Projects.Include("Lawyers").Include("Payments").Include("PayedPayments").Where(p => p.Name == project.Name).FirstOrDefault();
                    AllProjects.Add(proj);
                }

                //lawyers.Add(Ivan);
                foreach (Lawyer lawyer in lawyers)
                {
                    Lawyer lwyr = db.Lawyers.Include("LawyersProjects").Include("LawyersProjects.Payments").Where(l => l.Name == lawyer.Name).FirstOrDefault();
                    AllLawyers.Add(lwyr);
                }

                foreach (Partner partner in partners)
                {
                    Partner prtnr = db.Partners.Include("LawyersProjects").Include("LawyersProjects.Payments").Where(p => p.Name == partner.Name).FirstOrDefault();
                    AllPartners.Add(prtnr);
                }

                //foreach (Lawyer lawyer in lawyers)
                //{
                //    MessageBox.Show($"{lawyer.Name}, {lawyer.Salary}");
                //    foreach(LawyersProject project in lawyer.LawyersProjects)
                //    {
                //        MessageBox.Show($"{project.Name}");
                //    }
                //}
            }


            //AllProjects.Add(Clarke);

            //AllPartners.Add(Kovalev);
            //AllPartners.Add(Kislov);
            //AllPartners.Add(Tugushi);

            //AllLawyers.Add(Ivan);
            //AllLawyers.Add(Roman);

            //GetInfoFromSpreadsheet();
            foreach (Project project in AllProjects) project.CountMoneyOld();
            AllProjects = new ObservableCollection<Project>(AllProjects.OrderBy(i => i));
        }

        public void GetInfoFromSpreadsheet()
        {
            //var sheetAdress = new FileInfo(@"C:\Users\Никита\Desktop\Проекты\LawCalculator WPF\Движение по счетам_2020_.xlsx");
            var sheetAdress = new FileInfo(@"C:\Users\Никита\Desktop\Проекты\LawCalculator WPF\Движение по счетам_2020_с актами.xlsx");
            using (var p = new ExcelPackage(sheetAdress))
            {
                //int[] sheets = new int[] { 1, 3, 4 };
                int[] sheets = new int[] { 0, 1, 2 };
                foreach (int sheet in sheets)
                {
                    var currentSheet = p.Workbook.Worksheets[sheet];
                    for (int row = 2; row < 600; row++)
                    {
                        if (!string.IsNullOrEmpty((string)currentSheet.Cells[row, 4].Value))
                        {
                            if (currentSheet.Cells[row, 4].Value.ToString() != "Итого за день:" && currentSheet.Cells[row, 4].Value.ToString() != "Исходящий остаток"
                                && currentSheet.Cells[row, 4].Value.ToString() != "EUR" && currentSheet.Cells[row, 4].Value.ToString() != "USD")
                            {
                                string projName = currentSheet.Cells[row, 4].Value.ToString().Trim();
                                bool doNotAddPayment = false;
                                bool doNotAddProj = false;
                                Payment pay = new Payment() { Amount = (double)currentSheet.Cells[row, 5].Value, Currency = sheet == 0 ? CurrencyType.Dollar : sheet == 1 ? CurrencyType.Euro : CurrencyType.Rouble, Date = DateTime.FromOADate((double)currentSheet.Cells[row, 2].Value), ProjectName = projName };
                                pay.ToPay = DateTime.Compare(pay.Date, DateTime.Today.AddMonths(-3)) <= 0 && !pay.Payed ? true : false;
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
            foreach (Project project in AllProjects) project.CountMoneyOld();
            MessageBox.Show("Платежи выгружены");
        }
    }
}
