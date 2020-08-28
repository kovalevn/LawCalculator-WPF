using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace LawCalculator_WPF
{
    class LawyerContext : DbContext
    {
        public LawyerContext() : base("DbConnection") { }

        public DbSet<Lawyer> Lawyers { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<LawyersProject> LawyersProjects { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Project> Projects { get; set; }

        public static void UpdateLawyer(Lawyer lawyer)
        {
            using (LawyerContext db = new LawyerContext())
            {
                Lawyer lwyr = db.Lawyers.Include("LawyersProjects").Include("LawyersProjects.Payments").Where(l => l.Id == lawyer.Id).FirstOrDefault();
                lwyr.Salary = lawyer.Salary;
                lwyr.Name = lawyer.Name;
                CompareCollections(lwyr.LawyersProjects, lawyer.LawyersProjects);
                foreach(LawyersProject lp in lawyer.LawyersProjects)
                {
                    var lwyrpr = lwyr.LawyersProjects.Where(p => p.Id == lp.Id).FirstOrDefault();
                    var payments = lwyrpr.Payments;
                    if (lp.Payments.Count > 0) CompareCollections(payments, lp.Payments);
                }

                db.SaveChanges();
            }
        }

        public static void UpdateProject(Project project)
        {
            using (LawyerContext db = new LawyerContext())
            {
                Project proj = db.Projects.Include("Payments").Include("Lawyers").Where(p => p.Name == project.Name).FirstOrDefault();
                foreach (Lawyer l in project.Lawyers)
                {
                    bool doNotAddLawyer = false;
                    foreach (Lawyer lawyer in proj.Lawyers) if (lawyer.Name == l.Name) doNotAddLawyer = true;
                    if (!doNotAddLawyer) proj.Lawyers.Add(db.Lawyers.Where(ly => ly.Name == l.Name).FirstOrDefault());
                }

                //if (proj.Payments.Count < project.Payments.Count)
                //{
                //    foreach (Payment p in project.Payments)
                //    {
                //        bool doNotAddPay = false;
                //        foreach (Payment payment in proj.Payments) if (payment.Id == p.Id) doNotAddPay = true;
                //        if (!doNotAddPay) proj.Payments.Add(db.Payments.Where(py => py.Id == p.Id).FirstOrDefault());
                //    }
                //}
                //else if (proj.Payments.Count > project.Payments.Count) 
                //{
                //    //Project projToRemove = proj.Payments.SingleOrDefault(py => )
                //}

                //Переделать. Платежи должны удаляться из пэйментс и уходить в пэйд пэйментс, т.е. не терять айди, а менять место в таблице
                //а сейчас старый платёж убирается и пэйментс, и создаётся новый для пэйд пэйментс
                CompareCollections(proj.Payments, project.Payments);
                CompareCollections(proj.PayedPayments, project.PayedPayments);

                db.SaveChanges();
            }
        }

        private static void CompareCollections<T>(ObservableCollection<T> dbCollection, ObservableCollection<T> objCollection)
        {
            foreach (T obj in objCollection)
            {
                if (!dbCollection.Contains(obj)) dbCollection.Add(obj);
            }

            for (int i = 0; i < dbCollection.Count; i++)
            {
                if (!objCollection.Contains(dbCollection[i])) 
                {
                    dbCollection.Remove(dbCollection[i]);
                    i--;
                } 
            }
        }
    }
}
