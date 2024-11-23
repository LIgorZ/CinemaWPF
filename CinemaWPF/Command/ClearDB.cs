using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CinemaWPF.Model.CFilmsShortView;
using static CinemaWPF.Model.CinemaDB;

namespace CinemaWPF.Command
{
    public class ClearDB : CommandBase
    {
        public override void Execute(object? parameter)
        {
            using (Model.CinemaDB.AppContext db = new Model.CinemaDB.AppContext("Data Source=" + filenameDB))
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }
    }
}
