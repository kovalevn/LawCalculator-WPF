using System.Globalization;

namespace LawCalculator_WPF
{
    class Currency
    {
        public string code { get; set; }
        public string name { get; set; }
        public double rate { get; set; }
        public string dateString { get; set; }
        public double inverseRate { get; set; }

        public Currency(string code, string name, string rate, string date, string inverseRate)
        {
            this.code = code;
            this.name = name;
            this.rate = double.Parse(rate, NumberStyles.Float, CultureInfo.InvariantCulture);
            this.dateString = date;
            this.inverseRate = double.Parse(inverseRate, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return this.code + " - " + this.rate;
        }
    }
}
