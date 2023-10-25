using Formula1.Models.Contracts;
using Formula1.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula1.Repositories
{
    public class FormulaOneCarRepository : IRepository<IFormulaOneCar> 
    {
        public FormulaOneCarRepository()
        {
            models = new List<IFormulaOneCar>();
        }
        private List<IFormulaOneCar> models;
        public IReadOnlyCollection<IFormulaOneCar> Models => this.models.AsReadOnly();

        public void Add(IFormulaOneCar model)
        {
            models.Add(model);
        }

        public IFormulaOneCar FindByName(string name)//used to find a car by model
        {
            return models.FirstOrDefault(c => c.Model == name);
        }

        public bool Remove(IFormulaOneCar model)//used to remove a car by name
        {
            return models.Remove(model);

        }
    }
}
