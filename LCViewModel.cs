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
            Partner Kovalev = new Partner("Сергей Ковалев");
            Partner Kislov = new Partner("Сергей Кислов");
            Partner Tugushi = new Partner("Дмитрий Тугуши");
            Project Clarke = new Project("Clarke Invest Group", Kislov, Kovalev, false);
            Lawyer Ivan = new Lawyer("Иван", 60000);
            Lawyer Roman = new Lawyer("Роман", 80000);

            Clarke.AddLawyer(Ivan, 5);
            Clarke.AddLawyer(Roman, 7);
            Clarke.AddMoney(10000, CurrencyType.Rouble);
            Clarke.AddMoney(10000, CurrencyType.Rouble, DateTime.Today.AddMonths(-4));

            AllProjects.Add(Clarke);

            AllPartners.Add(Kovalev);
            AllPartners.Add(Kislov);
            AllPartners.Add(Tugushi);

            AllLawyers.Add(Ivan);
            AllLawyers.Add(Roman);

            GetInfoFromSpreadsheet();
            foreach (Project project in AllProjects) project.CountMoneyOld();
            AllProjects = new ObservableCollection<Project>(AllProjects.OrderBy(i => i));
        }

        private void GetInfoFromSpreadsheet()
        {
            var sheetAdress = new FileInfo(@"C:\Users\Никита\Desktop\Проекты\LawCalculator WPF\Движение по счетам_2020_.xlsx");
            using (var p = new ExcelPackage(sheetAdress))
            {
                int[] sheets = new int[] { 1, 3, 4 };
                foreach (int sheet in sheets)
                {
                    var currentSheet = p.Workbook.Worksheets[sheet];
                    for (int row = 4; row < 600; row++)
                    {
                        if (!string.IsNullOrEmpty((string)currentSheet.Cells[row, 4].Value))
                        {
                            if (currentSheet.Cells[row, 4].Value.ToString() != "Итого за день:" && currentSheet.Cells[row, 4].Value.ToString() != "Исходящий остаток")
                            {
                                string projName = currentSheet.Cells[row, 4].Value.ToString().Trim();
                                bool doNotAddPayment = false;
                                bool doNotAddProj = false;
                                //MessageBox.Show(projName);
                                Payment pay = new Payment() { Amount = (double)currentSheet.Cells[row, 5].Value, Currency = sheet == 1 ? CurrencyType.Dollar : sheet == 3 ? CurrencyType.Euro : CurrencyType.Rouble, Date = DateTime.FromOADate((double)currentSheet.Cells[row, 2].Value), ProjectName = projName };
                                Project newProj = new Project(projName, AllPartners[new Random().Next(0, AllPartners.Count)],
                                        AllPartners[new Random().Next(0, AllPartners.Count)], false);
                                foreach (Project proj in AllProjects)
                                {
                                    if (proj.Name == projName)
                                    {
                                        doNotAddProj = true;
                                        foreach (Payment payment in proj.Payments) 
                                        {
                                            if (payment.Amount == pay.Amount && payment.Date == pay.Date && payment.Currency == pay.Currency) doNotAddPayment = true;
                                        }
                                        if(!doNotAddPayment) proj.Payments.Add(pay);
                                    }
                                }

                                if (!doNotAddProj)
                                {
                                    newProj.Payments.Add(pay);
                                    AllProjects.Add(newProj);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
