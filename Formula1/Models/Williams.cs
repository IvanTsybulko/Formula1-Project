using System;
using System.Collections.Generic;
using System.Text;

namespace Formula1.Models
{
    public class Williams : FormulaOneCar //Car type which inherits the f1car
    {
        public Williams(string model, int horsepower, double engineDisplacement) : base(model, horsepower, engineDisplacement)
        {
        }
    }
}
