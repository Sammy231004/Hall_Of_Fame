using System.Runtime.InteropServices;

namespace Hall_Of_Fame.Entities
{
    public class Person
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public ICollection<Skills> Skills { get; set; }
    }

}
