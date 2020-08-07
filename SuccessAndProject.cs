using System;
using System.Collections.Generic;
using System.Text;

namespace LawCalculator_WPF
{
    class SuccessAndProject
    {
        public float Success { get; set; }
        public Project Project { get; set; }

        public SuccessAndProject(float success, Project project)
        {
            Success = success;
            Project = project;
        }
    }
}
