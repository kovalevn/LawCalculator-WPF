using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LawCalculator_WPF
{
    class LawyersProject 
    {
        public string Name { get; set; }
        public float Percent { get; set; }
        public ObservableCollection<Payment> Payments { get; set; } = new ObservableCollection<Payment>();

        public LawyersProject(string name) { Name = name; }
        public LawyersProject() { }
    }
}
