using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CinemaWPF.Model
{
    public class CPerson
    {
        public class ViewInfoPerson
        {
            public int Id { get; set; }
            public int? IdKP { get; set; }
            public string? Name { get; set; }
            public string? Fact {  get; set; }
            public string? Description { get; set; }
            public string? Year { get; set; }
            public string? Profession { get; set; }
            public string? EnProfession { get; set; }
            public string? EnName { get; set; }
            public BitmapImage? Photo {  get; set; }
            public string? Films {  get; set; }
            public string? Birthday { get; set; }
            public string? Death { get; set; }
            public string? Age { get; set; }
        }
    }
}
