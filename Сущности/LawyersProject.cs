using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LawCalculator_WPF
{
    class LawyersProject : IEquatable<LawyersProject>, IHaveId
    {
        public int Id { get; set; }
        public Project Project { get; set; }
        public int ProjectId;
        public string Name { get; private set; }
        public float Percent { get; set; }
        public ObservableCollection<Payment> Payments { get; set; } = new ObservableCollection<Payment>();

        public LawyersProject(Project project)
        {
            Project = project;
            Name = Project.Name; 
        }
        public LawyersProject() { }

        public bool Equals(LawyersProject other)
        {
            return null != other && Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as LawyersProject);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}
