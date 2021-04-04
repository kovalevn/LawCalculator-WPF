using System;
using System.Collections.Generic;
using System.Text;

namespace LawCalculator_WPF
{
    //Интерфейс для объектов, имеющих ID в базе данных
    interface IHaveId
    {
        public int Id { get; set; }
    }
}
