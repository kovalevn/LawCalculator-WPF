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

        public void Add<T>(ObservableCollection<T> dbCollection, T objectToAdd)
        {
            dbCollection.Add(objectToAdd);
            SaveChanges();
        }

        public void Update<T>(T obj)
        {
            Entry(obj).State = EntityState.Modified;
            SaveChanges();
        }
        public void Remove<T>(ObservableCollection<T> dbCollection, T objectToRemove)
        {
            dbCollection.Remove(objectToRemove);
            SaveChanges();
        }
    }
}
