using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORM
{
    public class MyOrm
    {
        public IQueryable<T> GetQuery<T>()
        {
            return new QueryableObject<T>(this);
        }

        public string Statement { get; set; }

        public void RunStatementOnDb(string statement)
        {

        }
    }
}
