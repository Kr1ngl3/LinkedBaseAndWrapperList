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
    public class ObservableWrapperList<TModel, TWrapper> : INotifyCollectionChanged where TModel : IModel where TWrapper : IWrapper
    {
        #region list
        // underlying list
        private readonly ObservableCollection<TWrapper> _list = new ObservableCollection<TWrapper>();
        // public gettor of the list
        public IEnumerable<TWrapper> List => _list;
        #endregion

        #region actions
        // action used to rebuild lists, in an action as to not need to have field for base list
        private readonly Action _rebuildCollectionAction;
        #endregion

        #region events
        //event invoked when anything changes on the lists
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        #endregion

        #region methods
        /// <summary>
        /// Constructor maps to a given base list, by subscribing to its events and saving some of its members in actions
        /// </summary>
        /// <param name="pList"> base list to map to </param>
        public ObservableWrapperList(BaseList<TModel, TWrapper> pList)
        {
            // send CollectionChanged event further through 
            _list.CollectionChanged += (sender, args) => CollectionChanged?.Invoke(sender, args);

            // creates rebuild action
            _rebuildCollectionAction = new Action(() =>
            {
                _list.Clear();
                foreach (TModel t in pList.List)
                    _list.Add((TWrapper)t.ToWrapper);
            });

            _rebuildCollectionAction.Invoke();

            // subscribes to events
            pList.CollectionShouldRebuild += OnCollectionShouldRebuild;
            pList.CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        /// Invokes list should rebuild
        /// </summary>
        private void OnCollectionShouldRebuild()
        {
            _rebuildCollectionAction.Invoke();
        }

        /// <summary>
        /// Changes list based on given action, to match base list
        /// </summary>
        /// <param name="action"> Action to run on list </param>
        private void OnCollectionChanged(Action<ObservableCollection<TWrapper>> action)
        {
            action.Invoke(_list);
        }
        #endregion
    }
}
