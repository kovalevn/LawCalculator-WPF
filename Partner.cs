using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace LawCalculator_WPF
{
    class Partner
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ObservableCollection<LawyersProject> LawyersProjects { get; set; } = new ObservableCollection<LawyersProject>();
        public List<Lawyer> Lawyers = new List<Lawyer>();

        public Partner(string name)
        {
            this.Name = name;
        }

        public Partner()
        {
        }
    }
}
