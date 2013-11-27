using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TestDataContext : AbstractDataContext
    {
        public TestDataContext()
            : base("TestProebank")
        {
        }

        /// <summary>
        /// This implementation keeps all changes in memory ("Local" property)
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            return 0;
        }
    }
}
