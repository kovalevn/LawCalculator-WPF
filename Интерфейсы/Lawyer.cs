using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LawCalculator_WPF.Интерфейсы
{
    interface Lawyer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Salary { get; set; }
        public ICollection<Project> Project { get; set; }
        public ObservableCollection<LawyersProject> LawyersProjects { get; set; }

        public double CountBalance(DateTime date);
    }
}
