using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LinkedBaseAndWrapperList
{
    /// <summary>
    /// Wrapper list, the list containging wrapper class of models, the list can only be read from, all intereaction happens with the base class
    /// this is observable and therefore implements INotifyCollectionChanged, and does so by having an observable collection as inner list instead of normal list,
    /// and then using its CollectionChanged event passes the information on
    /// </summary>
    /// <typeparam name="TModel"> Type of model </typeparam>
    /// <typeparam name="TWrapper"> Type of wrapper </typeparam>
    public class ObservableWrapperList<TModel, TWrapper> : WrapperListBase<TModel, TWrapper>, INotifyCollectionChanged where TModel : IModel where TWrapper : IWrapper
    {
        #region events
        //event invoked when anything changes on the lists
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        #endregion

        #region methods
        /// <summary>
        /// Constructor maps to a given base list, by subscribing to its events and saving some of its members in actions
        /// </summary>
        /// <param name="pList"> base list to map to </param>
        public ObservableWrapperList(BaseList<TModel, TWrapper> bList) : base(bList)
        {
            // send CollectionChanged event further through 
            _list.CollectionChanged += (sender, args) => CollectionChanged?.Invoke(sender, args);
        }
        #endregion
    }
}
