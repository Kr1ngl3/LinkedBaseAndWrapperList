using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkedBaseAndWrapperList
{
    interface IWrapperList
    {
        public IEnumerable<IWrapper> List { get; }

        protected abstract void OnListShouldRebuild();
    }
}
