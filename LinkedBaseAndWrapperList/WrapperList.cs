using System.Collections;
using System.Collections.Specialized;

namespace LinkedBaseAndWrapperList
{
    /// <summary>
    /// Wrapper list, the list containging wrapper class of models, the list can only be read from, all intereaction happens with the base class
    /// </summary>
    /// <typeparam name="TModel"> Type of model </typeparam>
    /// <typeparam name="TWrapper"> Type of wrapper </typeparam>
    public class WrapperList<TModel, TWrapper> : WrapperListBase<TModel, TWrapper> where TModel : IModel where TWrapper : IWrapper
    {
        #region list
        // underlying list
        private readonly List<TWrapper> _list = new List<TWrapper>();

        // accessor for non generic list, used in OnCollectionChanged of base class
        protected override IList GetNonGenericList => _list;

        // generic accessor for list
        protected override IList<TWrapper> GetList => _list;
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
        public WrapperList(BaseList<TModel, TWrapper> pList) : base(pList) { }

        /// <summary>
        /// Invokes list should rebuild
        /// </summary>
        protected override void OnCollectionShouldRebuild()
        {
            base.OnCollectionShouldRebuild();
            CollectionChanged?.Invoke();
        }

        /// <summary>
        /// Changes list based on given action, to match base list
        /// </summary>
        /// <param name="action"> Action to run on list </param>
        protected override void OnCollectionChanged(Action<IList> action)
        {
            base.OnCollectionChanged(action);
            CollectionChanged?.Invoke();
        }
        #endregion
    }
}
