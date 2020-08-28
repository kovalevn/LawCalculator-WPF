using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LawCalculator_WPF
{
    class Lawyer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Salary { get; set; }
        public ICollection<Project> Project { get; set; }
        public ObservableCollection<LawyersProject> LawyersProjects { get; set; } = new ObservableCollection<LawyersProject>();

        //public Dictionary<string, SuccessAndProject> Projects = new Dictionary<string, SuccessAndProject>();
        //public Dictionary<string, Payment> Payments = new Dictionary<string, Payment>();

        //Как будет выглядеть вкладка "Юристы" - имя каждого юриста в системе, при нажатии на стрелочку - раскрывающийся список проектов, по
        //которым он работает, с суммой заработанного им (в плюсе / в минусе). Такую же для партнеров. Всего три вкладки - проекты, юристы,
        //пратнёры

        public Lawyer(string name, int salary)
        {
            this.Name = name;
            this.Salary = salary;
        }

        public Lawyer() { }

        //public void ShowProjects()
        //{
        //    if (Projects.Count > 0)
        //    {
        //        Console.WriteLine($"Юрист {Name} участвует в проектах:");
        //        foreach (var project in Projects) Console.WriteLine(project.Value.Project.Name);
        //    }
        //    else Console.WriteLine($"Юрист {Name} не занят ни в одном проекте");
        //}
    }
}
