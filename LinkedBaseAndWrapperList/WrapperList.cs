namespace LinkedBaseAndWrapperList
{
    /// <summary>
    /// Wrapper list, the list containging wrapper class of models, the list can only be read from, all intereaction happens with the base class
    /// </summary>
    /// <typeparam name="TModel"> Type of model </typeparam>
    /// <typeparam name="TWrapper"> Type of wrapper </typeparam>
    class WrapperList<TModel, TWrapper> where TModel : IModel where TWrapper : IWrapper
    {
        #region list
        // underlying list
        private readonly List<TWrapper> _list = new List<TWrapper>();
        // public gettor of the list
        public IEnumerable<TWrapper> List => _list;
        #endregion

        #region actions
        // action used to rebuild lists, in an action as to not need to have field for base list
        private readonly Action _rebuildListAction;
        #endregion

        #region events
        //event invoked when anything changes og the lists
        public event Action ListChanged = null!;
        #endregion

        #region methods
        /// <summary>
        /// Constructor maps to a given base list, by subscribing to its events and saving some of its members in actions
        /// </summary>
        /// <param name="pList"> base list to map to </param>
        public WrapperList(BaseList<TModel, TWrapper> pList)
        {
            // creates rebuild action
            _rebuildListAction = new Action(() =>
            {
                _list.Clear();
                foreach (TModel t in pList.List)
                    _list.Add((TWrapper)t.ToWrapper);
            });

            _rebuildListAction.Invoke();

            // subscribes to events
            pList.ListShouldRebuild += OnListShouldRebuild;
            pList.ListChanged += OnListChanged;
        }

        /// <summary>
        /// Invokes list should rebuild
        /// </summary>
        private void OnListShouldRebuild()
        {
            _rebuildListAction.Invoke();
            ListChanged?.Invoke();
        }

        /// <summary>
        /// Changes list based on given action, to match base list
        /// </summary>
        /// <param name="action"> Action to run on list </param>
        private void OnListChanged(Action<List<TWrapper>> action)
        {
            action.Invoke(_list);
            ListChanged?.Invoke();
        }
        #endregion
    }
}
