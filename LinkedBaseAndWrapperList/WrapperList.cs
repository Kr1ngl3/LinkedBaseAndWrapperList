using System.Collections.Specialized;

namespace LinkedBaseAndWrapperList
{
    /// <summary>
    /// Wrapper list, the list containging wrapper class of models, the list can only be read from, all intereaction happens with the base class
    /// </summary>
    /// <typeparam name="TModel"> Type of model </typeparam>
    /// <typeparam name="TWrapper"> Type of wrapper </typeparam>
    public class WrapperList<TModel, TWrapper> where TModel : IModel where TWrapper : IWrapper
    {
        #region list
        // underlying list
        private readonly List<TWrapper> _list = new List<TWrapper>();
        // Exposed IEnumerable of the list
        public IEnumerable<TWrapper> List => _list;
        #endregion

        #region actions
        // action used to rebuild lists, in an action as to not need to have field for base list
        private readonly Action _rebuildCollectionAction;
        #endregion

        #region events
        //event invoked when anything changes on the lists
        public event Action? CollectionChanged;
        #endregion

        #region methods
        /// <summary>
        /// Constructor maps to a given base list, by subscribing to its events and saving some of its members in actions
        /// </summary>
        /// <param name="pList"> base list to map to </param>
        public WrapperList(BaseList<TModel, TWrapper> pList)
        {
            // creates rebuild action
            _rebuildCollectionAction = new Action(() =>
            {
                _list.Clear();
                foreach (TModel t in pList)
                    _list.Add((TWrapper)t.ToWrapper);
            });

            _rebuildCollectionAction.Invoke();

            // subscribes to events
            pList.CollectionShouldRebuild += OnCollectionShouldRebuild;
            pList.CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        /// Indexed getter 
        /// </summary>
        /// <param name="index"> Index to get wrapper item from </param>
        /// <returns> Returns item at given index </returns>
        public TWrapper this[int index] { get => _list[index]; }

        /// <summary>
        /// Invokes list should rebuild
        /// </summary>
        private void OnCollectionShouldRebuild()
        {
            _rebuildCollectionAction.Invoke();
            CollectionChanged?.Invoke();
        }

        /// <summary>
        /// Changes list based on given action, to match base list
        /// </summary>
        /// <param name="action"> Action to run on list </param>
        private void OnCollectionChanged(Action<List<TWrapper>> action)
        {
            action.Invoke(_list);
            CollectionChanged?.Invoke();
        }
        #endregion
    }
}
