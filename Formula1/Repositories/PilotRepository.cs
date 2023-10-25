using Formula1.Models.Contracts;
using Formula1.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula1.Repositories
{
    public class PilotRepository : IRepository<IPilot>
    {
        public PilotRepository()
        {
            models = new List<IPilot>();
        }
        private List<IPilot> models;
        public IReadOnlyCollection<IPilot> Models => this.models.AsReadOnly();

        public void Add(IPilot model)
        {
            models.Add(model);
        }

        public IPilot FindByName(string name)//used to find a pilot by name
        {
            return models.FirstOrDefault(p => p.FullName == name);
        }

        public bool Remove(IPilot model)//used to remove a pilot by name
        {
            return models.Remove(model);
        }
    }
}
