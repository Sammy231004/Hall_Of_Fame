using System.Runtime.InteropServices;

namespace Hall_Of_Fame.Entities
{
    public class Skills
    {

        public long Id { get; set; }
        public string Name { get; set; }
        public byte Level { get; set; }

        public long PersonId { get; set; }
    }
}
