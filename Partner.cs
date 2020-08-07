using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LawCalculator_WPF
{
    class Partner : Lawyer
    {
        public List<Lawyer> Lawyers = new List<Lawyer>();

        public Partner(string name)
        {
            this.Name = name;
        }
    }
}
