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

                //db.Entry(lawyer).State = EntityState.Modified;
                //db.SaveChanges();
                Lawyer lwyr = db.Lawyers.Include("LawyersProjects").Include("LawyersProjects.Payments").FirstOrDefault(l => l.Id == lawyer.Id);
                //db.Entry(lwyr).State = EntityState.Modified;
                lwyr.Salary = lawyer.Salary;
                lwyr.Name = lawyer.Name;
                CompareCollections(lwyr.LawyersProjects, lawyer.LawyersProjects);
                foreach (LawyersProject lp in lawyer.LawyersProjects)
                {
                    var lwyrpr = lwyr.LawyersProjects.Where(p => p.Id == lp.Id).FirstOrDefault();
                    lwyrpr.Percent = lp.Percent;
                    var payments = lwyrpr.Payments;
                    if (lp.Payments.Count > 0) CompareCollections(payments, lp.Payments);
                }

                db.SaveChanges();
            }
        }

        public static void UpdatePartner(Partner lawyer)
        {
            using (LawyerContext db = new LawyerContext())
            {
                if (lawyer == null) return;
                Partner lwyr = db.Partners.Include("LawyersProjects").Include("LawyersProjects.Payments").Where(l => l.Id == lawyer.Id).FirstOrDefault();
                lwyr.Name = lawyer.Name;
                CompareCollections(lwyr.LawyersProjects, lawyer.LawyersProjects);
                foreach (LawyersProject lp in lawyer.LawyersProjects)
                {
                    var lwyrpr = lwyr.LawyersProjects.Where(p => p.Id == lp.Id).FirstOrDefault();
                    lwyrpr.Percent = lp.Percent;
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
                Project dbProj = db.Projects.Include("Payments").Include("Lawyers").Where(p => p.Name == project.Name).FirstOrDefault();
                if (project.OriginatingPartner != null) dbProj.OriginatingPartner = db.Partners.Where(p => p.Id == project.OriginatingPartner.Id).FirstOrDefault();
                if (project.ManagingPartner != null) dbProj.ManagingPartner = db.Partners.Where(p => p.Id == project.ManagingPartner.Id)?.FirstOrDefault();
                dbProj.OriginatingPartnerPercent = project.OriginatingPartnerPercent;
                dbProj.ManagingPartnerPercent = project.ManagingPartnerPercent;
                dbProj.OriginatorVisibilityTrigger = project.OriginatorVisibilityTrigger;
                dbProj.ManagerVisibilityTrigger = project.ManagerVisibilityTrigger;
                foreach (Lawyer l in project.Lawyers)
                {
                    bool doNotAddLawyer = false;
                    foreach (Lawyer lawyer in dbProj.Lawyers) if (lawyer.Name == l.Name) doNotAddLawyer = true;
                    if (!doNotAddLawyer) dbProj.Lawyers.Add(db.Lawyers.Where(ly => ly.Name == l.Name).FirstOrDefault());
                }

                CompareCollections(dbProj.Payments, project.Payments, db.Payments);
                CompareCollections(dbProj.PayedPayments, project.PayedPayments, db.Payments);

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

        private static void CompareCollections<T>(ObservableCollection<T> dbCollection, ObservableCollection<T> objCollection, DbSet<T> dbCollectionToAdd) where T : class, IHaveId
        {
            foreach (T obj in objCollection)
            {
                if (!dbCollection.Contains(obj)) dbCollection.Add(dbCollectionToAdd.Where(o => o.Id == obj.Id).FirstOrDefault());
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
