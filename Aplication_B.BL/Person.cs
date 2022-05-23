using MessagePack;
using System;

namespace Aplication_B.BL
{

    [MessagePackObject]
    public class Person
    {
        [Key(0)]
        public string Name { get; set; }

        [Key(1)]
        public int PersonalId { get; set; }

        [Key(2)]
        public DateTime LastUpdated { get; set; }


    }
}
